using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class door : MonoBehaviour
{
    public bool isopen=false;
    public bool isopening=false;
    Quaternion targetRotation=Quaternion.Euler(-90,90,0);
    public float speed=100;
    public GameObject colli;
    AudioSource aud;
    // Start is called before the first frame update
    void Start()
    {
        colli.SetActive(false);
        aud=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isopening){
            transform.rotation=Quaternion.RotateTowards(transform.rotation, targetRotation,20*Time.deltaTime);
        }
        if(Mathf.Abs(90-transform.rotation.eulerAngles.y)<3&&isopen!=true){
            isopen=true;
            colli.SetActive(true);
            aud.Stop();
        }
    }
    public void open(){
        isopening=true;
        aud.Play();
    }
}
