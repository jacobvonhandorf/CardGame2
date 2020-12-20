using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static Card;

[RequireComponent(typeof(Image))]
public class EIdChangeHandler : StatChangeListener
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    protected override void OnValueUpdated(object value)
    {
        //Debug.Log("EID Change");
        ElementIdentity eId = (ElementIdentity)value;
        string spriteName = null;
        switch (eId)
        {
            case ElementIdentity.Fire:
                spriteName = "fire background";
                break;
            case ElementIdentity.Water:
                spriteName = "water background";
                break;
            case ElementIdentity.Wind:
                spriteName = "wind background";
                break;
            case ElementIdentity.Earth:
                spriteName = "earth background";
                break;
            case ElementIdentity.Nuetral:
                spriteName = "nuetral background";
                break;
        }
        image.sprite = Resources.Load<Sprite>(spriteName);
    }
}
