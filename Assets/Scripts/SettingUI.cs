using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup = null;
    [SerializeField] Animator animator = null;
    [SerializeField] Animator sceneAnimator = null;
    [SerializeField] float pauseTimeScale = 0f;
    bool _isOpen = false;
    bool isFadeOut = false;
    [SerializeField] float fadeOutSpeed = 3.5f;
    [SerializeField] AudioMixerSnapshot menuSnapshot = null;
    [SerializeField] AudioMixerSnapshot defaultSnapshot = null;


    bool isOpen
    {
        get { return _isOpen; }
        set
        {
            _isOpen = value;
            if(value)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                Cursor.lockState = CursorLockMode.None;
                menuSnapshot.TransitionTo(0.1f);
                Time.timeScale = pauseTimeScale;
                SoundManager.instance.Play(SoundType.SettingUI, transform.position);
                

            }
            else
            {
                isFadeOut = true;
                canvasGroup.blocksRaycasts = false;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
                defaultSnapshot.TransitionTo(0.1f);
                
                
            }
        }
    }

    private void UIFadeOut()
    {
        canvasGroup.alpha = 0f;
    }

    void Start()
    {
        canvasGroup.alpha = 0f;
        isOpen = isOpen;  //強迫執行一次set
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = !isOpen;  //狀態反轉,也就是switch開關的意思
            animator.SetBool("Open",isOpen);
        }


        if(isFadeOut)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * fadeOutSpeed);
            if(canvasGroup.alpha < 0.2f)
            {
                isFadeOut = false;
                canvasGroup.alpha = 0f;
            }
        }


    }


    public void ToMainMenu()
    {
        SoundManager.instance.Play(SoundType.ExitGame, transform.position);
        sceneAnimator.SetTrigger("FadeOut");
        Time.timeScale = 1f;
        Invoke("LoadMainMenu", 1f);
    }

    public void LoadMainMenu()
    {
        
        SceneManager.LoadScene(0);
    }

    public void SaveGame()
    {
        SaveManager.instance.gameData.playerPos = PlayerController.instance.transform.position;
        SaveManager.instance.gameData.playerHealth = PlayerController.instance.myHealth.health;
        SaveManager.instance.gameData.haveItemList = GameManager.instance.myItemList;

        if (PlayerController.instance.myHealth.health <= 0f)
        {
            SaveManager.instance.KillData(); //如果Player已死就不存檔,並且把現有的資料刪除(player不可能在死掉時進選單存檔,如果可以那就是bug,故刪檔)
        }
        else
        {
            SaveManager.instance.SaveData();
            SoundManager.instance.Play(SoundType.SaveGame, transform.position);
        }
    }


}
