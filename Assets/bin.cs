using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bin : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject rejected;
    public float aUp=70;

    public float speed=10;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void shoot(GameObject obj){
        float a=UnityEngine.Random.Range(0,360);
        Vector3 force=(Quaternion.AngleAxis(a, Vector3.up)*(Quaternion.AngleAxis(aUp, Vector3.left)*new Vector3(speed,0,0)));
        obj.GetComponent<Rigidbody>().velocity=force;
    }
    public void OnTriggerEnter(Collider coll){
        if (coll.tag=="poop"){
            Destroy(coll.gameObject);
        }
        else{
            
        }
    }
}
