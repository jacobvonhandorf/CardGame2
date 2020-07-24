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
    [SerializeField] protected Color aboveBaseColor;
    [SerializeField] protected Color belowBaseColor;


    public void setCardCosts(Card card)
    {
        if (goldText.gameObject.activeInHierarchy)
        {
            card.setGoldCost(int.Parse(goldText.text));
            card.setBaseGoldCost(card.getGoldCost());
        }
        else
        {
            card.setGoldCost(0);
            card.setBaseGoldCost(card.getGoldCost());
        }
        if (manaText1.gameObject.activeInHierarchy && manaText2.gameObject.activeInHierarchy)
        {
            throw new Exception(nameText.text + " has two active mana costs");
        }
        if (manaText1.gameObject.activeInHierarchy)
        {
            card.setManaCost(int.Parse(manaText1.text));
            card.setBaseManaCost(card.getManaCost());
        }
        else if (manaText2.gameObject.activeInHierarchy)
        {
            card.setManaCost(int.Parse(manaText1.text));
            card.setBaseManaCost(card.getManaCost());
        }
        else
        {
            card.setManaCost(0);
            card.setBaseManaCost(card.getManaCost());
        }
    }

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

    internal void setGoldCost(int newCost, int baseCost)
    {
        if (goldText.gameObject.activeInHierarchy)
        {
            goldText.text = "" + newCost;
            if (newCost > baseCost)
                goldText.color = aboveBaseColor;
            else if (newCost < baseCost)
                goldText.color = belowBaseColor;
            else
                goldText.color = Color.white;
        }
    }

    internal void setManaCost(int newCost, int baseCost)
    {
        if (manaText1.gameObject.activeInHierarchy)
        {
            manaText1.text = "" + newCost;
            if (newCost < baseCost)
                manaText1.color = aboveBaseColor;
            else if (newCost > baseCost)
                manaText1.color = belowBaseColor;
            else
                manaText1.color = Color.white;
        }
        else if (manaText2.gameObject.activeInHierarchy)
        {
            manaText2.text = "" + newCost;
            if (newCost < baseCost)
                manaText2.color = aboveBaseColor;
            else if (newCost > baseCost)
                manaText2.color = belowBaseColor;
            else
                manaText2.color = Color.white;
        }
    }

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
                return earthBackground;
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

    public void updateCosts(Card c)
    {
        setGoldCost(c.getGoldCost(), c.getBaseGoldCost());
        setManaCost(c.getManaCost(), c.getBaseManaCost());
    }
}