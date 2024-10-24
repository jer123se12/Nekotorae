using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class normalCat : basecat
{
    // properties of cat
    BoxCollider colli;
    Rigidbody rb;
    AudioSource con;
    public List<AudioClip> normalClip;
    MeshRenderer rend;
    Animator ani;

    // outside variables
    public float width=1.8f;
    public float length=1.5f;
    public float speed=5;
    public float rotateSpeed=180;
    public float waitTime=3;// time to wait before correcting angle


    // normal public
    Vector3 destination;
    public int normalMinSleep=0; // min sleep time when stopped
    public int normalMaxSleep=5; // max sleep time when stopped
    public float timetoturn=1; // time to turn to direction
    // normal private
    int normalState=0; // 0: stop 1: move to a random location
    float normalSleepStart; // time when start sleeping
    float normalSleepDur; // sleep duration
    Quaternion lookdirection;
    Quaternion initalRotation;
    float initialTime;
    bool startedMoving;
    bool correctingAngle=false;
    float ttg;
    Text textElement;
    public float adelay=5;
    public float mind=2;
    float nextPT;

    void lookXZ(Vector3 target){
        Vector3 rot = Quaternion.LookRotation(target - transform.position).eulerAngles;
        rot.x=rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);
    }

    // functions
    override public void setup(){

        rb = GetComponent<Rigidbody>();

        ani = GetComponentInChildren<Animator>();
        colli = GetComponent<BoxCollider>();
        con=GetComponent<AudioSource>();
    }
    // normal
    void getActionNormal(float elp){
        if (UnityEngine.Random.Range(0,5)<=1){
            normalNavRandom(elp);
        }else{
            normalPause(elp);
        }
    }
    void normalNavRandom(float elp){

        ani.SetInteger("State", 1);
        normalState=1;
        destination=new Vector3(UnityEngine.Random.Range(-(width/2), (width/2)),0,UnityEngine.Random.Range(-(length/2), (length/2)));
        Vector3 direction = Vector3.Scale(destination - transform.position, new Vector3(1,0,1));
        lookdirection = Quaternion.LookRotation(direction);
        initalRotation = transform.rotation;
        initialTime = elp;
        startedMoving=false;
    }
    void normalPause(float elp){
        ani.SetInteger("State", 0);
        normalState=0;

        normalSleepDur=UnityEngine.Random.Range(normalMinSleep,normalMaxSleep);
        normalSleepStart=elp;
    }
    override public void normalChange(float elp){

        getActionNormal(elp);
        nextPT=elp+mind+UnityEngine.Random.Range(0,adelay);
    }
    override public void normalUpdate(float elp){
        if (con.isPlaying==false&&elp>nextPT){
            nextPT=elp+mind+UnityEngine.Random.Range(0,adelay);
            con.clip=normalClip[UnityEngine.Random.Range(0, normalClip.Count)];
            con.Play();
        }
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.x, 0))+Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, 0))>45 && correctingAngle==false){
            ani.SetInteger("State", 0);
            correctingAngle=true;
            ttg=elp;
        }
        if (correctingAngle && new Vector2(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.z).magnitude<5){
            correctingAngle=false;
        }
        else if (correctingAngle && elp>ttg+waitTime){
            transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,transform.rotation.eulerAngles.y,0),(elp-(ttg+waitTime))/2);
        }
        else if (correctingAngle){
        }
        else if (normalState==1){
            if (Mathf.Abs(transform.rotation.eulerAngles.y-lookdirection.eulerAngles.y)>1){
                ani.SetInteger("State", 0);
                Vector3 direction = Vector3.Scale(destination - transform.position, new Vector3(1,0,1));
                lookdirection = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation,lookdirection, rotateSpeed*Time.deltaTime);
                startedMoving=false;
            }else if (Mathf.Abs(transform.rotation.eulerAngles.y-lookdirection.eulerAngles.y)<=5 &&startedMoving==false){
                
                startedMoving=true;
            }
            else if ((destination-transform.position).magnitude < 0.3f){ // path completed

                if (UnityEngine.Random.Range(0,5)<=1){
                    getActionNormal(elp);
                }else{
                    normalPause(elp);
                }
            }else if (startedMoving){
                ani.SetInteger("State", 1);
                transform.position=Vector3.MoveTowards(transform.position,destination, escapeSpeed*Time.deltaTime);

            }
        }else{
            
            if (elp >= (normalSleepStart+normalSleepDur)){
                getActionNormal(elp);
            }else{
                if (rb.velocity.magnitude>0){
                    rb.velocity=rb.velocity/2;
                }
                if (rb.angularVelocity.magnitude>0){
                    rb.velocity=rb.velocity/2;
                }
            }
        }
    }




    // annoy
    // annoyed public
    public float annoyJumpDist=0.5f; // how far cat can jump
    public float annoyJumppause=3; // how long before cat jump
    public float annoyJumpVel=4; // velocity of jump
    public float timeToEscape=3;
    public float escapeWaitTime=3;
    public float LaunchAngle=75.0f;
    public float escapeTime=5f;
    public float escapeSpeed=0.2f;
    public AudioClip audios;
    public AudioClip jumpc;

    // annoyed variables
    float annoyPauseStart;
    int annoyState=0;// 0: normal; 1: escape
    Vector3 annoyTarget;
    Vector3 annoyInitialPos;
    float escapeInitial;
    bool launched=false;
    bool isEscaping;
    float timeStartEscape=0;
    bool startEscape=false;
    void jumpToPoint(Vector3 point){
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetXZPos = new Vector3(annoyTarget.x, transform.position.y, annoyTarget.z);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
        float H = annoyTarget.y - transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)) );
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rb.velocity = globalVelocity;
    }
    Vector3 getNearestEscape(){
        BoxCollider[] exit=GameObject.Find("Exit points").GetComponentsInChildren<BoxCollider>();
        List<Vector3> exitPoss=new List<Vector3>();
        for (int i =0; i< exit.Length;i++){
            Vector3 closestpoint=exit[i].bounds.ClosestPoint(transform.position);
            exitPoss.Add(new Vector3(closestpoint.x, escapePoints[0].transform.position.y, closestpoint.z));
        }
        int pcount =  0;
        Vector3[] pnum= new Vector3[10];
        for (int i=0; i<exitPoss.Count;i++){
            float dist=Vector3.Scale(exitPoss[i]-transform.position, new Vector3(1,0,1)).magnitude;
            if (dist<annoyJumpDist && dist>1){
                pnum[pcount]=exitPoss[i];
                pcount+=1;
            }
        }
        if (pcount == 0){
            return new Vector3(-1,-1,-1);
        }else{
            return pnum[UnityEngine.Random.Range(0,pcount)];
        }
    }
    override public void annoyChange(float elp){
        
        launched=false;
        // check whether in range
        annoyTarget=getNearestEscape();
        if (annoyTarget == new Vector3(-1,-1,-1)){
            annoyState=0;
            normalChange(elp);
        }else{
            ani.SetInteger("State", 0);
            con.Stop();
            con.clip=audios;
            con.Play();
            annoyState=1;
            lookXZ(annoyTarget);
            annoyInitialPos=transform.position;
            escapeInitial=elp+timeToEscape;
        }
    }
    override public void annoyUpdate(float elp){
        if (annoyState==0){
            normalUpdate(elp);
            annoyTarget=getNearestEscape();
            if (annoyTarget != new Vector3(-1,-1,-1)){
                con.Stop();
                con.clip=audios;
                con.Play();
                isEscaping=false;
                ani.SetInteger("State", 0);
                annoyState=1;
                lookXZ(annoyTarget);
                annoyInitialPos=transform.position;
                escapeInitial=elp+timeToEscape;
            }

        }else{
            if ( (elp-escapeInitial)>0){
                if (!launched){

                    jumpToPoint(annoyTarget);
                    con.Stop();
                    con.clip=jumpc;
                    con.Play();
                    launched=true;
                }else{
                    ani.SetInteger("State", 0);
                }
            }
            if (isEscaping){
                if (startEscape){
                    startEscape=false;
                    timeStartEscape=elp;
                }
                if ((elp-timeStartEscape)>escapeWaitTime){
                    ani.SetInteger("State", 1);
                    transform.position+=transform.forward*escapeSpeed*Time.deltaTime;
                }
                
            }
            // transform.position=Vector3.Lerp(annoyInitialPos, annoyTarget, (elp-escapeInitial)/escapeWaitTime);
        }
    }
    
    
    
    // grabbed
    bool letGo=false;
    bool touchingGround=true;
    
    override public void whenGrabbed(float elp){
        letGo=false;
        touchingGround=false;
    }
    override public void grabChange(float elp){
        letGo=false;
        touchingGround=false;
        oldTg=touchingGround;
    }
    bool oldTg=false;
    override public void grabUpdate(float elp){
    }
    override public void grabRelease(float elp){
        letGo=true;
    }
    // collision handling
    void OnCollisionEnter(Collision coll){
        if (launched==true ){
            if (coll.gameObject.tag=="floor"){
                base.changeState(0);
                launched=false;
                base.setAnnoytime();
            }
        }
        if (letGo){
            if(coll.gameObject.tag=="floor"){
                touchingGround=true;
                changeState(0);
            }
        }
    }
    public void OnTriggerEnter(Collider coll){
        if (launched==true && isEscaping==false){
            if (coll.gameObject.tag=="exit"){
                isEscaping=true;
                startEscape=true;
            }
        }
    }
}
