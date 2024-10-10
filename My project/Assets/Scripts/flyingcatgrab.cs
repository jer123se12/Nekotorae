using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class flygrab : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject parent;
    GameObject self;
    void Start()
    {
        parent=transform.parent.gameObject;
        self=this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void grab(){
        // transform.parent.GetComponent<BoxCollider>().enabled=false;
        parent.GetComponent<flyingCat>().grabbed();

    }
    public void release(){
        print("released tail");
        // this.GetComponentInParent<BoxCollider>().enabled=true;
        parent.GetComponent<flyingCat>().released();
    }
}
