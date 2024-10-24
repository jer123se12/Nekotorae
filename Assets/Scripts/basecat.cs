using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class basecat : MonoBehaviour
{


    // functions
    public virtual void setup() { }
    public virtual void normalChange(float elp) { }
    public virtual void normalUpdate(float elp) { }
    public virtual void whenGrabbed(float elp) { }
    public virtual void annoyChange(float elp) { }
    public virtual void annoyUpdate(float elp) { }
    public virtual void superAnnoyChange(float elp) { }
    public virtual void superAnnoyUpdate(float elp) { }
    public virtual void grabChange(float elp) { }
    public virtual void grabUpdate(float elp) { }
    public virtual void grabRelease(float elp) { }

    // variables
    int state = 0; // stores state 0=normal 1=annoyed 2=grabbed
    float elp = 0; // time since spawn
    public float annoylvl = 0; // lvl of annoyed (0-100)
    public float maxStartlvl = 20f;
    public float minAPerSec=0;
    public float maxAPerSec=5;
    public int howOftenToCheck = 1;
    public float annoyTresh=30;
    public float SAnnoyTresh=80;

    int prevState = 0; // previous state used to check if state has changed

    // public variables

    public List<exit> exits=new List<exit>();
    ParticleSystem alert;




    void Start()
    {

        exits=new List<exit>();

        BoxCollider[] Exits=GameObject.Find("Exit points").GetComponentsInChildren<BoxCollider>(includeInactive:true);
        for (int i=0; i<Exits.Length; i++){
            exit e=new exit();

            e.Height=Exits[i].gameObject.GetNamedChild("point").transform;
            e.collider=Exits[i];
            exits.Add(e);
        }





        setup();

        // setup first state
        if (state == 0)
        {
            normalChange(0);
        }
        else
        {
            annoyChange(0);
        }
        alert = GetComponentInChildren<ParticleSystem>();
        alert.Stop();
        annoylvl=UnityEngine.Random.value*maxStartlvl;
    }

    public void changeState(int s)
    {
        state = s;
        alert.Stop();
        switch (state)
        {
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

    float everysec = 0;
    void FixedUpdate()
    {
        everysec += Time.deltaTime;
        
        prevState = state;
        elp += Time.deltaTime;

        if (state!=2 && everysec >= 1)
        {
            annoylvl+=Mathf.Pow(UnityEngine.Random.Range(minAPerSec, maxAPerSec),2)/2f;
            everysec %= 1;
            if (annoylvl>SAnnoyTresh)
            {
                alert.Play();
                state=3;
            }else if (annoylvl>annoyTresh){
                state = 1;
                // 
                
                // annoyChange(elp);
            }
        }
        if (state == 0)
        {
            normalUpdate(elp);
        }
        else if (state == 1)
        {
            annoyUpdate(elp);
        }
        else if(state==2)
        {
            grabUpdate(elp);
        }else{
            superAnnoyUpdate(elp);
        }
        if (prevState != state)
        { // state has changed
            switch (state)
            {
                case 0:
                    normalChange(elp);
                    break;
                case 1:
                    annoyChange(elp);
                    break;
                case 2:
                    grabChange(elp);
                    break;
                case 3:
                    superAnnoyChange(elp);
                    break;
                default:
                    break;
            }
        }

    }

    public void grabbed()
    {
        grabChange(elp);
        state = 2;
    }
    void OnCollisionEnter(Collision collision)
    {

    }
    public void released()
    {
        grabRelease(elp);
        if (state==0){
            annoylvl*=0.8f;
        }else{
            annoylvl=0;
        }

    }
}
