using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager
{
    /// <summary>GameManager的實際值(記憶體位置)</summary>
    static GameManager m_instance = null;  
    static public GameManager instance
    {
        get
        {
            if(m_instance == null)  //如果我還不存在
            {
                m_instance = new GameManager();  //生成我自己
            }
            return m_instance;  //回傳我自己
        }
    }
    /// <summary>道具發生了變化</summary>
    public Action Act_StuffChange = null;
    /// <summary>鑰匙發生了變化 </summary>
    public Action Act_KeyChange = null;

    public List<HaveItem> myItemList = new List<HaveItem>();


    /// <summary>從記錄檔載入物品</summary>
    /// <param name="haveItems"></param>
    public void LoadPlayerItem(List<HaveItem> haveItems)
    {
        myItemList.Clear();
        myItemList = haveItems;
        
        if(Act_StuffChange != null)
        {
            Act_StuffChange.Invoke();
            Act_KeyChange.Invoke();
        }
        
    }


    /// <summary> 添加一個道具 </summary>
    public void AddItem(int newId)
    {
        for(int i = 0;i < myItemList.Count; i++ )
        {
            if(myItemList[i].id == newId)  //如果新的物品已經持有就只需要增加數量
            {
                //id 900以上是鑰匙,鑰匙當作不同類別來儲存,不疊加數量
                if (newId >= 900) break; //跳出迴圈

                HaveItem temp = myItemList[i];    //List須採用抽換的方式才能更改其值
                temp.amount++;
                myItemList[i] = temp;
                
                InvokeItem();                              

                return;  //一次只能加一個道具
            }
        }
        //迴圈跑完都沒有和已持有道具相同,新增道具類別
        HaveItem haveItem = new HaveItem();
        haveItem.id = newId;
        haveItem.amount = 1;
        myItemList.Add(haveItem);

        if (newId >= 900) InvokeKey();  //如果是鑰匙就通知訂閱key的人
        else InvokeItem();   //否則就是通知訂閱item的人
    }

    public void AddItem(int newId, int amount)  //AddItem的多載,使其可一次添加大量item
    {
        for (int i = 0; i < amount; i++)
        {
            AddItem(newId);
        }

    }

    private void InvokeItem()
    {
        if (Act_StuffChange != null)  
        {
            Act_StuffChange.Invoke();
        }
    }
    private void InvokeKey()
    {
        if (Act_KeyChange != null)  
        {
            Act_KeyChange.Invoke();
        }
    }

    /// <summary> 一次添加大量道具 </summary>


    public int GetAmontById(int id)
    {
        for(int i = 0; i < myItemList.Count; i++)
        {
            if(myItemList[i].id == id)
            {
                return myItemList[i].amount;
            }
        }
        return 0;
    }

    public void DecreaseItem(int id)
    {
        for(int i = myItemList.Count - 1; i >= 0; i--)
        {
            if(myItemList[i].id == id)
            {
                if(myItemList[i].amount >= 2)  //道具數量還有至少2個 ,只要減少數量即可
                {
                    HaveItem temp = myItemList[i];
                    temp.amount--;
                    myItemList[i] = temp;
                }
                else   //道具只剩1個  此次減少道具可直接從清單中移除
                {
                    myItemList.RemoveAt(i);
                }
                InvokeItem();
                //InvokeKey();
            }
        }
    }

    public void Test()
    {
        foreach(HaveItem item in myItemList)
        {
            ItemData temp = ItemDataManager.instance.GetDataByID(item.id);  //用持有item的id 在資料庫中找他的資料
            Debug.Log("持有" + temp.title + " x " + item.amount);

        }
    }
    /// <summary>依照紀錄載入遊戲</summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("MainGame");
    }

}

[System.Serializable]
public struct HaveItem
{
    public int id;
    public int amount;

}
