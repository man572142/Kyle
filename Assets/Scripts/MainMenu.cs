using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Animator animator = null;
    [SerializeField] GameObject continueUI = null;
    [SerializeField] TextEffect narrateScreen = null;


    private void Start()
    {
        //narrateScreen.gameObject.SetActive(false);

        bool haveSaveFile = SaveManager.instance.LoadData();
        if (haveSaveFile && SaveManager.instance.gameData.playerHealth > 0)//如果沒有紀錄
        {
            continueUI.SetActive(true);

        }
        else
        {
            continueUI.SetActive(false);
        }

        //每次到主選單就停止播放音樂
        MusicPlayer musicPlayer = FindObjectOfType<MusicPlayer>();
        if(musicPlayer != null) musicPlayer.PlaybackControl(false);

        //musicPlayer.PlaybackControl(true);

        Cursor.lockState = CursorLockMode.None;

    }


    public void NewGame()
    {
        //將記錄刪除並要求系統再次載入檔案
        PlayerPrefs.DeleteKey("SAVE");
        //SaveManager.instance.LoadData();    //在進入場景後Player會於Start()再次讀取
        StartCoroutine(NewGameNarrate());

        //animator.SetTrigger("Play");
        //Invoke("ChangeScene", 1.1f);
    }

    IEnumerator NewGameNarrate()
    {
        narrateScreen.gameObject.GetComponent<Animator>().SetBool("Play", true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        asyncOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(5f);  //FadeOut加上MiProduction的動畫長度
        yield return StartCoroutine(narrateScreen.TextFillIn());

        asyncOperation.allowSceneActivation = true;
    }

    public void LoadMenu()
    {
        //讀取playerData中所記錄的玩家待的場景
        //Debug.Log("load scene : " + SaveManager.instance.gameData.sceneName);
        SceneManager.LoadScene(SaveManager.instance.gameData.sceneName);
    }
    public void LoadGame()
    {
        //讀檔,將相關資料存入playerData
        //SaveManager.instance.LoadData();  改成在進入場景後由Player於Awake()直接讀取
        
        animator.SetTrigger("Play");
        Invoke("LoadMenu", 1.1f);
        //SaveManager.instance.SaveData();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
