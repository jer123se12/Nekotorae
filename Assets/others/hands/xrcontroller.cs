using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;




public class XRHandController : MonoBehaviour
{

    
    public InputActionProperty grab;
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        ani=GetComponent<Animator>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHand();
    }

    

    void AnimateHand()
    {
        float tv=grab.action.ReadValue<float>();
        ani.SetFloat("grab",tv);
    }
}