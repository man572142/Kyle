using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] Rigidbody myRigibody = null;
    [SerializeField] float trailSpeed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = Mathf.Abs(myRigibody.velocity.x) + Mathf.Abs(myRigibody.velocity.y) + Mathf.Abs(myRigibody.velocity.z);

        if(speed > trailSpeed)
        {
            GetComponentInChildren<TrailRenderer>().enabled = true;
        }
        else
        {
            GetComponentInChildren<TrailRenderer>().enabled = false;
        }
    }
}
