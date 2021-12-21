using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static public SoundManager instance = null;
    [SerializeField] List<SoundPack> soundList = new List<SoundPack>();

    /// <summary>�p���h�ӬۦP�γ~���ĴN�����b�@�_,�H�K�H������</summary>
    List<SoundPack> collectedSounds = new List<SoundPack>();

    private void Awake()
    {
        instance = this;
    }

    public void Play(SoundType soundType,Vector3 pos)
    {
        int playNum = -1;
        collectedSounds.Clear();  //�}�l�e���M�z�}�C
        for (int i = 0; i < soundList.Count; i++)
        {
            if(soundList[i].soundType == soundType)
            {
                soundList[i].audioSource.transform.position = pos;
                playNum = i;
                collectedSounds.Add(soundList[i]);
            }
        }
        if (collectedSounds.Count < 2)  //�p�G�S���W�L1�ӭ��ĴN��������
        {
            soundList[playNum].audioSource.Play(); 
        }
        else
        {
            //�W�L1�ӴN�H����������䤤�@��
            int num = Random.Range(0, collectedSounds.Count);
            collectedSounds[num].audioSource.Play();
        }
        

    }

    public void PlayUIsound(string soundType)
    {
        SoundType temp = (SoundType)System.Enum.Parse(typeof(SoundType), soundType);

        Play(temp, Vector3.zero);
    }

    public void Stop(SoundType soundType)
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            if (soundList[i].soundType == soundType)
            {
                soundList[i].audioSource.Stop();
            }
        }
    }


}

public enum SoundType
{
    Music,
    ButtonTouch,
    ButtonClick,
    SaveGame,
    ExitGame,
    SwordAttack,
    Shoot,
    PrepareShoot,
    PlayerHit,
    CollectKey,
    SkeletonAttack,
    SkeletonHit,
    KeySpeedFly,
    PlayerDie,
    TextSFX,
    ClickUI,
    SettingUI,
    CallEnemy,
    CoolBubble,
    Pickup,
    ThrowItem,
    UseItem
}

[System.Serializable]
public struct SoundPack
{
    public AudioSource audioSource;
    public SoundType soundType;
}
