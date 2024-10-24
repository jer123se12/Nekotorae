using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPoopyCat : normalCat
{
    float nextpooptime;
    public float minPoopTime = 5;
    public float maxPoopTime = 10;
    float nextBlink = 0;
    public Transform pooplocation;
    public GameObject poop;
    public AudioClip fartAudioClip;
    CatExpressionHandler handler;

    override public void setup()
    {
        rb = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
        colli = GetComponent<BoxCollider>();
        con = GetComponent<AudioSource>();
        handler = transform.Find("PoopyCatV1.1").Find("Poopy Face").GetComponent<CatExpressionHandler>();
    }

    override public void normalChange(float elp)
    {

        getActionNormal(elp);
        nextPT = elp + mind + Random.Range(0, adelay);
        nextpooptime = elp + Random.Range(minPoopTime, maxPoopTime);
    }

    override public void normalUpdate(float elp)
    {
        nextBlink += Time.deltaTime;
        if (nextBlink >= 5)
        {
            nextBlink %= 5;
            StartCoroutine(handler.blink());
        }

        if (con.isPlaying == false && elp > nextPT)
        {
            nextPT = elp + mind + Random.Range(0, adelay);
            con.clip = normalClip[Random.Range(0, normalClip.Count)];
            con.Play();
        }
        if (elp > nextpooptime)
        {
            // It is POOPY TIME
            nextpooptime = elp + Random.Range(minPoopTime, maxPoopTime);
            if (!con.isPlaying && (Random.Range(1, 4) == 1))
            {
                con.clip = fartAudioClip;
                con.Play();
            }
            GameObject poo = Instantiate(poop, pooplocation.position, pooplocation.rotation);

            poo.transform.localScale = new Vector3(Random.Range(0.5f, 1.5f) * 15, Random.Range(0.5f, 1.5f) * 15, Random.Range(0.5f, 1.5f) * 15);
            poo.GetComponent<Rigidbody>().velocity = (-poo.transform.forward);
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
                startmov = elp;

                startedMoving = true;
            }
            else if ((destination - transform.position).magnitude < 0.1f || elp > initialTime + (2 * timeout))
            { // path completed


                if (Random.Range(0, 5) <= 1)
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
                ani.SetInteger("State", 1);
                Debug.DrawLine(transform.position, destination);
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
}
