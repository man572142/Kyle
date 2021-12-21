using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookCamaera : MonoBehaviour
{
    Transform myCam = null;
    void Start()
    {
        myCam = GameObject.Find("Main Camera").transform;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(myCam.position);
    }
}
