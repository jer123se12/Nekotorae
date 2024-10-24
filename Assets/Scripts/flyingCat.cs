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
    public float width = 1.8f;
    public float length = 1.5f;
    public float height = 2f;

    // normal public
    Vector3 destination;
    public List<AudioClip> normalClip;
    public int normalMinSleep = 0; // min sleep time when stopped
    public int normalMaxSleep = 5; // max sleep time when stopped
    public float turnSpeed = 100; // time to turn to direction
    // normal private
    int normalState = 0; // 0: stop 1: move to a random location
    float normalSleepStart; // time when start sleeping
    float normalSleepDur; // sleep duration
    Quaternion lookdirection;
    Quaternion initalRotation;
    float initialTime;
    bool startedMoving;

    bool correctingAngle = false;
    float ttg;
    public float waitTime = 3;
    public float adelay = 5;
    public float mind = 2;
    float nextPT;

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    // functions
    override public void setup()
    {
        rb = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
        colli = GetComponent<BoxCollider>();
        con = GetComponent<AudioSource>();
    }
    // normal
    void getActionNormal(float elp)
    {
        if (UnityEngine.Random.Range(0, 5) <= 1)
        {
            normalNavRandom(elp);
        }
        else
        {
            normalPause(elp);
        }
    }
    Vector3 getDestination()
    {
        
        Vector3 d = new Vector3(
        UnityEngine.Random.Range(-(width / 2), (width / 2)),
        UnityEngine.Random.Range(0, height),
        UnityEngine.Random.Range(-(length / 2), (length / 2))
        );
        Vector3 pred=d;
    
        

        Ray check = new Ray(transform.position, d-transform.position);
        RaycastHit hitData;

        if (Physics.Raycast(check, out hitData, (transform.position-d).magnitude, ~8))
        {
            Debug.DrawRay(d,transform.position);
            d = hitData.point;
            d+=(transform.position-d).normalized*0.1f;
        };
        
        
        

        return d;
    }
    void normalNavRandom(float elp)
    {
        ani.SetBool("walking", true);
        normalState = 1;
        destination = getDestination();
        

        initalRotation = transform.rotation;
        initialTime = elp;
        startedMoving = false;

    }
    void resetrb()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Rigidbody[] rbs = transform.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].velocity = Vector3.zero;
            rbs[i].angularVelocity = Vector3.zero;
        }
    }
    void normalPause(float elp)
    {
        ani.SetBool("walking", false);
        normalState = 0;
        rb.velocity = Vector3.zero;
        normalSleepDur = UnityEngine.Random.Range(normalMinSleep, normalMaxSleep);
        normalSleepStart = elp;
    }
    override public void normalChange(float elp)
    {

        getActionNormal(elp);
        nextPT = elp + mind + UnityEngine.Random.Range(0, adelay);
    }
    override public void normalUpdate(float elp)
    {
        
        if (elp > nextPT)
        {
            nextPT = elp + mind + UnityEngine.Random.Range(0, adelay);
            con.Stop();
            con.clip =
            normalClip[UnityEngine.Random.Range(0, normalClip.Count)];
            con.Play();
        }
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.x, 0)) + Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, 0)) > 5 && correctingAngle == false && !startedMoving && !correctingAngle) 
        {

            resetrb();
            correctingAngle = true;
            ttg = elp;
        }
        if (correctingAngle && new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.z).magnitude < 5)
        {
            resetrb();
            correctingAngle = false;
        }
        else if (correctingAngle && elp > ttg + waitTime)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), (elp - (ttg + waitTime)) / 2);
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, (elp - (ttg + waitTime)) / 2);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, (elp - (ttg + waitTime)) / 2); ;
        }
        else if (correctingAngle)
        {
        }
        if (normalState == 1)
        {
            Vector3 direction = destination - transform.position;
            direction = new Vector3(direction.x, 0, direction.z);
            lookdirection = Quaternion.LookRotation(direction);
            if (Mathf.Abs((transform.rotation.eulerAngles - lookdirection.eulerAngles).magnitude) > 1)
            {


                ani.SetBool("walking", false);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookdirection, (elp - initialTime) * turnSpeed);
            }
            else if (Mathf.Abs((transform.rotation.eulerAngles - lookdirection.eulerAngles).magnitude) <= 1 && startedMoving == false)
            {
                startedMoving = true;
            }
            if (startedMoving && (destination - transform.position).magnitude < 0.2f)
            { // path completed
                if (UnityEngine.Random.Range(0, 5) <= 1)
                {
                    getActionNormal(elp);
                }
                else
                {
                    normalPause(elp);
                }
            }
            else if (startedMoving)
            {
                Debug.DrawLine(transform.position,destination);
                ani.SetBool("walking", true);
                transform.position = Vector3.MoveTowards(transform.position, destination, escapeSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (elp >= (normalSleepStart + normalSleepDur))
            {
                getActionNormal(elp);
            }
        }
    }




    // annoy
    // annoyed public
    public float annoyJumpDist = 0.5f; // how far cat can jump
    public float annoyJumppause = 3; // how long before cat jump
    public float annoyJumpVel = 4; // velocity of jump
    public float timeToEscape = 3;
    public float escapeWaitTime = 3;
    public float LaunchAngle = 75.0f;
    public float escapeTime = 5f;
    public float escapeSpeed = 0.2f;
    public AudioClip audios;


    // annoyed variables
    float annoyPauseStart;
    int annoyState = 0;// 0: normal; 1: escape
    Vector3 annoyTarget;
    Vector3 annoyInitialPos;
    float escapeInitial;
    bool launched = false;
    bool isEscaping;
    float timeStartEscape = 0;
    bool startEscape = false;
    Vector3 getNearestEscape()
    {

        List<Vector3> exitPoss = new List<Vector3>();
        for (int i = 0; i < exits.Count; i++)
        {
            if(exits[i].collider.gameObject.activeSelf){
                exitPoss.Add(exits[i].collider.bounds.ClosestPoint(transform.position));
            }
        }

        int pcount = 0;
        Vector3[] pnum = new Vector3[10];
        for (int i = 0; i < exitPoss.Count; i++)
        {
            if (Vector3.Scale(exitPoss[i] - transform.position, new Vector3(1, 0, 1)).magnitude < annoyJumpDist && exitPoss[i].y>0.2)
            {
                pnum[pcount] = exitPoss[i];
                pcount += 1;
            }
        }
        if (pcount == 0)
        {
            return new Vector3(-1, -1, -1);
        }
        else
        {
            return pnum[UnityEngine.Random.Range(0, pcount)];
        }
    }
    override public void annoyChange(float elp)
    {
        launched = false;
        // check whether in range
        annoyTarget = getNearestEscape();

        if (annoyTarget == new Vector3(-1, -1, -1))
        {
            annoyState = 0;
            normalChange(elp);
        }
        else
        {
            ani.SetBool("walking", false);
            con.Stop();
            con.clip = audios;
            con.Play();
            annoyState = 1;
            transform.LookAt(new Vector3(annoyTarget.x, transform.position.y, annoyTarget.z));
            annoyInitialPos = transform.position;
            escapeInitial = elp + timeToEscape;
        }

    }
    override public void annoyUpdate(float elp)
    {
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.x, 0)) + Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, 0)) > 5 && correctingAngle == false && !startedMoving && !correctingAngle) 
        {

            resetrb();
            correctingAngle = true;
            ttg = elp;
        }
        if (correctingAngle && new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.z).magnitude < 5)
        {
            resetrb();
            correctingAngle = false;
        }
        else if (correctingAngle && elp > ttg + waitTime)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), (elp - (ttg + waitTime)) / 2);
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, (elp - (ttg + waitTime)) / 2);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, (elp - (ttg + waitTime)) / 2); ;
        }
        else if (correctingAngle)
        {
        }
        if (annoyState == 0)
        {
            normalUpdate(elp);

            annoyTarget = getNearestEscape();
            if (annoyTarget != new Vector3(-1, -1, -1))
            {
                isEscaping = false;
                ani.SetBool("walking", false);
                con.Stop();
                con.clip = audios;
                con.Play();
                annoyState = 1;
                transform.LookAt(new Vector3(annoyTarget.x, transform.position.y, annoyTarget.z));
                annoyInitialPos = transform.position;
                escapeInitial = elp + timeToEscape;
            }

        }
        else
        {

            if (!isEscaping && (elp - escapeInitial) > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, annoyTarget, escapeSpeed * Time.deltaTime);
                if (!isEscaping && (annoyTarget - transform.position).magnitude < 0.1f)
                {
                    isEscaping = true;
                    startEscape = true;
                }
            }

            if (isEscaping)
            {
                if (startEscape)
                {
                    startEscape = false;
                    timeStartEscape = elp;
                }
                if (elp - timeStartEscape > escapeTime)
                {
                    ani.SetBool("walking", true);
                    transform.position += transform.forward * escapeSpeed * Time.deltaTime;
                }

            }
            // transform.position=Vector3.Lerp(annoyInitialPos, annoyTarget, (elp-escapeInitial)/escapeWaitTime);
        }
    }
    // superannoyed
    void lookXZ(Vector3 target)
    {
        Vector3 rot = Quaternion.LookRotation(target - transform.position).eulerAngles;
        rot.x = rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);
    }
    Vector3 ori;
    public Vector3 getClosetExit(){

        Vector3 exitpos=exits[0].collider.bounds.ClosestPoint(transform.position);
        ori=exitpos;
        float closetdist=(exitpos-transform.position).magnitude;
        for (int i = 1; i < exits.Count; i++)
        {
            if(exits[0].collider.gameObject.activeSelf){
                Vector3 nearexit = exits[i].collider.bounds.ClosestPoint(transform.position);
                float dist=(nearexit-transform.position).magnitude;
                if (dist<closetdist && nearexit.y>0.5){
                    exitpos=nearexit;
                    closetdist=dist;
                }
            }
            
        }
        return exitpos;
    }
    Vector3 closestExit;
    // Super Annoyed
    public override void superAnnoyChange(float elp)
    {
        annoyState = 0;
        print("super annoyed");
        closestExit=getClosetExit();
        
        //closestExit=(transform.position-closestExit).normalized*annoyJumpDist;
        launched = false;
        lookXZ(closestExit);
        isEscaping=false;
        
    }
    public override void superAnnoyUpdate(float elp)
    {
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.x, 0)) + Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, 0)) > 5 && correctingAngle == false && !startedMoving && !correctingAngle) 
        {

            resetrb();
            correctingAngle = true;
            ttg = elp;
        }
        if (correctingAngle && new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.z).magnitude < 5)
        {
            resetrb();
            correctingAngle = false;
        }
        else if (correctingAngle && elp > ttg + waitTime)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), (elp - (ttg + waitTime)) / 2);
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, (elp - (ttg + waitTime)) / 2);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, (elp - (ttg + waitTime)) / 2); ;
        }
        else if (correctingAngle)
        {
        }
        else {
            if((transform.position-closestExit).magnitude>0.1 && !isEscaping){
                ani.SetBool("walking",true);
                transform.position=Vector3.MoveTowards(transform.position, closestExit, (escapeSpeed*2)*Time.deltaTime);
            }else if (!isEscaping){
                isEscaping=true;
                startEscape=true;
            }
            if (isEscaping)
            {
                if (startEscape)
                {
                    ani.SetBool("walking",false);
                    startEscape = false;
                    timeStartEscape = elp;
                }
                if (elp - timeStartEscape > escapeTime)
                {
                    ani.SetBool("walking", true);
                    transform.position += transform.forward * escapeSpeed * Time.deltaTime;
                }

            }
        }
    }


    // grabbed
    bool letGo = false;
    override public void whenGrabbed(float elp) { }
    override public void grabChange(float elp)
    {
    }
    override public void grabUpdate(float elp) { }
    override public void grabRelease(float elp)
    {
        changeState(0);
    }



    // collision handling
    void OnCollisionEnter(Collision coll)
    {

    }
}
