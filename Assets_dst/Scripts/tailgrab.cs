using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tailgrab : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject parent;
    void Start()
    {
        parent=transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void grab(){
        // transform.parent.GetComponent<BoxCollider>().enabled=false;
        
        parent.GetComponent<normalCat>().grabbed();
    }
    public void release(){
        print("released tail");
        // this.GetComponentInParent<BoxCollider>().enabled=true;
        parent.GetComponent<normalCat>().released();
    }
}
