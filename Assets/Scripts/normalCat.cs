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

public class exit{
    public BoxCollider collider;
    public Transform Height;
}
public class normalCat : basecat
{
    // properties of cat
    protected BoxCollider colli;
    protected Rigidbody rb;
    protected AudioSource con;
    public List<AudioClip> normalClip;
    protected Animator ani;

    // outside variables
    public float width = 1.8f;
    public float length = 1.5f;
    public float rotateSpeed = 180;
    public float waitTime = 3;// time to wait before correcting angle


    // normal public
    protected Vector3 destination;
    public int normalMinSleep = 0; // min sleep time when stopped
    public int normalMaxSleep = 5; // max sleep time when stopped
    // normal private
    protected int normalState = 0; // 0: stop 1: move to a random location
    protected float normalSleepStart; // time when start sleeping
    protected float normalSleepDur; // sleep duration
    protected Quaternion lookdirection;
    protected Quaternion initalRotation;
    protected float initialTime;
    protected bool startedMoving;
    protected bool correctingAngle = false;
    protected float ttg;
    public float adelay = 5;
    public float mind = 2;
    protected float nextPT;
    public AudioClip thud;
    CatExpressionHandler handler;
    public float timeout=5;



    void lookXZ(Vector3 target)
    {
        Vector3 rot = Quaternion.LookRotation(target - transform.position).eulerAngles;
        rot.x = rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);
    }

    // functions
    override public void setup()
    {

        rb = GetComponent<Rigidbody>();

        ani = GetComponentInChildren<Animator>();
        colli = GetComponent<BoxCollider>();
        con = GetComponent<AudioSource>();
        handler = transform.Find("normalCat").Find("Cat Face2").GetComponent<CatExpressionHandler>();
    }
    // normal
    protected void getActionNormal(float elp)
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
    Ray check;
    Vector3 getDestination()
    {
        Vector3 d = new Vector3(UnityEngine.Random.Range(-(width / 2), (width / 2)), transform.position.y, UnityEngine.Random.Range(-(length / 2), (length / 2)));

        check = new Ray(transform.position, destination-transform.position);
        RaycastHit hitData;

        if (Physics.SphereCast(check.origin, 0.2f, check.direction, out hitData, (transform.position-d).magnitude,~(1<<8)))
        {
            if (hitData.point!=new Vector3(0,0,0)){
                d = hitData.point;
                d=d-((d-transform.position).normalized)*0.1f;
            }
            
        };

        return new Vector3(d.x,0,d.z);
    }
    void normalNavRandom(float elp)
    {

        ani.SetInteger("State", 1);
        normalState = 1;
        destination = getDestination();
        Vector3 direction = Vector3.Scale(destination - transform.position, new Vector3(1, 0, 1));
        lookdirection = Quaternion.LookRotation(direction);
        initalRotation = transform.rotation;
        initialTime = elp;
        startedMoving = false;
    }
    protected void normalPause(float elp)
    {
        ani.SetInteger("State", 0);
        normalState = 0;

        normalSleepDur = UnityEngine.Random.Range(normalMinSleep, normalMaxSleep);
        normalSleepStart = elp;
    }
    override public void normalChange(float elp)
    {

        getActionNormal(elp);
        nextPT = elp + mind + UnityEngine.Random.Range(0, adelay);
    }
    float nextBlink=0;
    protected float startmov=0;
    override public void normalUpdate(float elp)
    {
        //Debug.DrawLine(new Vector3(-(width / 2), (width / 2)), 0, UnityEngine.Random.Range(-(length / 2), (length / 2)))
        Debug.DrawRay(check.origin,check.direction);
        nextBlink+=Time.deltaTime;
        if (nextBlink>=5){
            nextBlink%=5;
            StartCoroutine(handler.blink());
            
        }
        if (con.isPlaying == false && elp > nextPT)
        {
            nextPT = elp + mind + UnityEngine.Random.Range(0, adelay);
            con.clip = normalClip[UnityEngine.Random.Range(0, normalClip.Count)];
            con.Play();
        }
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.x, 0)) + Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, 0)) > 45 && correctingAngle == false)
        {
            ani.SetInteger("State", 0);
            correctingAngle = true;
            ttg = elp;
        }
        if (correctingAngle && new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.z).magnitude < 5)
        {
            correctingAngle = false;
        }
        else if (correctingAngle && elp > ttg + waitTime)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), (elp - (ttg + waitTime)) / 2);
        }
        else if (correctingAngle)
        {

        }
        else if (normalState == 1)
        {
            if (Mathf.Abs(transform.rotation.eulerAngles.y - lookdirection.eulerAngles.y) > 1)
            {
                ani.SetInteger("State", 0);
                Vector3 direction = Vector3.Scale(destination - transform.position, new Vector3(1, 0, 1));
                lookdirection = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookdirection, rotateSpeed * Time.deltaTime);
                startedMoving = false;
            }
            else if (Mathf.Abs(transform.rotation.eulerAngles.y - lookdirection.eulerAngles.y) <= 5 && startedMoving == false)
            {
                startmov=elp;

                startedMoving = true;
            }
            else if ((destination - transform.position).magnitude < 0.1f || elp>startmov+timeout)
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
                Debug.DrawLine(transform.position, destination);
                ani.SetInteger("State", 1);
                transform.position = Vector3.MoveTowards(transform.position, destination, escapeSpeed * Time.deltaTime);

            }
        }
        else
        {

            if (elp >= (normalSleepStart + normalSleepDur))
            {
                getActionNormal(elp);
            }
            else
            {
                if (rb.velocity.magnitude > 0)
                {
                    rb.velocity = rb.velocity / 2;
                }
                if (rb.angularVelocity.magnitude > 0)
                {
                    rb.velocity = rb.velocity / 2;
                }
            }
        }
    }




    // annoy
    // annoyed public
    public float annoyJumpDist = 0.5f; // how far cat can jump
    public float timeToEscape = 3;
    public float escapeWaitTime = 3;
    public float LaunchAngle = 75.0f;
    public float escapeSpeed = 0.2f;
    public AudioClip audios;

    // annoyed variables
    int annoyState = 0;// 0: normal; 1: escape
    Vector3 annoyTarget;
    Vector3 annoyInitialPos;
    float escapeInitial;
    bool launched = false;
    bool isEscaping;
    bool movingToward;
    float timeStartEscape = 0;
    bool startEscape = false;
    void jumpToPoint(Vector3 point)
    {
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
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rb.velocity = globalVelocity;
    }
    Vector3 getNearestEscape()
    {

        List<Vector3> exitPoss = new List<Vector3>();
        for (int i = 0; i < exits.Count; i++)
        {
            if (exits[i].collider.gameObject.activeSelf){
                Vector3 closestpoint = exits[i].collider.bounds.ClosestPoint(transform.position);
                exitPoss.Add(new Vector3(closestpoint.x, exits[i].Height.position.y, closestpoint.z));
                print(exits[i].Height.position.y);
            }
        }
        int pcount = 0;
        Vector3[] pnum = new Vector3[10];
        for (int i = 0; i < exitPoss.Count; i++)
        {
            float dist = Vector3.Scale(exitPoss[i] - transform.position, new Vector3(1, 0, 1)).magnitude;
            if ( (dist < annoyJumpDist &&  dist > 0.5)||(dist<annoyJumpDist/2 && exitPoss[i].y<0.5))
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
        movingToward=false;
        launched = false;
        // check whether in range
        annoyTarget = getNearestEscape();
        print(annoyTarget);
        if (annoyTarget == new Vector3(-1, -1, -1))
        {
            annoyState = 0;
            normalChange(elp);
        }
        else
        {
            ani.SetInteger("State", 0);
            con.Stop();
            con.clip = audios;
            con.Play();
            annoyState = 1;
            lookXZ(annoyTarget);
            annoyInitialPos = transform.position;
            escapeInitial = elp + timeToEscape;
        }
    }
    override public void annoyUpdate(float elp)
    {
        if (annoyState == 0)
        {
            normalUpdate(elp);
            annoyTarget = getNearestEscape();
            if (annoyTarget != new Vector3(-1, -1, -1))
            {
                con.Stop();
                con.clip = audios;
                con.Play();
                isEscaping = false;
                ani.SetInteger("State", 0);
                annoyState = 1;
                lookXZ(annoyTarget);
                annoyInitialPos = transform.position;
                escapeInitial = elp + timeToEscape;
            }

        }
        else
        {
            if ((elp - escapeInitial) > 0)
            {
                print("not launched");
                print(launched);
                print(annoyTarget.y);
                if (annoyTarget.y>0.1 && !launched)
                {   
                    print("Launched");
                    con.Stop();
                    con.clip = audios;
                    con.Play();
                    jumpToPoint(annoyTarget);
                    launched = true;
                }
                else
                {
                    if(annoyTarget.y<0.1&&!movingToward){
                        movingToward=true;
                        con.Stop();
                        con.clip = audios;
                        con.Play();
                    }
                    if(annoyTarget.y<0.1){
                        
                        transform.position = Vector3.MoveTowards(transform.position, annoyTarget, escapeSpeed * Time.deltaTime);
                    }else{
                        ani.SetInteger("State", 0);
                    }
                }
            }
            if (isEscaping)
            {
                if (startEscape)
                {
                    startEscape = false;
                    timeStartEscape = elp;
                }
                if ((elp - timeStartEscape) > escapeWaitTime)
                {
                    ani.SetInteger("State", 1);
                    transform.position += transform.forward * escapeSpeed * Time.deltaTime;
                }

            }
            // transform.position=Vector3.Lerp(annoyInitialPos, annoyTarget, (elp-escapeInitial)/escapeWaitTime);
        }
    }


    public Vector3 getClosetExit(){

        Vector3 firstexit=exits[0].collider.bounds.ClosestPoint(transform.position);
        Vector3 exitpos=new Vector3(firstexit.x, exits[0].Height.position.y, firstexit.z);
        float closetdist=(exitpos-transform.position).magnitude;
        for (int i = 1; i < exits.Count; i++)
        {
            if (exits[i].collider.gameObject.activeSelf){
                Vector3 nearesttoexit = exits[i].collider.bounds.ClosestPoint(transform.position);
                Vector3 nearexit=new Vector3(nearesttoexit.x, exits[i].Height.position.y, nearesttoexit.z);
                float dist=(nearexit-transform.position).magnitude;
                if (dist<closetdist){
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
        closestExit=new Vector3(closestExit.x, transform.position.y, closestExit.z);
        //closestExit=(transform.position-closestExit).normalized*annoyJumpDist;
        movingToward=false;
        launched = false;
        lookXZ(closestExit);
    }
    public override void superAnnoyUpdate(float elp)
    {
        Debug.DrawLine(transform.position,closestExit);

        if (annoyState == 0)
        {
            ani.SetBool("walking", true);
            transform.position=Vector3.MoveTowards(transform.position, closestExit, (escapeSpeed*2)*Time.deltaTime);
            annoyTarget = getNearestEscape();
            if (annoyTarget != new Vector3(-1, -1, -1))
            {
                con.Stop();
                con.clip = audios;
                con.Play();
                isEscaping = false;
                ani.SetInteger("State", 0);
                annoyState = 1;
                lookXZ(annoyTarget);
                annoyInitialPos = transform.position;
                escapeInitial = elp + timeToEscape;
            }

        }
        else
        {
            if ((elp - escapeInitial) > 0)
            {
                if (annoyTarget.y>0.5 && !launched)
                {
                    con.Stop();
                    con.clip = audios;
                    con.Play();
                    jumpToPoint(annoyTarget);
                    launched = true;
                }
                else
                {
                    if(annoyTarget.y<0.5&&!movingToward){
                        movingToward=true;
                        con.Stop();
                        con.clip = audios;
                        con.Play();
                    }
                    if(annoyTarget.y<0.5){
                        transform.position = Vector3.MoveTowards(transform.position, annoyTarget, escapeSpeed * Time.deltaTime);
                    }else{
                        ani.SetInteger("State", 0);
                    }
                }
            }
            if (isEscaping)
            {
                if (startEscape)
                {
                    startEscape = false;
                    timeStartEscape = elp;
                }
                if ((elp - timeStartEscape) > escapeWaitTime)
                {
                    ani.SetInteger("State", 1);
                    transform.position += transform.forward * escapeSpeed * Time.deltaTime;
                }

            }
        }
    }



    // grabbed
    bool letGo = false;
    bool touchingGround = true;

    override public void whenGrabbed(float elp)
    {
        letGo = false;
        touchingGround = false;
    }
    override public void grabChange(float elp)
    {
        letGo = false;
        touchingGround = false;
        oldTg = touchingGround;
    }
    bool oldTg = false;
    override public void grabUpdate(float elp)
    {
        if (rb.velocity.magnitude > 4)
        {
            handler.setDizzy();
        } else if (rb.velocity.magnitude < 1)
        {
            handler.unsetDizzy();
        }
    }
    override public void grabRelease(float elp)
    {
        letGo = true;
    }
    // collision handling
    void OnCollisionEnter(Collision coll)
    {
        
        if (launched == true)
        {
            if (coll.gameObject.tag == "floor")
            {
                
                base.changeState(0);
                launched = false;
                annoylvl = 0;
            }
        }
        if (letGo)
        {
            if (coll.gameObject.tag == "floor")
            {
                con.Stop();
                con.clip = thud;
                con.Play();
                touchingGround = true;
                changeState(0);
            }
        }
    }
    public void OnTriggerEnter(Collider coll)
    {
        if (launched == true && isEscaping == false)
        {
            if (coll.gameObject.tag == "exit")
            {
                isEscaping = true;
                startEscape = true;
            }
        }
    }
}
