using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetective : MonoBehaviour
{
    [SerializeField] PlayerController playerController = null;
    [SerializeField] CapsuleCollider capsuleCollider = null;
    [SerializeField] Rigidbody myRigidbody = null;
    [SerializeField] LayerMask groundLayer = default;
    [SerializeField] float fallingHeight = 1f;
    bool isGrounded = true;
    bool isFalling = false;
    //float lastTimeInGround = Mathf.Infinity;



    private void FixedUpdate()
    {
        RaycastHit groundHit;
        Vector3 center = transform.position + capsuleCollider.height / 2 * Vector3.up;
        
        if(Physics.Raycast(center, Vector3.down, out groundHit, 100f, groundLayer))
        {
            Debug.DrawRay(center, Vector3.down, Color.red, 2f);
            float groundDistance = Vector3.Distance(transform.position, groundHit.point);

            if(groundDistance > fallingHeight)
            {
                //如果超過1公尺就先進來這裡等，直到collider也離開地面後isGrounded = false進入以下if()
                if(!isGrounded)
                {
                    playerController.SetAirAnimate(true);   //掉落
                    isFalling = true;
                }
                
            }
            else
            {
                if (isFalling) //如果在離地1公尺處時是正在掉落的狀態,就把AirAnimate設為false準備著地
                {
                    playerController.SetAirAnimate(false);
                    isFalling = false;
                    isGrounded = true;
                }
                else
                {
                    //Debug.Log("isGrounded");
                    isGrounded = true;
                    myRigidbody.AddForce(Vector3.down * groundDistance, ForceMode.VelocityChange);  //只有離地不超過1公尺時會施加                                                                              
                }
            }

        }
    }


    private void OnTriggerExit(Collider other)  
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }
}
