using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlocker : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        UIEvents.EnableUIBlocker.AddListener(delegate ()
        {
            image.enabled = true;
        });
        UIEvents.DisableUIBlocker.AddListener(delegate ()
        {
            image.enabled = false;
        });
    }

    private void OnDisable()
    {
        UIEvents.EnableUIBlocker.RemoveAllListeners();
        UIEvents.DisableUIBlocker.RemoveAllListeners();
    }
}
