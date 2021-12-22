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
        if (haveSaveFile && SaveManager.instance.gameData.playerHealth > 0)//�p�G�S������
        {
            continueUI.SetActive(true);

        }
        else
        {
            continueUI.SetActive(false);
        }

        //�C����D���N����񭵼�
        MusicPlayer musicPlayer = FindObjectOfType<MusicPlayer>();
        if(musicPlayer != null) musicPlayer.PlaybackControl(false);

        //musicPlayer.PlaybackControl(true);

        Cursor.lockState = CursorLockMode.None;

    }


    public void NewGame()
    {
        //�N�O���R���ín�D�t�ΦA�����J�ɮ�
        PlayerPrefs.DeleteKey("SAVE");
        //SaveManager.instance.LoadData();    //�b�i�J������Player�|��Start()�A��Ū��
        StartCoroutine(NewGameNarrate());

        //animator.SetTrigger("Play");
        //Invoke("ChangeScene", 1.1f);
    }

    IEnumerator NewGameNarrate()
    {
        narrateScreen.gameObject.GetComponent<Animator>().SetBool("Play", true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        asyncOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(5f);  //FadeOut�[�WMiProduction���ʵe����
        yield return StartCoroutine(narrateScreen.TextFillIn());

        asyncOperation.allowSceneActivation = true;
    }

    public void LoadMenu()
    {
        //Ū��playerData���ҰO�������a�ݪ�����
        //Debug.Log("load scene : " + SaveManager.instance.gameData.sceneName);
        SceneManager.LoadScene(SaveManager.instance.gameData.sceneName);
    }
    public void LoadGame()
    {
        //Ū��,�N������Ʀs�JplayerData
        //SaveManager.instance.LoadData();  �令�b�i�J�������Player��Awake()����Ū��
        
        animator.SetTrigger("Play");
        Invoke("LoadMenu", 1.1f);
        //SaveManager.instance.SaveData();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
