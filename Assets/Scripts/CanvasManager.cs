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
        if (PlayerPrefs.GetString("SAVE", "") == "")  //如果沒有紀錄就播9814
        {
            powerUI.SetActive(false);
            energyUI.SetActive(false);
            itemBoxUI.SetActive(false);

            waitAnimator.SetTrigger("9814Fade");
            yield return null;  // trigger動畫要到下一次刷新才開始
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
