using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MechanicController : MonoBehaviour
{
    //[SerializeField] MechanicType mechanicType;
    [SerializeField] UnityEvent getKey;
    [SerializeField] UnityEvent loadState;
    [SerializeField] int targetKeyNum = 3;
    [SerializeField] int currentKeyNum = 0;
    [SerializeField] bool isOpen = false;
    List<string> keySave = new List<string>();

    private void Start()
    {
        SaveManager.instance.Act_OnSave += Save;


        if (SaveManager.instance.gameData.mechanicDatas == null) return;
        for (int i = 0; i < SaveManager.instance.gameData.mechanicDatas.Count; i++)
        {
            MechanicData mechanicData = SaveManager.instance.gameData.mechanicDatas[i];
            if (mechanicData.objName == this.gameObject.name)
            {
                currentKeyNum = mechanicData.currentKey;
                if (mechanicData.isOpen)
                {
                    LoadState();
                    Debug.Log(this.name + " is opened from save file");                 
                }
                break;
            }          
        }
    }

    private void OnDestroy()
    {
        SaveManager.instance.Act_OnSave -= Save;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "Key") return;
        Key key = collider.GetComponentInParent<Key>();
        //if (mechanicType != key.mechanicType) return;
        
        currentKeyNum++;
        GameManager.instance.AddItem(key.keyID);
        keySave.Add(collider.transform.parent.name);  // 先儲存鑰匙到list等待存檔

        if (currentKeyNum >= targetKeyNum)
        {
            Open();
        }
        Destroy(collider.transform.parent.gameObject);
        //GameManager.instance.DecreaseItem(key.keyID);
    }


    void Open()
    {
        getKey.Invoke();
        isOpen = true;
    }

    void LoadState()
    {
        loadState.Invoke();
        isOpen = true;
    }


    void Save()
    {
        for (int i = 0; i < SaveManager.instance.gameData.mechanicDatas.Count; i++)  
        {
            if (SaveManager.instance.gameData.mechanicDatas[i].objName == this.gameObject.name)
            {
                SaveManager.instance.gameData.mechanicDatas.RemoveAt(i);  //找出自己並把之前的資料清空
            }
        }
        MechanicData temp = new MechanicData();
        temp.objName = this.name;
        temp.currentKey = currentKeyNum;
        temp.isOpen = isOpen;
        SaveManager.instance.gameData.mechanicDatas.Add(temp);


        if(keySave.Count > 0)
        {
            SaveManager.instance.gameData.keyDatas = keySave;  //把目前有存的鑰匙都覆蓋到gamedata裡
            Debug.Log("save key " + SaveManager.instance.gameData.keyDatas.Count);
        }
        

        
    }

}
