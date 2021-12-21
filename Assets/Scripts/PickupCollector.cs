using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCollector : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Pickup")
        {
            Pickup pickup = other.GetComponent<Pickup>();
            pickup.FlyToTarget(this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
