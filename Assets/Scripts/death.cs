using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;


public class originalL{
    public float initialIntensity;
    public Light light;
}
public class death : MonoBehaviour
{
    // Start is called before the first frame update
    Light[] lights;
    List<originalL> originL=new List<originalL>();
    List<Light> badLight= new List<Light>();
    public GameObject endScreen;
    public GameObject left;
    public GameObject right;
    bool catEscaped=false;
    public float speedDec=.01f;
    float prevt;
    bool gameStarted;
    public GameObject normalcat;
    GameObject theFirstCat;
    normalCat theFirstNormal;
    float timeGameStarted;
    XRGrabInteractable[] grabbers;
    public endpanel endp;
    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);
            
        }
    }
    void Start()
    {
        lights=FindObjectsOfType<Light>();
        prevt=Time.realtimeSinceStartup;
        endScreen.SetActive(true);
        left.SetActive(true);
        right.SetActive(true);
        gameStarted=false;
    
        for (int i=0;i<lights.Length;i++){
            lights[i].GetComponent<Light>().cullingMask=~(1<<LayerMask.NameToLayer("death"));
            GameObject newl=Instantiate(lights[i].gameObject);
            newl.GetComponent<Light>().cullingMask=(1<<LayerMask.NameToLayer("death"));
            badLight.Add(newl.GetComponent<Light>());
        }
        for(int i=0; i<lights.Length; i++){
            originalL l=new originalL();
            l.initialIntensity=lights[i].intensity;
            l.light=lights[i];
            lights[i].intensity=0;
            originL.Add(l);
        }
        theFirstCat=GameObject.Instantiate(normalcat, new Vector3(0,0.7f,0), Quaternion.identity);
        grabbers=theFirstCat.GetComponentsInChildren<XRGrabInteractable>();
        theFirstNormal=theFirstCat.GetComponent<normalCat>();
        theFirstNormal.enabled=false;
        theFirstCat.GetComponent<Rigidbody>().isKinematic=true;
        SetGameLayerRecursive(theFirstCat,LayerMask.NameToLayer("death"));
        
        
    }
    void gameUpdate(){
        float ct=Time.realtimeSinceStartup;
        float dt=Time.realtimeSinceStartup-prevt;
        if(catEscaped==true){
            for(int i=0; i<lights.Length; i++){
            lights[i].intensity-=speedDec*dt;
            }
        }
        prevt=ct;
        if (lights[0].intensity<=0){
            PlayerPrefs.SetFloat("finalScore", Time.timeSinceLevelLoad-timeGameStarted);
            Time.timeScale=1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    bool hasBeenGrabbed=false;
    void preGameUpdate(){
        if (!hasBeenGrabbed){
            for (int i=0;i<grabbers.Length;i++){
                if (grabbers[i].isSelected){
                    hasBeenGrabbed=true;
                    theFirstNormal.enabled=true;
                    theFirstNormal.grabbed();
                    theFirstCat.GetComponent<Rigidbody>().isKinematic=false;
                    if (PlayerPrefs.HasKey("finalScore")){
                        endp.save();
                    }
                    endScreen.SetActive(false);
                }
            }
        }else{
            for (int i=0; i<originL.Count;i++){
                if (originL[i].light.intensity<originL[i].initialIntensity){
                    originL[i].light.intensity+=(originL[i].initialIntensity/0.5f)*Time.deltaTime;
                }
                
            }
            
            if (originL[0].initialIntensity<=originL[0].light.intensity){
                
                gameStarted=true;
                timeGameStarted=Time.timeSinceLevelLoad;
                left.SetActive(false);
                right.SetActive(false);

                SetGameLayerRecursive(theFirstCat,LayerMask.NameToLayer("cats"));
                FindAnyObjectByType<controller>().GetComponent<controller>().TheFirstCat=true;
            }
        }
    }
    float elp=0;
    // Update is called once per frame
    void Update()
    {
        elp+=Time.deltaTime;
        if (gameStarted){
            gameUpdate();
        }else{
            preGameUpdate();
        }
        

    }
    public void OnTriggerEnter(Collider coll){
        if (coll.gameObject.tag=="cat"){
            // cat escaped
            coll.gameObject.layer=7;

            var children = coll.gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
    //            Debug.Log(child.name);
                child.gameObject.layer = 7;
            }
            
            catEscaped=true;
            Time.timeScale=0;

        }
    }
}
