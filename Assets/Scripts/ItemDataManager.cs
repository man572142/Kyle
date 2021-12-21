using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager
{
    static ItemDataManager _instance = null;
    ItemData[] allData = new ItemData[0];
    static public ItemDataManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new ItemDataManager();
                _instance.LoadData();
            }
            return _instance;
        }
    }

    public void LoadData()
    {
        allData = Resources.LoadAll<ItemData>("");

        //foreach (ItemData item in allData)
        //{
        //    Debug.Log(item.title + " / " + item.info);
        //}
    }


    public ItemData GetDataByID(int id)
    {
        for(int i = 0; i < allData.Length; i++)
        {
            if(allData[i].id == id)
            {
                return allData[i];  //把找到的資料回傳 (資料是Resources中的內容)
            }
        }
        Debug.LogWarning("資料庫中缺少id :" + id + "物件");  //如果都找不到
        return null;
    }

}
