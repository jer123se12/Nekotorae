using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RatToy : MonoBehaviour
{
    bool isReleased;
    public float durationActive = 10;
    public float movementSpeed = 1;
    float timeRemaining;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        onRelease();
        rb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        // When released, if colliding into wall, turn a random direction and continue. Given timer
        if (timeRemaining < 0)
        {
            // Reset state
            isReleased = false;
            rb.useGravity = true;
            rb.freezeRotation = false;
            
        } else if (isReleased == true) {
            timeRemaining -= Time.deltaTime;
            // Check collider before moving
            transform.position += transform.forward * Time.deltaTime * movementSpeed;
        }
    }

    public void onRelease()
    {
        isReleased = true;
        rb.useGravity = false;
        timeRemaining = durationActive;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void onGrabbed()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "cat")
        {
            //transform.Rotate(0, 180f + UnityEngine.Random.Range(-30f,30f), 0);
            
             float angle = Vector3.Angle(Vector3.Reflect(transform.forward, -(collision.contacts[0].normal)), transform.forward);
             transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles+new Vector3(0,angle-30+UnityEngine.Random.Range(0,60),0));

		    // Debug.Log("AngleTest1: " + angle);
            Debug.Log("The player has collided with the wall!");
        }
    }

}
