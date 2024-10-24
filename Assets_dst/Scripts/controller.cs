using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class controller : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject nc;
    public GameObject pc;
    public GameObject fc;
    public float width=2.5f;
    public float len=2.5f;
    public float hei=2f;
    public float safeSpace=1;
    float elapsed = 0f;
    public float catdelay=1;
    public float timemultiplier=2;
    public GameObject ps;
    TextMeshPro text;
    
    void Start()
    {
        text=GameObject.Find("clock").GetComponent<TextMeshPro>();
    }
    void spawncat(Vector3 pos, GameObject cat){
        ps.transform.position=pos;
        ps.GetComponent<ParticleSystem>().Play();
        Instantiate(cat,pos, Quaternion.identity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsed+=Time.deltaTime;
        if (elapsed >= catdelay) {
            elapsed=elapsed%catdelay;
            catdelay*=timemultiplier;
            // determine position
            GameObject[] cats=GameObject.FindGameObjectsWithTag("cat");
            GameObject ctbs;
            int c;
            if(UnityEngine.Random.value>0.33){
                ctbs=(UnityEngine.Random.value>0.5)?nc:pc;
                c=0;
            }else{
                ctbs=fc;
                c=1;
            }
            Vector3 pos;
            while (true){
                bool clear=true;
                pos= new Vector3(UnityEngine.Random.Range(-(width/2),(width/2)),(c==0)?0:UnityEngine.Random.Range (0,hei),UnityEngine.Random.Range (-(len/2),(len/2)));
                for (int i=0; i<cats.Length; i++){
                    if (Mathf.Abs((cats[i].transform.position-pos).magnitude)<safeSpace){
                        clear=false;
                    }
                }
                if (clear){
                    break;
                }
            }
            spawncat(pos,ctbs);
            }else{

        }
        text.text= 
            (Mathf.Floor(Time.timeSinceLevelLoad/60)).ToString().PadLeft(2,'0')
                +":"+
                (Mathf.Floor(Time.timeSinceLevelLoad%60)).ToString().PadLeft(2,'0');
        
    }
    void restart(){
        Time.timeScale=1;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    } 
    
}
