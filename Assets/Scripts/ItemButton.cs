using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        ItemInfoWindow.instance.SetDataAndOpen(int.Parse(this.name));
        SoundManager.instance.Play(SoundType.ClickUI,transform.position);
    }


}
