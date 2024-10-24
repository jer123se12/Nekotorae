using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;
using UnityEngine.UIElements;
public enum cats{
    normal,
    poopy,
    flying
}
[System.Serializable]
public class catTypes{
    public cats cat;
    public int cost;
}
[System.Serializable]
public class wave{
    public int numNormal;
    public int numFlying;
    public int numPoopy;
    public int timeToLast;
}
public class catLoc{
    public GameObject cat;
    public Vector3 loc;
    public catLoc(GameObject c, Vector3 l){
        cat=c;
        loc=l;
    }
}
public class controller : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject nc;
    public GameObject pc;
    public GameObject fc;
    public GameObject Vent;
    public List<catTypes> cattypes=new List<catTypes>();
    public GameObject VentCollider;
    public float ventOpenTime=1;
    public float doorOpenTime=100;
    public float width = 2.5f;
    public float len = 2.5f;
    public float hei = 2f;
    public float safeSpace = 1;
    public List<wave> waves= new List<wave>();
    public float catdelay = 0.1f;
    public float timemultiplier = 1.1f;
    public GameObject ps;
    public GameObject tdoor;
    public bool TheFirstCat;
    Queue<catLoc> spawnQueue=new Queue<catLoc>();
    TextMeshPro text;
    AudioSource ventSoundComponent;
    int numNormal=0;
    int numPoopy=0;
    int numFlying=0;
    float elp;
    bool ventopen=false;
    bool dooropen=false;
    float timeSinceLastWave;
    float timeToNextWave;
    int totalSpawned=0;


    void Start()
    {
        text = GameObject.Find("clock").GetComponent<TextMeshPro>();
        VentCollider.SetActive(false);
        // catTypes normalc=new catTypes();
        // normalc.cat=cats.normal;
        // normalc.cost=1;
        // catTypes flyingc=new catTypes();
        // flyingc.cat=cats.flying;
        // flyingc.cost=2;
        // catTypes poopyc=new catTypes();
        // poopyc.cat=cats.poopy;
        // poopyc.cost=3;
        // cattypes.Add(normalc);
        // cattypes.Add(flyingc);
        // cattypes.Add(poopyc);
        generateWave();
    }
    void generateWave(){
        if(waves.Count>0){
            wave w=waves[0];
            waves.RemoveAt(0);
            for (int i=0; i<w.numNormal;i++){
                snc(cats.normal);
            }
            for (int i=0; i<w.numFlying;i++){
                snc(cats.flying);
            }
            for (int i=0; i<w.numPoopy;i++){
                snc(cats.poopy);
            }
            timeToNextWave=w.timeToLast;
        }else{
            spawnRandomCat();
            timeToNextWave=60;
        }
    }
    void snc(cats c){
        GameObject cat;
        int n=0;
        if(c==cats.normal){
            cat=nc;
            n=0;
        }else if (c==cats.poopy){
            cat=pc;
            n=0;
        }else{
            cat=fc;
            n=2;
        }
        Vector3 pos=getpos(n);
        if(pos!=new Vector3(-1,-1,-1)){
            spawncat(pos, cat);
        }
        
    }
    void spawncat(Vector3 pos, GameObject cat)
    {
        spawnQueue.Enqueue(new catLoc(cat,pos));
        
    }
    void asp(Vector3 pos, GameObject cat){
        totalSpawned+=1;
        ps.transform.position = pos;
        ps.GetComponent<ParticleSystem>().Play();
        ps.GetComponent<AudioSource>().Play();
        Instantiate(cat, pos, Quaternion.identity);
    }
    Vector3 getpos(int c){
        //GameObject[] cats = GameObject.FindGameObjectsWithTag("cat");
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-(width / 2), (width / 2)), (c == 0) ? 0 : UnityEngine.Random.Range(0, hei), UnityEngine.Random.Range(-(len / 2), (len / 2))); ;
            int count = 0;
            while (true && count < 100)
            {
                count += 1;
                bool clear = true;
                pos = new Vector3(UnityEngine.Random.Range(-(width / 2), (width / 2)), (c == 0) ? 0 : UnityEngine.Random.Range(0, hei), UnityEngine.Random.Range(-(len / 2), (len / 2)));
                Ray r = new Ray(new Vector3(0,0,0) , pos);
                Debug.DrawLine(new Vector3(0,0,0), pos);


                // for (int i = 0; i < cats.Length; i++)
                // {
                //     if (Mathf.Abs((cats[i].transform.position - pos).magnitude) < safeSpace)
                //     {
                //         clear = false;
                //     }
                // }
                if (clear && !Physics.Raycast(r,pos.magnitude))
                {
                    break;
                }
            }
            if (count > 100)
            {
                return new Vector3(-1,-1,-1);
            }
            return pos;
    }
    void spawnRandomCat(){
        
            GameObject ctbs;
            int c;
            if (UnityEngine.Random.value > 0.33)
            {
                if(UnityEngine.Random.value > 0.5){
                    ctbs=nc;
                    numNormal+=1;
                }else{
                    ctbs=pc;
                    numPoopy+=1;
                }
                
                c = 0;
                
            }
            else
            {
                ctbs = fc;
                numFlying+=1;
                c = 1;
            }
            Vector3 pos=getpos(c);
            if(pos!=new Vector3(-1,-1,-1)){
                spawncat(pos, ctbs);
            }
    }
    void openVent(){
        ventSoundComponent = Vent.GetComponent<AudioSource>();
        ventSoundComponent.enabled = true;
        Rigidbody vrb=Vent.GetComponent<Rigidbody>();
        vrb.isKinematic=false;
        vrb.AddForce(Vector3.left*30);
        VentCollider.SetActive(true);
        print("Vent has been opened");
    }
    void openDoor(){
        tdoor.GetComponent<door>().open();
    }
    void preGameUpdate(){

    }
    void FixedUpdate(){
        if (TheFirstCat){
            gameUpdate();
        }else{
            preGameUpdate();
        }
    }
    
    
    // Update is called once per frame
    void gameUpdate()
    {
        if (spawnQueue.Count>0 && !ps.GetComponent<ParticleSystem>().isPlaying){
            catLoc ca=spawnQueue.Dequeue();
            asp(ca.loc,ca.cat);
        }
        elp+=Time.deltaTime;
        timeSinceLastWave+=Time.deltaTime;
        if (timeSinceLastWave>timeToNextWave){
            timeSinceLastWave=0;
            generateWave();
            
        }
        if (!ventopen && elp>ventOpenTime){
            ventopen=true;
            openVent();
        }
        if(!dooropen && elp>doorOpenTime){
            dooropen=true;
            openDoor();
        }
        text.text =
            (Mathf.Floor(elp / 60)).ToString().PadLeft(2, '0')
                + ":" +
                (Mathf.Floor(elp % 60)).ToString().PadLeft(2, '0');

    }
    

}
