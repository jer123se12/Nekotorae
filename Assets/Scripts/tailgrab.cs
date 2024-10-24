using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class tailgrab : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject parent;
    Animator ani;
    void Start()
    {
        parent = transform.parent.gameObject;
        ani = parent.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void grab()
    {
        // transform.parent.GetComponent<BoxCollider>().enabled=false;
        ani.SetInteger("State", 2);
        if (parent.GetComponent<normalCat>().IsUnityNull())
        {

            parent.GetComponent<poopyCat>().grabbed();
        }
        else
        {
            parent.GetComponent<normalCat>().grabbed();
        }
    }
    public void release()
    {
        // this.GetComponentInParent<BoxCollider>().enabled=true;
        ani.SetInteger("State", 0);
        if (parent.GetComponent<normalCat>().IsUnityNull())
        {
            parent.GetComponent<poopyCat>().released();
        }
        else
        {
            parent.GetComponent<normalCat>().released();
        }
    }
}
