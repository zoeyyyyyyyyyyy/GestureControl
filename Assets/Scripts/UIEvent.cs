using System;
using UnityEngine;
using UnityEngine.UI;

public class UIEvent : MonoBehaviour
{
    public UIInteractiveLogic uiInteractiveLogic;
    private Image image;

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.name);
        uiInteractiveLogic.uiObj = col.gameObject;
        if (image == null)
        {
            image = col.gameObject.GetComponent<Image>();
            image.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (image != null)
        {
            image.enabled = false;
            image = null;
        }
    }
}