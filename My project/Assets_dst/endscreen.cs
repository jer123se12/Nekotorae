using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endscreen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject camerapos;
    public float distance;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position=camerapos.transform.position+(camerapos.transform.forward*distance);
        transform.rotation=camerapos.transform.rotation;
        
    }
}
