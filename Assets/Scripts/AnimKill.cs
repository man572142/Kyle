using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimKill : MonoBehaviour
{
    /// <summary>接受動畫刪除事件</summary>
    public void Kill()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
