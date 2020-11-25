using System;
using TMPro;
using UnityEngine;
using static Card;

public abstract class CardStatsGetter : MonoBehaviour
{
    // public Transform cardGraphicsRoot;
    [SerializeField] protected TextMeshPro manaText1;
    [SerializeField] protected TextMeshPro manaText2;
    [SerializeField] protected TextMeshPro goldText;
    [SerializeField] public TextMeshPro nameText;
    [SerializeField] public TextMeshPro effectText;
    [SerializeField] protected TextMeshPro typeText;
    [SerializeField] protected SpriteRenderer background;
    [SerializeField] protected SpriteRenderer cardArt;
    [SerializeField] protected Sprite fireBackground;
    [SerializeField] protected Sprite windBackground;
    [SerializeField] protected Sprite waterBackground;
    [SerializeField] protected Sprite earthBackground;
    [SerializeField] protected Sprite nuetralBackground;
    [SerializeField] protected Color aboveBaseColor;
    [SerializeField] protected Color belowBaseColor;


    public string getCardName()
    {
        return nameText.text;
    }

    public string getEffectText()
    {
        return effectText.text;
    }

    public SpriteRenderer getBackgroundSprite()
    {
        return background;
    }
    public SpriteRenderer getArtSprite()
    {
        return cardArt;
    }
    public void setSprite(Sprite sprite)
    {
        cardArt.sprite = sprite;
    }

    public abstract void setCardViewer(CardViewer viewer);

    public ElementIdentity getElementIdentity()
    {
        string spriteName = background.sprite.name;
        if (spriteName == "fire background")
            return ElementIdentity.Fire;
        if (spriteName == "earth background")
            return ElementIdentity.Earth;
        if (spriteName == "wind background")
            return ElementIdentity.Wind;
        if (spriteName == "water background")
            return ElementIdentity.Water;
        //Debug.Log("Sprite not registered or not set " + spriteName + ", " + nameText.text);
        return ElementIdentity.Nuetral;
    }

    public void setElementIdentity(ElementIdentity newId)
    {
        background.sprite = elementIdToSprite(newId);
    }

    private Sprite elementIdToSprite(ElementIdentity eId)
    {
        switch (eId)
        {
            case ElementIdentity.Fire:
                return fireBackground;
            case ElementIdentity.Water:
                return waterBackground;
            case ElementIdentity.Wind:
                return windBackground;
            case ElementIdentity.Earth:
                return earthBackground;
            case ElementIdentity.Nuetral:
                return nuetralBackground;
            default:
                return earthBackground;
        }
    }

    private string elementIdToString(ElementIdentity eId)
    {
        switch (eId)
        {
            case ElementIdentity.Fire:
                return "fire background";
            case ElementIdentity.Water:
                return "earth background";
            case ElementIdentity.Wind:
                return "wind background";
            case ElementIdentity.Earth:
                return "water background";
            case ElementIdentity.Nuetral:
                return "earth background";
            default:
                return "earth background";
        }
    }
}