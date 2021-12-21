using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemInfoWindow : MonoBehaviour
{
    [SerializeField] Image icon = null;
    [SerializeField] TextMeshProUGUI title = null;
    [SerializeField] Text amountText = null;
    [SerializeField] Text info = null;
    [SerializeField] CanvasGroup canvasGroup = null;
    [SerializeField] GameObject useItemButton = null;

    public Action<int> Act_UseStuff = null;

    //假單例 
    static public ItemInfoWindow instance = null;
    int myID = -1;

    private void Awake()
    {
        instance = this;
        open = false;
    }

    public void CloseItemUI()
    {
        open = false;
    }


    bool open
    {
        get { return _open; }
        set
        {
            _open = value;
            canvasGroup.alpha = value ? 1f : 0f;
            canvasGroup.blocksRaycasts = value;

            if(value == true)
            {
                GameManager.instance.Act_StuffChange += UpdateUI;
            }
            else
            {
                GameManager.instance.Act_StuffChange -= UpdateUI; 
            }
        }
    }
    bool _open = false;
    


    public void SetDataAndOpen(int id)
    {
        open = true;

        myID = id;

        
        UpdateUI();

    }

    void UpdateUI()
    {
        int haveAmount = GameManager.instance.GetAmontById(myID);  //檢查道具持有量,如果數量為0就自動關閉介面
        if(haveAmount <= 0)
        {
            open = false;
            return;
        }

        ItemData itemData = ItemDataManager.instance.GetDataByID(myID);
        icon.sprite = itemData.icon;
        icon.color = itemData.color;
        title.text = itemData.title;
        amountText.text = "x " + haveAmount.ToString();
        info.text = itemData.info;

        useItemButton.SetActive(itemData.useType == UseType.CAN_USE);

    }

    public void ThrowItemButton()
    {
        SoundManager.instance.Play(SoundType.ThrowItem, transform.position);
        GameManager.instance.DecreaseItem(myID);
    }

    public void UseItemButton()
    {
        if(Act_UseStuff != null)
        {
            SoundManager.instance.Play(SoundType.UseItem, transform.position);
            Act_UseStuff.Invoke(myID);
        }

        ItemData itemData = ItemDataManager.instance.GetDataByID(myID);
        if(itemData.costType == CostType.COST_ONE)  //確認道具使否會被消耗
        {
            GameManager.instance.DecreaseItem(myID);
        }


        
    }

}
