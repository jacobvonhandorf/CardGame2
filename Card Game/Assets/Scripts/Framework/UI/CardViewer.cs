using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
    public Card sourceCard;

    public GameObject hpGameObject;
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
    public TextMeshPro moveValueText;
    public TextMeshPro moveText;
    public TextMeshPro halfBodyText; //used by creatures
    public TextMeshPro fullBodyText; // used by spells and structures
    public TextMeshPro typeText;

    public SpriteRenderer background;
    public SpriteRenderer cardArt;

    [SerializeField] private ToolTipBox toolTipPrefab;

    private void OnDestroy()
    {
        if (sourceCard != null)
            sourceCard.removeFromCardViewer(this);
    }

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

    public virtual void setCard(Card c)
    {
        if (sourceCard != null)
            sourceCard.removeFromCardViewer(this);
        c.addToCardViewer(this);
        sourceCard = c;
    }

    private List<ToolTipBox> toolTips = new List<ToolTipBox>();
    private static Vector3 toolTipOffset = new Vector3(3.7f, 2.1f, 0);
    //private static float toolTipTOffsetPerBox = 1.5f;
    private static Vector3 toolTipTOffsetPerBox = new Vector3(0, -1.74f, 0);
    public void showToolTips()
    {
        showToolTips(sourceCard.toolTipInfos);
    }
    public void showToolTips(List<ToolTipInfo> toolTipInfos)
    {
        //For some reason (probably Unity being bad) log statements in here break things, so don't do that
        int i = 0;
        foreach (ToolTipInfo info in toolTipInfos)
        {
            
            Vector3 position = toolTipOffset + (toolTipTOffsetPerBox * i);
            ToolTipBox box = Instantiate(toolTipPrefab, transform);
            box.transform.localPosition = position;
            box.setup(info.headerText, info.descriptionText);
            toolTips.Add(box);
            i++;
            
        }
        
    }
    public void clearToolTips()
    {
        foreach (ToolTipBox box in toolTips)
        {
            Destroy(box.gameObject);
        }
        toolTips.Clear();
    }
}
