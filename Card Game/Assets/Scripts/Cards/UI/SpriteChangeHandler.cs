using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteChangeHandler : StatChangeListener
{
    private Image image;

    public void Awake()
    {
        image = GetComponent<Image>();
    }

    protected override void onValueUpdated(object value)
    {
        image.sprite = (Sprite)value;
    }
}
