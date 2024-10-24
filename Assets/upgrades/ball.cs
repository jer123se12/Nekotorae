using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

struct catStuck
{
    public BoxCollider[] bcx;
    public Rigidbody[] rbs;
    public GameObject cat;
    public FixedJoint j;
    public float t;
    public catStuck(GameObject c, Rigidbody[] rb, BoxCollider[] b, FixedJoint joint, float tim)
    {
        this.bcx = b;
        this.rbs = rb;
        this.cat = c;
        this.t = tim;
        this.j = joint;
    }
}

struct ignoreCat
{
    public int uid;
    public float elp;
    public ignoreCat(int id, float e)
    {
        uid = id;
        elp = e;
    }
}
public class ball : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    bool isGrabbing = false;
    public float speed = 20;
    public float stop = 1f;
    List<catStuck> cs = new List<catStuck>();
    Vector3 lastPosition;
    Vector3 lastVelocity;
    Vector3 lastAngularVelocity;
    List<ignoreCat> toignorecats = new List<ignoreCat>();
    float elp = 0;
    public float stickTime = 5;
    bool stopSticking = false;

    void Start()
    {
        cs.Clear();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        elp += Time.deltaTime;
        lastPosition = transform.position;
        lastVelocity = rb.velocity;
        lastAngularVelocity = rb.angularVelocity;
        if (rb.velocity.magnitude < stop)
        {
            stopSticking = true;
        }
        stopSticking = false;
        List<int> indextopop = new List<int>();
        for (int i = 0; i < cs.Count; i++)
        {

            catStuck c = cs[i];
            if (elp > c.t + stickTime)
            {
                if (c.cat.transform.position.y<0){
                    c.cat.transform.position=new Vector3(c.cat.transform.position.x,0,c.cat.transform.position.z);
                }
                Destroy(c.j);

                indextopop.Add(i);
                
                c.cat.GetComponent<BoxCollider>().enabled = true;
                for (int j = 0; j < c.bcx.Length; j++)
                {
                    // print("I am enabling" + j + c.bcx[j].gameObject.name);
                    c.bcx[j].enabled = true;
                }
                if (c.cat.GetComponent<normalCat>().IsUnityNull() && c.cat.GetComponent<poopyCat>().IsUnityNull())
                {
                    for (int j = 0; j < c.rbs.Length; j++)
                    {

                        c.rbs[j].WakeUp();
                        c.rbs[j].velocity = -((transform.position - c.cat.transform.position).normalized) * 5;
                        c.rbs[j].angularVelocity = Vector3.zero;
                    }
                    c.cat.GetComponent<flyingCat>().released();
                }
                else
                {
                    for (int j = 0; j < c.rbs.Length; j++)
                    {

                        c.rbs[j].useGravity = true;
                        c.rbs[j].WakeUp();
                        c.rbs[j].velocity = -((transform.position - c.cat.transform.position).normalized) * 5;
                        c.rbs[j].angularVelocity = Vector3.zero;
                    }
                    if (c.cat.GetComponent<poopyCat>().IsUnityNull())
                    {
                        c.cat.GetComponent<normalCat>().released();
                    }
                    else
                    {
                        c.cat.GetComponent<poopyCat>().released();
                    }
                }
            }

        }
        for (int i = indextopop.Count() - 1; i > -1; i--)
        {
            toignorecats.Add(new ignoreCat(cs[i].cat.GetInstanceID(), elp));
            cs.RemoveAt(i);
            
        }
        for(int i=toignorecats.Count()-1; i>-1;i--){
            if (elp>toignorecats[i].elp+5){
                toignorecats.RemoveAt(i);
            }
        }


    }

    public void OnCollisionEnter(Collision coll)
    {


        if ((!isGrabbing && !stopSticking) && coll.gameObject.tag == "cat")
        {
            
            for (int i=0;i<toignorecats.Count();i++){
                if (toignorecats[i].uid==coll.gameObject.GetInstanceID()){
                    return;
                }
            }
            transform.position = lastPosition;
            rb.velocity = lastVelocity;
            rb.angularVelocity = lastAngularVelocity;
            Physics.IgnoreCollision(GetComponent<SphereCollider>(), coll.collider);

            FixedJoint j = GetComponentInParent<Transform>().gameObject.AddComponent<FixedJoint>();
            j.connectedBody = coll.gameObject.GetComponent<Rigidbody>();
            j.massScale = 0.01f;
            GameObject obj = coll.gameObject;
            BoxCollider bo = obj.GetComponent<BoxCollider>();
            if (obj.GetComponent<normalCat>().IsUnityNull()&&obj.GetComponent<poopyCat>().IsUnityNull())
            {
                obj.GetComponent<flyingCat>().grabbed();
            }
            else if (obj.GetComponent<poopyCat>().IsUnityNull())
            {
                obj.GetComponent<normalCat>().grabbed();
            }
            else
            {
                obj.GetComponent<poopyCat>().grabbed();
            }
            bo.enabled = false;
            Rigidbody[] rbs = obj.GetComponentsInChildren<Rigidbody>();
            BoxCollider[] bos = obj.GetComponentsInChildren<BoxCollider>();

            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].useGravity = false;
            }
            for (int i = 0; i < bos.Length; i++)
            {
                // print(bos[i].gameObject);
                bos[i].enabled = false;
            }

            cs.Add(new catStuck(obj,
                rbs,
                bos,
                j, elp));

        }
    }
    public void grab()
    {
        isGrabbing = true;
        // Commented out to allow the ball to be regrabbed with the cats still attached to it
        // FixedJoint[] joints = GetComponents<FixedJoint>();
        // if (joints.Length > 0)
        // {
        //     for (int i = 0; i < joints.Length; i++)
        //     {
        //         Destroy(joints[i]);
        //     }
        // }
        // for (int i = 0; i < cs.Count; i++)
        // {

        //     catStuck c = cs[i];


        //     c.cat.GetComponent<BoxCollider>().enabled = true;
        //     for (int j = 0; j < c.bcx.Length; j++)
        //     {
        //         c.bcx[i].enabled = true;
        //     }
        //     if (c.cat.GetComponent<normalCat>().IsUnityNull()&&c.cat.GetComponent<poopyCat>().IsUnityNull())
        //     {
        //         for (int j = 0; j < c.rbs.Length; j++)
        //         {

        //             c.rbs[j].WakeUp();
        //             c.rbs[j].velocity = Vector3.zero;
        //             c.rbs[j].angularVelocity = Vector3.zero;
        //         }
        //         c.cat.GetComponent<flyingCat>().released();
        //     }
        //     else
        //     {
        //         for (int j = 0; j < c.rbs.Length; j++)
        //         {

        //             c.rbs[j].useGravity = true;
        //             c.rbs[j].WakeUp();
        //             c.rbs[j].velocity = Vector3.zero;
        //             c.rbs[j].angularVelocity = Vector3.zero;
        //         }
        //         if (c.cat.GetComponent<poopyCat>().IsUnityNull())
        //         {
        //             c.cat.GetComponent<normalCat>().released();
        //         }
        //         else
        //         {
        //             c.cat.GetComponent<poopyCat>().released();
        //         }
        //     }

        // }
        // cs.Clear();

    }
    void grab2()
    {

    }
    public void release()
    {
        isGrabbing = false;
    }
}
