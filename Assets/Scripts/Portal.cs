using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Portal destination = null;
    
    public float stopTime = 0;
    [SerializeField] float dontTransportTime = 5f;
    [SerializeField] Vector3 transportOffset = new Vector3(0f, 1f, 0f);
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if (Time.time < stopTime) return;

        destination.stopTime = Time.time + dontTransportTime;



        if(destination != null)
        {
            other.transform.position = destination.transform.position + transportOffset;
            
        }
        
    }
    private void Update()
    {


    }

}
