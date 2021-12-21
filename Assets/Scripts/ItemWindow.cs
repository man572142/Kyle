using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemWindow : MonoBehaviour
{
    [SerializeField] RectTransform itemBackground = null;
    [SerializeField] GameObject itemUI = null;
    /// <summary>刷新時需要刪除的東西</summary>
    [SerializeField] List<GameObject> needKill = new List<GameObject>();


    private void Awake()
    {
        GameManager.instance.Act_StuffChange += UpdateUI;
        itemUI.SetActive(false);

    }

    private void OnDestroy()
    {
        if(GameManager.instance != null)  //因為遊戲關閉時,不確定誰會先被刪掉, 退訂前檢查頻道還在不在
        {
            GameManager.instance.Act_StuffChange -= UpdateUI;
        }
        
    }

    void UpdateUI()
    {
        for(int i = 0; i < needKill.Count; i++)
        {
            Destroy(needKill[i].gameObject);
        }

        needKill.Clear(); //整個列表清空

        for(int i = 0; i < GameManager.instance.myItemList.Count; i++)  //依照我目前擁有的道具種類量
        {
            if (GameManager.instance.myItemList[i].id > 900) continue;  //如果是鑰匙就跳過迴圈
            GameObject temp = Instantiate(itemUI, itemBackground);
            temp.name = GameManager.instance.myItemList[i].id.ToString();  //把生成的UI以其ID為名,為了在暫停時可以被itemInfoWindow使用
            temp.SetActive(true);
            needKill.Add(temp);  //放入下次刷新時要刪除的清單中

            ItemData itemDataTemp = ItemDataManager.instance.GetDataByID(GameManager.instance.myItemList[i].id);
            temp.GetComponent<Image>().sprite = itemDataTemp.icon;
            temp.GetComponent<Image>().color = itemDataTemp.color;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.instance.myItemList[i].amount.ToString();
        }
    }
}
