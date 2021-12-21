using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateItem : MonoBehaviour
{
    /// <summary>讓玩家可以撿的道具 </summary>
    [SerializeField] GameObject arrowPickup = null;
    [SerializeField] GameObject corePickup = null;
    [SerializeField] GameObject chipPickup = null;
    /// <summary>會掉的寶物種類及權重(在Inspector中添加)</summary>
    [SerializeField] List<ItemWeights> allTreasure = new List<ItemWeights>();
    /// <summary>一次掉寶的數量</summary>
    [SerializeField] int giftAmount = 2;
    [SerializeField] Transform spawnTransform = null;

    public void GetAllItem()
    {
        for(int i = 0; i < giftAmount; i++)
        {
            GetItem();
        }
    }


    public void GetItem()
    {
        int totalWeight = 0;

        foreach(ItemWeights item in allTreasure)
        {
            totalWeight += item.weights;
        }

        int winNumber = Random.Range(0, totalWeight); 

        for(int i = 0; i < allTreasure.Count; i++)
        {
            if(winNumber > allTreasure[i].weights)  //籤比目前的權重大,表示沒有在範圍內(槓龜)
            {
                winNumber -= allTreasure[i].weights;  //將中獎號碼減去目前號碼,使檢查點往後,直到 winNumber比allTreasure小(即中獎)
            }
            else
            {
                GameObject pickup = null;
                if (allTreasure[i].id == 0) pickup = arrowPickup;
                else if (allTreasure[i].id == 1) pickup = corePickup;
                else if (allTreasure[i].id == 2) pickup = chipPickup;

                Vector3 spawnPos = transform.position;
                if (spawnTransform != null) spawnPos = spawnTransform.position;  //如果有安排spawn點,就用它的座標

                GameObject temp = Instantiate(pickup, spawnPos + Vector3.up * Random.Range(0f,2f), 
                    Quaternion.Euler(Random.Range(0f,360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));

                temp.name = allTreasure[i].id.ToString(); 

                return;
            }
        }
       
    }
}

[System.Serializable]
public struct ItemWeights
{
    public int id;
    [Range(1f,100f)] public int weights;


}