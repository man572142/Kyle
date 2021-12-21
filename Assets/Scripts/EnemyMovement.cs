using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    bool isWalking = false;
    [SerializeField] float walkSpeed = 1f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] Rigidbody myRigibody = null;
    [SerializeField] bool noForwardRootmotion = false;
    bool isRunning = false;


    private void FixedUpdate()
    {
        if (!noForwardRootmotion) return;

        if (isWalking)
        {
            myRigibody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * walkSpeed);
        }
        else if(isRunning)
        {
            myRigibody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * runSpeed);
        }
        
    }

    public void Walk(bool walkState)
    {
        isWalking = walkState;
    }

    public void Running(bool runState)
    {
        isRunning = runState;
    }
}
