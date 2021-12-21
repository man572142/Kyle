using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource = null;
    
    void Start()
    {
        //MusicPlayer[] musicPlayer = FindObjectsOfType<MusicPlayer>();
        //if(musicPlayer.Length > 1)  //如果場上已有MusicPlayer,加上自己便大於1,故刪除自己
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    DontDestroyOnLoad(this.gameObject);
        //}
        

    }

    public void PlaybackControl(bool play)
    {
        if(play)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
