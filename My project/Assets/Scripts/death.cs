using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class death : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject lights;
    public GameObject endScreen;
    Light lit;
    bool catEscaped=false;
    public float speedDec=.01f;
    float prevt;
    void Start()
    {
        prevt=Time.realtimeSinceStartup;
        lit=lights.GetComponent<Light>();
        endScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float ct=Time.realtimeSinceStartup;
        float dt=Time.realtimeSinceStartup-prevt;
        if(catEscaped==true){
                
                lit.intensity-=speedDec*dt;

        }
        prevt=ct;
        if (lit.intensity<=0){
            endScreen.SetActive(true);
        }
        
        

    }
    public void OnTriggerEnter(Collider coll){
        print(coll.gameObject.name);
        if (coll.gameObject.tag=="cat"){
            // cat escaped
            coll.gameObject.layer=7;

            var children = coll.gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
    //            Debug.Log(child.name);
                child.gameObject.layer = 7;
            }
            lights.GetComponent<Light>().cullingMask=~(1<<LayerMask.NameToLayer("death"));
            GameObject newl=Instantiate(lights);
            newl.GetComponent<Light>().cullingMask=(1<<LayerMask.NameToLayer("death"));
            catEscaped=true;
            Time.timeScale=0;

        }
    }
}
