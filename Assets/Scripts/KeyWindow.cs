using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyWindow : MonoBehaviour
{
    [SerializeField] GameObject keyUI = null;
    [SerializeField] int targetKeyID = 999;
    int keyCount = 0;
    


    private void Awake()
    {
        GameManager.instance.Act_KeyChange += UpdateUI;
        //keyUI.SetActive(false);
    }

    private void Start()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)  //因為遊戲關閉時,不確定誰會先被刪掉, 退訂前檢查頻道還在不在
        {
            GameManager.instance.Act_KeyChange -= UpdateUI;
        }

    }

    void UpdateUI()
    {
        //if (keyCount >= 3) return;  //已經集滿鑰匙後就不再更新

        for (int i = 0; i < GameManager.instance.myItemList.Count; i++)  //依照我目前擁有的道具種類量
        {
            if (GameManager.instance.myItemList[i].id != targetKeyID) continue;  //如果不是指定的鑰匙就跳過這次迴圈

            ItemData itemDataTemp = ItemDataManager.instance.GetDataByID(GameManager.instance.myItemList[i].id);
            transform.GetChild(keyCount).GetComponent<Image>().sprite = itemDataTemp.icon;
            transform.GetChild(keyCount).GetComponent<Image>().color = itemDataTemp.color;
            keyCount++;
        }


        keyCount = 0;
    }
}
