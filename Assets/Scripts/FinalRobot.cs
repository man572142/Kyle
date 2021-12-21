using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalRobot : MonoBehaviour
{
    //[SerializeField] Animator animator = null;
    [SerializeField] float walkingTime = 3f;
    [SerializeField] float loadSceneTime = 5f;
    Animator animator;
    public void FinalWalk()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Walker());
    }

    IEnumerator Walker()
    {
        animator.SetBool("Final Walk", true);
        yield return new WaitForSeconds(walkingTime);
        animator.SetBool("Final Walk", false);
        yield return new WaitForSeconds(loadSceneTime);
        SceneManager.LoadScene("Ending");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
