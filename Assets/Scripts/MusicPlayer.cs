using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource = null;
    
    void Start()
    {
        //MusicPlayer[] musicPlayer = FindObjectsOfType<MusicPlayer>();
        //if(musicPlayer.Length > 1)  //�p�G���W�w��MusicPlayer,�[�W�ۤv�K�j��1,�G�R���ۤv
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
