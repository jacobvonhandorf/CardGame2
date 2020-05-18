using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private SpriteRenderer background;

    public void setText(int number)
    {
        text.text = "" + number;
    }
    public void setBorderColor(Color color)
    {
        border.color = color;
    }
    public void setBackgroundColor(Color color)
    {
        background.color = color;
    }
}
