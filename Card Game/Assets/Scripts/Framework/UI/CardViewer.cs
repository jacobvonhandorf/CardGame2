using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
    public Card sourceCard;

    public GameObject hpGameObject;
    public GameObject armorGameObject;
    public GameObject attackGameObject;
    public GameObject goldGameObject;
    public GameObject manaGameObject;
    public GameObject manaLowerGameObject;
    public GameObject moveGameObject;

    public TextMeshPro manaText1;
    public TextMeshPro manaText2;
    public TextMeshPro goldText;
    public TextMeshPro nameText;
    public TextMeshPro hpText;
    public TextMeshPro attackText;
    public TextMeshPro armorText;
    public TextMeshPro moveValueText;
    public TextMeshPro moveText;
    public TextMeshPro halfBodyText; //used by creatures
    public TextMeshPro fullBodyText; // used by spells and structures
    public TextMeshPro typeText;

    public SpriteRenderer background;
    public SpriteRenderer cardArt;

    public void setMoveActive(bool active)
    {
        moveGameObject.SetActive(active);
    }
    public void setHalfBodyTextActive(bool active)
    {
        halfBodyText.gameObject.SetActive(active);
    }
    public void setFullBodyTextActive(bool active)
    {
        fullBodyText.gameObject.SetActive(active);
    }
    public void setHpActive(bool active)
    {
        hpGameObject.SetActive(active);
    }
    public void setArmorActive(bool active)
    {
        armorGameObject.SetActive(active);
    }
    public void setAttackActive(bool active)
    {
        attackGameObject.SetActive(active);
    }
    public void setGoldActive(bool active)
    {
        goldGameObject.SetActive(active);
    }
    public void setManaActive(bool active)
    {
        manaGameObject.SetActive(active);
    }
    public void setManaLowerActive(bool active)
    {
        manaLowerGameObject.SetActive(active);
    }
    public void setCardArt(Sprite newSprite)
    {
        cardArt.sprite = newSprite;
    }

    public void setCard(Card c)
    {
        if (sourceCard != null)
            sourceCard.removeFromCardViewer(this);
        c.addToCardViewer(this);
        sourceCard = c;
    }
}
