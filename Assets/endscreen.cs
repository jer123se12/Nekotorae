using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

//Time.timeSinceLevelLoad
public class endscreen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject camerapos;
    public GameObject finalscore;
    public GameObject HighScore;
    public float distance;
    public float finalScore;
    GameObject tobespawned;
    void Start()
    {

        if (!PlayerPrefs.HasKey("finalScore")){
            finalScore=-1;
            tobespawned=HighScore;
        }else{
            finalScore=PlayerPrefs.GetFloat("finalScore");
            tobespawned=finalscore;
            finalscore.GetComponent<endpanel>().fs=finalScore;
        }

        //GetComponentInChildren<endpanel>().fs=finalScore;
        tobespawned.SetActive(true);
        transform.position=new Vector3(0,1.3f,1);
        
    }

    // Update is called once per frame
    void Update()
    {
       
        
        
    }
}
