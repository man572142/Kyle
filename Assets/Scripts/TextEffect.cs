using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class TextEffect : MonoBehaviour
{
    [SerializeField] List<NattrateText> textList = new List<NattrateText>();
    
    [SerializeField] Text text = null;
    string readyToPrint = "";
    [SerializeField] float fillTime = 0.3f;
    [SerializeField] float nextLineTime = 1f;
    bool isFinished = false;
    [SerializeField] AudioSource glitchSFX = null;

    void Start()
    {
        
    }

    public IEnumerator TextFillIn()
    {
        for (int i = 0; i < textList.Count; i++)  //�ĴX��
        {
            //if (narrateList[i].Length < 1) return;  //�i��ݭn�Ψ���µe��,�G�o�̤��ݭn���L�ŭ�
            char[] word = new char[textList[i].narrateText.Length];
            word = textList[i].narrateText.ToCharArray();

            SoundManager.instance.Play(SoundType.TextSFX,transform.position);
            for (int c = 0; c < word.Length; c++)  //��Ӧ檺�r�C�j�@�q�ɶ���J�@�r
            {
                yield return new WaitForSeconds(fillTime);
                readyToPrint = readyToPrint + word[c];
                text.text = readyToPrint;
            }
            SoundManager.instance.Stop(SoundType.TextSFX);
            yield return new WaitForSeconds(nextLineTime + word.Length * 0.06f);
            readyToPrint = "";

            if(i == textList.Count -1)   //�̫�@�y
            {
                yield return new WaitForSeconds(0.8f);
            }
        }




    }

    //�q�ʵe���I�s
    public void PlayGlitch()
    {
        glitchSFX.Play();
    }

}

[System.Serializable]
public struct NattrateText
{
    public string narrateText;
}
