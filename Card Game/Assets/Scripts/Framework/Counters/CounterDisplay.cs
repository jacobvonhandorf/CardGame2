using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CounterDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Outline border;
    [SerializeField] private Image background;

    public void SetText(int number) => text.text = number.ToString();
    public void SetBorderColor(Color color) => border.effectColor = color;
    public void SetBackgroundColor(Color color) => background.color = color;
    public void SetTextColor(Color color) => text.color = color;
}
