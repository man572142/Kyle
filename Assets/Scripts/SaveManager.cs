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
   
    /// <summary>在存檔之前</summary>
    public Action Act_OnSave = null;


    /// <summary>從硬碟檔案讀取 </summary>
    public bool LoadData()
    {
        string ogData = PlayerPrefs.GetString("SAVE", "");
        if (ogData == "")  //如果之前還沒存檔過
        {
            Debug.Log("還沒存檔過");
            gameData = new GameData();  //初始化玩家各項數值
            gameData.sceneName = "MainGame";
            gameData.playerPos = new Vector3(470f,30.41f,698.5f);
            gameData.playerHealth = 100f;


            List<HaveItem> tempList = new List<HaveItem>();  //初始化玩家的物品
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



            return false;  //還沒存檔過就回傳false表示沒有紀錄
        }
        else
        {
            Debug.Log("讀取現有存檔");
            gameData = JsonUtility.FromJson<GameData>(ogData); //讀取現有的存檔(Json)並轉為資料
            return true;
        }

    }
    /// <summary>將現有的資料存入硬碟 </summary>
    public void SaveData()
    {
        if (Act_OnSave != null)  //如果有人訂閱我
        {
            Act_OnSave.Invoke();  //呼叫所有訂閱Act_OnSave的 
        }
            

        //先將資料轉換為Json
        string json = JsonUtility.ToJson(gameData, true);
        //把json存進硬碟
        PlayerPrefs.SetString("SAVE",json);
        Debug.Log(json);

    }

    /// <summary>刪檔</summary>
    public void KillData()
    {
        PlayerPrefs.DeleteKey("SAVE");
    }


}

[System.Serializable]
public struct GameData
{
    /// <summary>場景名稱</summary>
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
    /// <summary>AI名稱</summary>
    public string objName;
    public Vector4 pos;  //除了xyz以外再多一個w值(旋轉量)
    public float hp;
}

[System.Serializable]
public struct MechanicData
{
    public string objName;
    public bool isOpen;
    public int currentKey;
}