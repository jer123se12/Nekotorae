using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

struct catStuck{
    public BoxCollider[] bcx;
    public Rigidbody[] rbs;
    public GameObject cat;
    public FixedJoint j;
    public catStuck(GameObject c, Rigidbody[] rb, BoxCollider[] b, FixedJoint joint){
        this.bcx=b;
        this.rbs=rb;
        this.cat=c;
        this.j=joint;
    }
}
public class ball : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    bool isGrabbing=false;
    public float speed=20; 
    public float stop=1f;
    List<catStuck> cs = new List<catStuck>();
    Vector3 lastPosition ;
    Vector3 lastVelocity ;
    Vector3 lastAngularVelocity ;

    void Start()
    {
        cs.Clear();
        rb=GetComponent<Rigidbody>();
    }

    void FixedUpdate(){
        lastPosition = transform.position;
        lastVelocity = rb.velocity;
        lastAngularVelocity = rb.angularVelocity;
        if (rb.velocity.magnitude<stop){
            rb.useGravity=true;
            grab();
        }else{
            // rb.useGravity=false;
        }
    }

    public void OnCollisionEnter(Collision coll){
        
        
        if (!isGrabbing&&coll.gameObject.tag=="cat"){
            transform.position = lastPosition;
            rb.velocity = lastVelocity;
            rb.angularVelocity = lastAngularVelocity;
            Physics.IgnoreCollision(GetComponent<SphereCollider>(), coll.collider);
            
            FixedJoint j=GetComponentInParent<Transform>().gameObject.AddComponent<FixedJoint>();
            j.connectedBody=coll.gameObject.GetComponent<Rigidbody>();
            j.massScale=0.01f;
            GameObject obj=coll.gameObject;
            BoxCollider bo=obj.GetComponent<BoxCollider>();
            if (obj.GetComponent<normalCat>().IsUnityNull()){
                obj.GetComponent<flyingCat>().grabbed();
            }else if(obj.GetComponent<poopyCat>().IsUnityNull()){
                obj.GetComponent<normalCat>().grabbed();
            }else{
                obj.GetComponent<poopyCat>().grabbed();
            }
            bo.enabled=false;
            Rigidbody[] rbs = obj.GetComponentsInChildren<Rigidbody>();
            BoxCollider[] bos = obj.GetComponentsInChildren<BoxCollider>();
            cs.Add(new catStuck(obj, 
                rbs, 
                bos,
                j));
            
            for (int i=0;i < rbs.Length; i++){
                rbs[i].useGravity=false;
            }
            for (int i=0;i < bos.Length; i++){
                bos[i].enabled=false;
            }
        }
    }
    public void grab(){
        isGrabbing=true;
        FixedJoint[] joints = GetComponents<FixedJoint>();
        if (joints.Length>0){
            for (int i=0;i<joints.Length;i++){
                Destroy(joints[i]);
            }
        }
        for (int i =0; i<cs.Count;i++){
            
            catStuck c=cs[i];
            
            
            c.cat.GetComponent<BoxCollider>().enabled=true;
            for (int j=0;j < c.bcx.Length; j++){
                    c.bcx[i].enabled=true;
            }
            if (c.cat.GetComponent<normalCat>().IsUnityNull()){
                for (int j=0;j < c.rbs.Length; j++){
                    print("woke one");
                    c.rbs[j].WakeUp();
                    c.rbs[j].velocity=Vector3.zero;
                    c.rbs[j].angularVelocity=Vector3.zero;
                }
                c.cat.GetComponent<flyingCat>().released();
            }else{
                for (int j=0;j < c.rbs.Length; j++){
                    print("woke one");
                    c.rbs[j].useGravity=true;
                    c.rbs[j].WakeUp();
                    c.rbs[j].velocity=Vector3.zero;
                    c.rbs[j].angularVelocity=Vector3.zero;
                }
                if(c.cat.GetComponent<poopyCat>().IsUnityNull()){
                    c.cat.GetComponent<normalCat>().released();
                }else{
                    c.cat.GetComponent<poopyCat>().released();
                }
            }
            
        }
        cs.Clear();

    }
    void grab2(){

    }
    public void release(){
        isGrabbing=false;
    }
}
