using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class flyingCat : basecat
{
    // properties of cat
    BoxCollider colli;
    Rigidbody rb;
    MeshRenderer rend;
    Animator ani;
    AudioSource con; 

    // outside variables
    public float width=1.8f;
    public float length=1.5f;
    public float height=2f;

    // normal public
    Vector3 destination;
    public List<AudioClip> normalClip;
    public int normalMinSleep=0; // min sleep time when stopped
    public int normalMaxSleep=5; // max sleep time when stopped
    public float turnSpeed=100; // time to turn to direction
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
    public float waitTime=3;
    public float adelay=5;
    public float mind=2;
    float nextPT;

    public static Vector3 RandomPointInBounds(Bounds bounds) {
    return new Vector3(
        UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
        UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
        UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
    );
}

    // functions
    override public void setup(){
        rb = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
        colli = GetComponent<BoxCollider>();
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

        ani.SetBool("walking", true);
        normalState=1;
        destination=new Vector3(
            UnityEngine.Random.Range(-(width/2), (width/2)),
            UnityEngine.Random.Range(0, height),
            UnityEngine.Random.Range(-(length/2), (length/2)));
        
        initalRotation = transform.rotation;
        initialTime = elp;
        startedMoving=false;
    }
    void resetrb(){
        rb.velocity=Vector3.zero;
        rb.angularVelocity=Vector3.zero;
        Rigidbody[] rbs=transform.GetComponentsInChildren<Rigidbody>();
        for (int i=0;i< rbs.Length;i++){
            rbs[i].velocity=Vector3.zero;
            rbs[i].angularVelocity=Vector3.zero;
        }
    }
    void normalPause(float elp){
        ani.SetBool("walking", false);
        normalState=0;
        rb.velocity=Vector3.zero;
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
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.x, 0))+Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, 0))>5 && correctingAngle==false){
            resetrb();
            correctingAngle=true;
            ttg=elp;
            print("correcting angle");
        }
        if (correctingAngle && new Vector2(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.z).magnitude<5){
            resetrb();
            correctingAngle=false;
        }
        else if (correctingAngle && elp>ttg+waitTime){
            transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,transform.rotation.eulerAngles.y,0),(elp-(ttg+waitTime))/2);
            rb.velocity=Vector3.Lerp(rb.velocity,Vector3.zero, (elp-(ttg+waitTime))/2);
            rb.angularVelocity=Vector3.Lerp(rb.angularVelocity,Vector3.zero, (elp-(ttg+waitTime))/2);;
        }
        else if (correctingAngle){
        }
        if (normalState==1){
            Vector3 direction = destination - transform.position;
            direction=new Vector3(direction.x,0,direction.z);
            lookdirection = Quaternion.LookRotation( direction );
            if (Mathf.Abs((transform.rotation.eulerAngles-lookdirection.eulerAngles).magnitude)>1){
                

                ani.SetBool("walking",false);
                transform.rotation = Quaternion.RotateTowards(transform.rotation,lookdirection, (elp-initialTime)/turnSpeed);
            }else if (Mathf.Abs((transform.rotation.eulerAngles-lookdirection.eulerAngles).magnitude)<=1 && startedMoving==false){
                startedMoving=true;
            }
            if (startedMoving && (destination-transform.position).magnitude<0.5f){ // path completed
                if (UnityEngine.Random.Range(0,5)<=1){
                    getActionNormal(elp);
                }else{
                    normalPause(elp);
                }
            }else if (startedMoving){
                ani.SetBool("walking",true);
                transform.position = Vector3.MoveTowards(transform.position,destination,escapeSpeed*Time.deltaTime);
            }
        }else{
            if (elp >= (normalSleepStart+normalSleepDur)){
                getActionNormal(elp);
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
    Vector3 getNearestEscape(){
        BoxCollider[] exit=GameObject.Find("Exit points").GetComponentsInChildren<BoxCollider>();
        List<Vector3> exitPoss=new List<Vector3>();
        for (int i =0; i< exit.Length;i++){
            exitPoss.Add(exit[i].bounds.ClosestPoint(transform.position));
        }

        int pcount =  0;
        Vector3[] pnum= new Vector3[10];
        for (int i=0; i<exitPoss.Count;i++){
            if (Vector3.Scale(exitPoss[i]-transform.position, new Vector3(1,0,1)).magnitude<annoyJumpDist){
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
            ani.SetBool("walking",false);
            con.Stop();
            con.clip=audios;
            con.Play();
            annoyState=1;
            transform.LookAt(new Vector3(annoyTarget.x, transform.position.y, annoyTarget.z));
            annoyInitialPos=transform.position;
            escapeInitial=elp+timeToEscape;
        }
        
    }
    override public void annoyUpdate(float elp){
        if (annoyState==0){
            normalUpdate(elp);

            annoyTarget=getNearestEscape();
            if (annoyTarget != new Vector3(-1,-1,-1)){
                isEscaping=false;
                ani.SetBool("walking",false);
                con.Stop();
                con.clip=audios;
                con.Play();
                annoyState=1;
                transform.LookAt(new Vector3(annoyTarget.x, transform.position.y, annoyTarget.z));
                annoyInitialPos=transform.position;
                escapeInitial=elp+timeToEscape;
            }

        }else{
            
            if (!isEscaping && (elp-escapeInitial)>0){
                transform.position=Vector3.MoveTowards(transform.position,annoyTarget,escapeSpeed*Time.deltaTime);
                if(!isEscaping && (annoyTarget-transform.position).magnitude<0.1f){
                    isEscaping=true;
                    startEscape=true;
                }
            }
            
            if (isEscaping){
                if (startEscape){
                    startEscape=false;
                    timeStartEscape=elp;
                }
                if (elp-timeStartEscape>escapeTime){
                    ani.SetBool("walking",true);
                    transform.position+=transform.forward*escapeSpeed*Time.deltaTime;
                }
                
            }
            // transform.position=Vector3.Lerp(annoyInitialPos, annoyTarget, (elp-escapeInitial)/escapeWaitTime);
        }
    }
    
    
    
    // grabbed
    bool letGo=false;
    override public void whenGrabbed(float elp){}
    override public void grabChange(float elp){
    }
    override public void grabUpdate(float elp){}
    override public void grabRelease(float elp){
        changeState(0);
    }



    // collision handling
    void OnCollisionEnter(Collision coll){
        
    }
}
