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
                return allData[i];  //���쪺��Ʀ^�� (��ƬOResources�������e)
            }
        }
        Debug.LogWarning("��Ʈw���ʤ�id :" + id + "����");  //�p�G���䤣��
        return null;
    }

}
