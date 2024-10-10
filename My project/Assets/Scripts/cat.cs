using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class cat : MonoBehaviour
{
    private int state=0;
    public Rigidbody rb;
    // Start is called before the first frame update
    private float direction=0;
    private Vector3 target;
    public float width=1.8f;
    public float length=1.5f;
    public float speed;
    float elapsed = 0f;
    private bool normalMovement;
    float elp=0;
    private float normalSleep;
    private float normalSleepStart;
    private Vector3 velocity;

    public NavMeshAgent agen;

    public GameObject[] escapePoints;
    
    public LayerMask mask;
    public float jumpDist=0.3f;
    public float jumpSpeed=2f;

    float anger=0;
    public float angerTresh=30;

    public float maxangerstep=1f;
    bool acceptnewstate=true;

    Vector3 targetEscapePoint;

    bool goingToEscape=false;
    Vector3 initialpos;
    float initialtime;
    MeshRenderer rend;
    bool aboveground=false;
    SphereCollider colli;
    void Start()
    {
        rend=GetComponent<MeshRenderer>();
        changestate(state);
        agen.areaMask=mask;
        colli=GetComponent<SphereCollider>();
    }
    void getNewActionEscape(){
        agen.enabled=false;
        // find closes escape point
        int pcount =  0;
        Vector3[] pnum= new Vector3[10];
        for (int i=0; i<escapePoints.Length;i++){
            print(Vector3.Scale(escapePoints[i].transform.position-transform.position, new Vector3(1,0,1)).magnitude);
            if (Vector3.Scale(escapePoints[i].transform.position-transform.position, new Vector3(1,0,1)).magnitude<jumpDist){
                pnum[pcount]=escapePoints[i].transform.position;
                pcount+=1;
            }
        }
        if (pcount==0){
            print("none in radius");
            getNewActionNormal();
            state=0;
            return;
        }


        targetEscapePoint=pnum[UnityEngine.Random.Range(0,pcount)];

        goingToEscape=true;
        transform.LookAt(targetEscapePoint);
        initialpos=transform.position;
        new WaitForSeconds(3);
        initialtime=elapsed+3;
        


       
        


    }
    
    void changestate(int state){
        

        if (state==1){
            getNewActionEscape();
        }else{
            // select a target
            getNewActionNormal();
        }
    }
    void pauseforawhile(int offset=0){
            rb.velocity=Vector3.zero;
            normalMovement=false;
            normalSleep=offset+UnityEngine.Random.Range(1,3);
            velocity=new Vector3(0, 0, 0);
            normalSleepStart=elapsed;

    }
    void getNewActionNormal(){
        agen.enabled=true;
        if (UnityEngine.Random.Range(0,5)<=2){
            normalMovement=true;
            target=new Vector3(UnityEngine.Random.Range(-(width/2), (width/2)),0,UnityEngine.Random.Range(-(length/2), (length/2)));
            agen.destination=target;

        }else{
            pauseforawhile();
            
        }
    }

    void swapState(){
        if (UnityEngine.Random.value<(Mathf.Min(anger/angerTresh,1)*0.8)){
                if (state==0 && anger>angerTresh){
                    state=1;
                    changestate(state);
                    print("state changed");
                }
                else if (state==1){
                    state=0;
                    changestate(state);
                }
            }
            else{
                state=0;
                getNewActionNormal();
            }
    }
    // Update is called once per frame
    void Update()
    {
        
        
        elapsed+=Time.deltaTime;
        elp+=Time.deltaTime;
        // if (elp >= 1f) {
        //     elp = elp % 1f;
            
            
        //     print(angerTresh);
            
        // }
        if (state==0 ) {
            anger+=UnityEngine.Random.value*maxangerstep*Time.deltaTime;
        float percentage=Mathf.Min(anger/angerTresh,1);
        rend.material.color=new Color(
            Mathf.Lerp(0,1,percentage),
            Mathf.Lerp(1,0,percentage),
            Mathf.Lerp(1,0,percentage));
            if(normalMovement==true && (!agen.pathPending && agen.remainingDistance < 0.05f)){
                agen.destination=transform.position;
                pauseforawhile();
                swapState();

            
            }
            else if (normalMovement==false && (elapsed>=(normalSleepStart+normalSleep))){
                state=0;
                agen.enabled=true;
                swapState();
                

            }
            
        }else if (state==1){
            if (goingToEscape==true && (elapsed>initialtime)){
                
                transform.position=Vector3.Lerp(initialpos, targetEscapePoint, (elapsed-initialtime)/5);
            }
        }else{
             if (normalMovement==false && (elapsed>=(normalSleepStart+normalSleep))){
                state=0;
                agen.enabled=true;
                swapState();
                

            }
        }

    }

    public void grabbed(){
        state=2;
        goingToEscape=false;
        anger=0;

    }
    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("floor") && aboveground){


            

            pauseforawhile();
        }
    }
    public void released(){
        
        aboveground=true;
        agen.enabled=false;
        
    }
}
