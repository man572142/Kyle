using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField] Animator animator = null;
    [SerializeField] List<string> credits = new List<string>();
    [SerializeField] Text text = null;
    [SerializeField] float waitTime = 5f;
    [SerializeField] float firstWaitTime = 2f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(firstWaitTime);
        for(int i = 0; i < credits.Count; i++)
        {
            text.text = credits[i];
            animator.SetTrigger("Fade");
            yield return new WaitForSeconds(waitTime);
            
        }
        animator.SetTrigger("End");
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0);
        asyncOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(30f);
        asyncOperation.allowSceneActivation = true;
    }

    void Update()
    {
        
    }
}
