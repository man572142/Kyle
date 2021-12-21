using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarEffect : MonoBehaviour
{
    [SerializeField] Image myBar = null;
    [SerializeField] Image targetBar = null;
    [SerializeField] float speed = 3f;

    // Update is called once per frame
    void Update()
    {
        myBar.fillAmount = Mathf.Lerp(myBar.fillAmount, targetBar.fillAmount, Time.deltaTime * speed);
    }
}
