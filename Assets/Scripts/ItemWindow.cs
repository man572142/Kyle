using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemWindow : MonoBehaviour
{
    [SerializeField] RectTransform itemBackground = null;
    [SerializeField] GameObject itemUI = null;
    /// <summary>��s�ɻݭn�R�����F��</summary>
    [SerializeField] List<GameObject> needKill = new List<GameObject>();


    private void Awake()
    {
        GameManager.instance.Act_StuffChange += UpdateUI;
        itemUI.SetActive(false);

    }

    private void OnDestroy()
    {
        if(GameManager.instance != null)  //�]���C��������,���T�w�ַ|���Q�R��, �h�q�e�ˬd�W�D�٦b���b
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

        needKill.Clear(); //��ӦC��M��

        for(int i = 0; i < GameManager.instance.myItemList.Count; i++)  //�̷ӧڥثe�֦����D������q
        {
            if (GameManager.instance.myItemList[i].id > 900) continue;  //�p�G�O�_�ʹN���L�j��
            GameObject temp = Instantiate(itemUI, itemBackground);
            temp.name = GameManager.instance.myItemList[i].id.ToString();  //��ͦ���UI�H��ID���W,���F�b�Ȱ��ɥi�H�QitemInfoWindow�ϥ�
            temp.SetActive(true);
            needKill.Add(temp);  //��J�U����s�ɭn�R�����M�椤

            ItemData itemDataTemp = ItemDataManager.instance.GetDataByID(GameManager.instance.myItemList[i].id);
            temp.GetComponent<Image>().sprite = itemDataTemp.icon;
            temp.GetComponent<Image>().color = itemDataTemp.color;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.instance.myItemList[i].amount.ToString();
        }
    }
}
