using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CardVisual : MonoBehaviour
{
    private List<Image> allImages = new List<Image>();

    private void Awake()
    {
        allImages.AddRange(GetComponentsInChildren<Image>());
    }

    public void SetColor(Color color)
    {
        foreach (Image i in allImages)
            i.color = color;
    }
}
