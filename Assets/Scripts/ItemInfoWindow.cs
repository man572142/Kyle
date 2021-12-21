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

    //����� 
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
        int haveAmount = GameManager.instance.GetAmontById(myID);  //�ˬd�D������q,�p�G�ƶq��0�N�۰���������
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
        if(itemData.costType == CostType.COST_ONE)  //�T�{�D��ϧ_�|�Q����
        {
            GameManager.instance.DecreaseItem(myID);
        }


        
    }

}
