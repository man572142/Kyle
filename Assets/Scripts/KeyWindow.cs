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
        if (GameManager.instance != null)  //�]���C��������,���T�w�ַ|���Q�R��, �h�q�e�ˬd�W�D�٦b���b
        {
            GameManager.instance.Act_KeyChange -= UpdateUI;
        }

    }

    void UpdateUI()
    {
        //if (keyCount >= 3) return;  //�w�g�����_�ͫ�N���A��s

        for (int i = 0; i < GameManager.instance.myItemList.Count; i++)  //�̷ӧڥثe�֦����D������q
        {
            if (GameManager.instance.myItemList[i].id != targetKeyID) continue;  //�p�G���O���w���_�ʹN���L�o���j��

            ItemData itemDataTemp = ItemDataManager.instance.GetDataByID(GameManager.instance.myItemList[i].id);
            transform.GetChild(keyCount).GetComponent<Image>().sprite = itemDataTemp.icon;
            transform.GetChild(keyCount).GetComponent<Image>().color = itemDataTemp.color;
            keyCount++;
        }


        keyCount = 0;
    }
}
