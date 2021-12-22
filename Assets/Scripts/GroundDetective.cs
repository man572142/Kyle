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
                //�p�G�W�L1���شN���i�ӳo�̵��A����collider�]���}�a����isGrounded = false�i�J�H�Uif()
                if(!isGrounded)
                {
                    playerController.SetAirAnimate(true);   //����
                    isFalling = true;
                }
                
            }
            else
            {
                if (isFalling) //�p�G�b���a1���سB�ɬO���b���������A,�N��AirAnimate�]��false�ǳƵۦa
                {
                    playerController.SetAirAnimate(false);
                    isFalling = false;
                    isGrounded = true;
                }
                else
                {
                    //Debug.Log("isGrounded");
                    isGrounded = true;
                    myRigidbody.AddForce(Vector3.down * groundDistance, ForceMode.VelocityChange);  //�u�����a���W�L1���خɷ|�I�[                                                                              
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
