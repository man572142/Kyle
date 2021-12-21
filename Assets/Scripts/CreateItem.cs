using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateItem : MonoBehaviour
{
    /// <summary>�����a�i�H�ߪ��D�� </summary>
    [SerializeField] GameObject arrowPickup = null;
    [SerializeField] GameObject corePickup = null;
    [SerializeField] GameObject chipPickup = null;
    /// <summary>�|�����_���������v��(�bInspector���K�[)</summary>
    [SerializeField] List<ItemWeights> allTreasure = new List<ItemWeights>();
    /// <summary>�@�����_���ƶq</summary>
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
            if(winNumber > allTreasure[i].weights)  //�Ҥ�ثe���v���j,��ܨS���b�d��(�b�t)
            {
                winNumber -= allTreasure[i].weights;  //�N�������X��h�ثe���X,���ˬd�I����,���� winNumber��allTreasure�p(�Y����)
            }
            else
            {
                GameObject pickup = null;
                if (allTreasure[i].id == 0) pickup = arrowPickup;
                else if (allTreasure[i].id == 1) pickup = corePickup;
                else if (allTreasure[i].id == 2) pickup = chipPickup;

                Vector3 spawnPos = transform.position;
                if (spawnTransform != null) spawnPos = spawnTransform.position;  //�p�G���w��spawn�I,�N�Υ����y��

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