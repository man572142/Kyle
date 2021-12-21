using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    bool isFly = false;
    Transform target = null;
    [SerializeField] float speed = 1f;
    float progress = 0f;
    public void FlyToTarget(Transform player)
    {
        target = player;
        isFly = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFly)
        {
            progress += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(transform.position, target.position, progress);
        }
    }
}
