using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] GameObject powerUI = null;
    [SerializeField] GameObject energyUI = null;
    [SerializeField] GameObject itemBoxUI = null;
    [SerializeField] Animator waitAnimator = null;

    IEnumerator Start()
    {
        if (PlayerPrefs.GetString("SAVE", "") == "")  //�p�G�S�������N��9814
        {
            powerUI.SetActive(false);
            energyUI.SetActive(false);
            itemBoxUI.SetActive(false);

            waitAnimator.SetTrigger("9814Fade");
            yield return null;  // trigger�ʵe�n��U�@����s�~�}�l
            AnimatorStateInfo animatorState = waitAnimator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(animatorState.length);

            powerUI.SetActive(true);
            energyUI.SetActive(true);
            itemBoxUI.SetActive(true);
        }
        else
        {
            waitAnimator.SetTrigger("NormalFade");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
