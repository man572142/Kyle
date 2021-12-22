using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveManager
{
    static public SaveManager instance
    {
        get 
        {
            if (_instance == null)
            {
                _instance = new SaveManager();
            }
            return _instance; 
        }

    }
    static SaveManager _instance = null;
    
    
    public GameData gameData = new GameData();
   
    /// <summary>�b�s�ɤ��e</summary>
    public Action Act_OnSave = null;


    /// <summary>�q�w���ɮ�Ū�� </summary>
    public bool LoadData()
    {
        string ogData = PlayerPrefs.GetString("SAVE", "");
        if (ogData == "")  //�p�G���e�٨S�s�ɹL
        {
            Debug.Log("�٨S�s�ɹL");
            gameData = new GameData();  //��l�ƪ��a�U���ƭ�
            gameData.sceneName = "MainGame";
            gameData.playerPos = new Vector3(470f,30.41f,698.5f);
            gameData.playerHealth = 100f;


            List<HaveItem> tempList = new List<HaveItem>();  //��l�ƪ��a�����~
            HaveItem item1 = new HaveItem();
            item1.id = 0;
            item1.amount = 2;
            tempList.Add(item1);
            gameData.haveItemList = tempList;

            List<MechanicData> mechanicList = new List<MechanicData>();
            MechanicData mechanicData = new MechanicData();
            mechanicData.objName = "";
            mechanicData.currentKey = 0;
            mechanicData.isOpen = false;
            mechanicList.Add(mechanicData);
            gameData.mechanicDatas = mechanicList;

            List<AIdata> aIdataList = new List<AIdata>();
            AIdata aiData = new AIdata();
            aiData.objName = "";
            aiData.pos = Vector4.zero;
            aiData.hp = 100;
            aIdataList.Add(aiData);
            gameData.aiDatas = aIdataList;

            gameData.keyDatas = new List<string>();
            gameData.deadEnemy = new List<string>();



            return false;  //�٨S�s�ɹL�N�^��false��ܨS������
        }
        else
        {
            Debug.Log("Ū���{���s��");
            gameData = JsonUtility.FromJson<GameData>(ogData); //Ū���{�����s��(Json)���ର���
            return true;
        }

    }
    /// <summary>�N�{������Ʀs�J�w�� </summary>
    public void SaveData()
    {
        if (Act_OnSave != null)  //�p�G���H�q�\��
        {
            Act_OnSave.Invoke();  //�I�s�Ҧ��q�\Act_OnSave�� 
        }
            

        //���N����ഫ��Json
        string json = JsonUtility.ToJson(gameData, true);
        //��json�s�i�w��
        PlayerPrefs.SetString("SAVE",json);
        Debug.Log(json);

    }

    /// <summary>�R��</summary>
    public void KillData()
    {
        PlayerPrefs.DeleteKey("SAVE");
    }


}

[System.Serializable]
public struct GameData
{
    /// <summary>�����W��</summary>
    public string sceneName;
    public Vector3 playerPos;
    public float playerHealth;
    public List<HaveItem> haveItemList;
    public List<AIdata> aiDatas;
    public List<string> keyDatas;
    public List<MechanicData> mechanicDatas;
    public List<string> deadEnemy;
}

[System.Serializable]
public struct AIdata
{
    /// <summary>AI�W��</summary>
    public string objName;
    public Vector4 pos;  //���Fxyz�H�~�A�h�@��w��(����q)
    public float hp;
}

[System.Serializable]
public struct MechanicData
{
    public string objName;
    public bool isOpen;
    public int currentKey;
}