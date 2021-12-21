using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager
{
    /// <summary>GameManager����ڭ�(�O�����m)</summary>
    static GameManager m_instance = null;  
    static public GameManager instance
    {
        get
        {
            if(m_instance == null)  //�p�G���٤��s�b
            {
                m_instance = new GameManager();  //�ͦ��ڦۤv
            }
            return m_instance;  //�^�ǧڦۤv
        }
    }
    /// <summary>�D��o�ͤF�ܤ�</summary>
    public Action Act_StuffChange = null;
    /// <summary>�_�͵o�ͤF�ܤ� </summary>
    public Action Act_KeyChange = null;

    public List<HaveItem> myItemList = new List<HaveItem>();


    /// <summary>�q�O���ɸ��J���~</summary>
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


    /// <summary> �K�[�@�ӹD�� </summary>
    public void AddItem(int newId)
    {
        for(int i = 0;i < myItemList.Count; i++ )
        {
            if(myItemList[i].id == newId)  //�p�G�s�����~�w�g�����N�u�ݭn�W�[�ƶq
            {
                //id 900�H�W�O�_��,�_�ͷ�@���P���O���x�s,���|�[�ƶq
                if (newId >= 900) break; //���X�j��

                HaveItem temp = myItemList[i];    //List���ĥΩ⴫���覡�~������
                temp.amount++;
                myItemList[i] = temp;
                
                InvokeItem();                              

                return;  //�@���u��[�@�ӹD��
            }
        }
        //�j��]�����S���M�w�����D��ۦP,�s�W�D�����O
        HaveItem haveItem = new HaveItem();
        haveItem.id = newId;
        haveItem.amount = 1;
        myItemList.Add(haveItem);

        if (newId >= 900) InvokeKey();  //�p�G�O�_�ʹN�q���q�\key���H
        else InvokeItem();   //�_�h�N�O�q���q�\item���H
    }

    public void AddItem(int newId, int amount)  //AddItem���h��,�Ϩ�i�@���K�[�j�qitem
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

    /// <summary> �@���K�[�j�q�D�� </summary>


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
                if(myItemList[i].amount >= 2)  //�D��ƶq�٦��ܤ�2�� ,�u�n��ּƶq�Y�i
                {
                    HaveItem temp = myItemList[i];
                    temp.amount--;
                    myItemList[i] = temp;
                }
                else   //�D��u��1��  ������ֹD��i�����q�M�椤����
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
            ItemData temp = ItemDataManager.instance.GetDataByID(item.id);  //�Ϋ���item��id �b��Ʈw����L�����
            Debug.Log("����" + temp.title + " x " + item.amount);

        }
    }
    /// <summary>�̷Ӭ������J�C��</summary>
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
