using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class poop : MonoBehaviour
{
    // Start is called before the first frame update
    public float selfadd = 0.01f;
    void Start()
    {

    }
    float next = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        next += Time.deltaTime;
        if (next > 1)
        {
            next %= 1;
            GameObject[] allc = GameObject.FindGameObjectsWithTag("cat");
            for (int i = 0; i < allc.Count(); i++)
            {
                if (!allc[i].GetComponent<basecat>().IsUnityNull())
                {

                    allc[i].GetComponent<basecat>().annoylvl += selfadd * Time.deltaTime;
                }
            }
        }
    }
}
