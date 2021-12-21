using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    //public MechanicType mechanicType;
    [SerializeField] MechanicController targetMechanic = null;
    [SerializeField] Animator animator = null;
    [SerializeField] float speed = 1f;
    public int keyID = 999;
    bool isCollected = false;
    bool isPlay = false;
    Vector3 highPos = Vector3.zero;
    bool startFly = false;

    private void Start()
    {
        //SaveManager.instance.Act_OnSave += Save;

        //讀取鑰匙狀態
        if (SaveManager.instance.gameData.keyDatas == null) return;
        //Debug.Log("load key state " + SaveManager.instance.gameData.keyDatas.Count);
        for (int i = 0; i < SaveManager.instance.gameData.keyDatas.Count; i++)
        {
            if (SaveManager.instance.gameData.keyDatas[i] == this.name) //有在清單中表示有被撿起並進到神廟中
            {
                Destroy(gameObject);
            }
        }

        highPos = transform.position + (Vector3.up * 10f);
    }

    private void OnDestroy()
    {
        //SaveManager.instance.Act_OnSave -= Save;
    }

    //void Save()
    //{
    //    if (!isCollected) return;  //沒被撿起來的就不用存檔

    //    Debug.Log("collected key save");
    //    for (int i = 0; i < SaveManager.instance.gameData.keyDatas.Count; i ++)
    //    {
    //        if (SaveManager.instance.gameData.keyDatas[i] == this.name)
    //        return;
    //        Debug.Log("save " + SaveManager.instance.gameData.keyDatas[i]);
    //    }
    //    SaveManager.instance.gameData.keyDatas.Add(this.name);
    //    Debug.Log(SaveManager.instance.gameData.keyDatas[0]);
    //}



    private void OnCollisionEnter(Collision other)  //被撿起
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Bullet")
        {
            if (targetMechanic == null || animator == null) return;

            SoundManager.instance.Play(SoundType.CollectKey, transform.position);
            //targetMechanic.AddKey(this);
            //GameManager.instance.AddItem(keyID);
            animator.SetBool("Collect", true);
            isCollected = true;
        }
        
    }

    //private Vector3 GetHighPos()
    //{
    //    if(highPos == Vector3.zero)
    //    {
    //        highPos = transform.position + (Vector3.up * 5f);
    //        Debug.Log(highPos);
    //    }
    //    return highPos;
    //}

   
    void Update()
    {
        if(isCollected)
        {
            Vector3 targetY = new Vector3(transform.position.x, targetMechanic.transform.position.y, transform.position.z); //目標的高度
            float relativeY = targetMechanic.transform.position.y - transform.position.y;
            float highY = highPos.y - transform.position.y;

            if (startFly)  //到差不多的高度就可以開始往目標前進
            {
                //highPos = Vector3.zero;
                transform.position = Vector3.Lerp(transform.position, targetMechanic.transform.position + Vector3.up, Time.deltaTime * speed);
                if (!isPlay)
                {
                    SoundManager.instance.Play(SoundType.KeySpeedFly, transform.position);
                    isPlay = true;
                }
            }
            else if (highY >= 1f)  //如果還沒先飛到10公尺高
            {
                transform.position = Vector3.Lerp(transform.position, highPos, Time.deltaTime * speed * 5f);
            }
            else if (highY < 1f  && relativeY > 5f )   //如果已經飛高10公尺,卻還是低於目標高度超過5公尺
            {
                transform.position = Vector3.Lerp(transform.position, targetY, Time.deltaTime * speed * 2);  //繼續往上飛
            }
            else
            {
                startFly = true;
            }     
                     
        }
    }

}
