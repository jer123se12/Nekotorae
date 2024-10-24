using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headGrab : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void grab(){
        GetComponentInParent<CapsuleCollider>().enabled=false;
    }
    void release(){
        GetComponentInParent<CapsuleCollider>().enabled=true;
    }
}
