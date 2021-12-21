using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepsManager : MonoBehaviour
{
    [SerializeField] AudioSource myAudioSource = null;
    [SerializeField] List<AudioClip> runClip = new List<AudioClip>();
    [SerializeField] List<AudioClip> walkClip = new List<AudioClip>();
    int footMode = 0;
    float playVolume = 1f;
    
    public void Footstep(int animeNum)
    {
        if (myAudioSource == null) return;
        if (animeNum != footMode) return;

        if(animeNum == 99)  //«á°h¼Ò¦¡
        {
            int walkIndex = Random.Range(0, walkClip.Count);
            myAudioSource.PlayOneShot(walkClip[walkIndex], playVolume);
        }
        else
        {
            int runIndex = Random.Range(0, runClip.Count);
            myAudioSource.PlayOneShot(runClip[runIndex], playVolume);
        }
        
    }

    public void SetFootStepMode(int mode,float vol)
    {
        footMode = mode;
        playVolume = vol;
    }

}
