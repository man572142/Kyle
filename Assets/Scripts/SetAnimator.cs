using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimator : MonoBehaviour
{
    [SerializeField] Animator animator = null;

    public void OpenDoor()
    {
        animator.SetBool("Open", true);
    }

    public void LoadDoor()
    {
        animator.SetBool("LoadDoor", true);
    }

    public void LoadBridge()
    {
        animator.SetBool("LoadBridge", true);
    }

    public void PlaySFX()
    {
        SoundManager.instance.Play(SoundType.CoolBubble, transform.position);
    }

}
