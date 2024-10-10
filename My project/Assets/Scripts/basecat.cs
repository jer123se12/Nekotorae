using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class basecat : MonoBehaviour
{
    
    
    // functions
    public virtual void setup(){}
    public virtual void normalChange(float elp){}
    public virtual void normalUpdate(float elp){}
    public virtual void whenGrabbed(float elp){}
    public virtual void annoyChange(float elp){}
    public virtual void annoyUpdate(float elp){}
    public virtual void grabChange(float elp){}
    public virtual void grabUpdate(float elp){}
    public virtual void grabRelease(float elp){}
    
    // variables
    int state=0; // stores state 0=normal 1=annoyed 2=grabbed
    float elp=0; // time since spawn
    float annoytime; // time before annoyed
    int prevState=0; // previous state used to check if state has changed

    // public variables
    public List<GameObject> escapePoints; // points where cat can escape to
    public float annoyAvg=10; // avg time in seconds to anger
    public float annoyDev=5; // how much time can deviate for annoyance to trigger
    public audioCon auds;

    ParticleSystem alert;


     
    public void setAnnoytime(){
        annoytime = elp+(annoyAvg-(annoyDev/2))+UnityEngine.Random.Range(0f,annoyDev);
    }
    void Start()
    {
        escapePoints=GameObject.FindGameObjectsWithTag("ExitPoint").ToList<GameObject>();
        auds=GetComponent<audioCon>();

        setAnnoytime();
        setup();

        // setup first state
        if (state==0){
            normalChange(0);
        }else{
            annoyChange(0);
        }
        alert=GetComponentInChildren<ParticleSystem>();
        alert.Stop();
    }

    public void changeState(int s){
        state=s;
        alert.Stop();
        switch (state){
            case 0:
                normalChange(elp);
                break;
            case 1:
                annoyChange(elp);
                break;
            case 2:
                grabChange(elp);
                break;
            default:
                break;
        }
    }
    
    void FixedUpdate()
    {
        prevState=state;
        elp+=Time.deltaTime;
        if (state==0 && elp > annoytime){
            alert.Play();
            state=1;
            annoyChange(elp);
        }
        if (state==0){
            normalUpdate(elp);
        }else if (state==1){
            annoyUpdate(elp);
        }else{
            grabUpdate(elp);
        }
        if (prevState!=state){ // state has changed
            auds.changeState(state);
            switch (state){
                case 0:
                    normalChange(elp);
                    break;
                case 1:
                    annoyChange(elp);
                    break;
                case 2:
                    grabChange(elp);
                    break;
                default:
                    break;
            }
        }
        
    }

    public void grabbed(){
        grabChange(elp);
        state=2;
    }
    void OnCollisionEnter(Collision collision){
        
    }
    public void released(){
        grabRelease(elp);
        setAnnoytime();

    }
}
