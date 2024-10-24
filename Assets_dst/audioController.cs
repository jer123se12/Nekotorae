using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioCon : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource con;
    public List<AudioClip> normalClip;
    public List<AudioClip> annoyClip;
    public int state=0;
    public float delay=5;
    public float mind=6;
    float nextPT;
    float elp=0;
    void Start()
    {
        con=GetComponent<AudioSource>();
        nextPT=elp+mind+Random.Range(0,delay);
    }

    // Update is called once per frame
    void Update()
    {
        elp+=Time.deltaTime;
        if (con.isPlaying==false&&elp>nextPT){
            nextPT=elp+mind+Random.Range(0,delay);
            if (state==0){
                con.clip=normalClip[Random.Range(0, normalClip.Count)];
            }else{
                con.clip=annoyClip[Random.Range(0, annoyClip.Count)];
            }
            con.Play();
        }
    }
    public void changeState(int st){
        nextPT=elp;
        state=st;
        con.Stop();
    }
}
