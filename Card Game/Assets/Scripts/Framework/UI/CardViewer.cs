using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
    public Card sourceCard;
    public int sourceCardId { get; private set; }

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
    [SerializeField] private Sprite earthBackground;
    [SerializeField] private Sprite fireBackground;
    [SerializeField] private Sprite waterBackground;
    [SerializeField] private Sprite airBackground;
    [SerializeField] private Sprite nuetralBackground;

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
    public virtual void setCard(int cardId)
    {
        CardData data = ResourceManager.Get().getCardDataById(cardId);
        setCard(data);
    }
    public void setCard(CardData data)
    {
        sourceCardId = data.id;
        nameText.text = data.cardName;
        cardArt.sprite = data.art;
        background.sprite = getSpriteFromEId(data.elementalIdentity);

        switch (data)
        {
            case CreatureCardData creatureData:
                setToCard(creatureData);
                break;
            case StructureCardData structureData:
                setToCard(structureData);
                break;
            case SpellCardData spellData:
                setToCard(spellData);
                break;
        }

    }
    private void setToCard(CreatureCardData data)
    {
        hpGameObject.SetActive(true);
        attackGameObject.SetActive(true);
        goldGameObject.SetActive(true);
        manaGameObject.SetActive(false);
        manaLowerGameObject.SetActive(false);
        moveGameObject.SetActive(true);
        moveText.gameObject.SetActive(true);
        moveValueText.gameObject.SetActive(true);
        halfBodyText.gameObject.SetActive(true);
        fullBodyText.gameObject.SetActive(false);

        goldText.text = data.goldCost + "";
        hpText.text = data.health + "";
        attackText.text = data.attack + "";
        halfBodyText.text = data.effectText + "";
        moveValueText.text = data.movement + "";

        string tagsText = "";
        foreach (Card.Tag tag in data.tags)
        {
            tagsText += tag.ToString() + " ";
        }
        typeText.text = "Creature - " + tagsText;
    }
    private void setToCard(StructureCardData data)
    {
        hpGameObject.SetActive(true);
        attackGameObject.SetActive(false);
        goldGameObject.SetActive(true);
        fullBodyText.gameObject.SetActive(true);
        halfBodyText.gameObject.SetActive(false);
        manaGameObject.SetActive(false);
        moveGameObject.SetActive(false);

        goldText.text = data.goldCost + "";
        hpText.text = data.health + "";
        fullBodyText.text = data.effectText;

        string tagsText = "";
        foreach (Card.Tag tag in data.tags)
        {
            tagsText += tag.ToString() + " ";
        }
        typeText.text = "Structure - " + tagsText;
    }
    private void setToCard(SpellCardData data)
    {
        hpGameObject.SetActive(false);
        attackGameObject.SetActive(false);
        goldGameObject.SetActive(false);
        manaGameObject.SetActive(true);
        moveGameObject.SetActive(false);
        fullBodyText.gameObject.SetActive(true);
        halfBodyText.gameObject.SetActive(false);

        manaText1.text = data.manaCost + "";
        fullBodyText.text = data.effectText;
        string tagsText = "";
        foreach (Card.Tag tag in data.tags)
        {
            tagsText += tag.ToString() + " ";
        }
        typeText.text = "Spell - " + tagsText;
    }
    private Sprite getSpriteFromEId(Card.ElementIdentity element)
    {
        switch (element)
        {
            case Card.ElementIdentity.Fire:
                return fireBackground;
            case Card.ElementIdentity.Water:
                return waterBackground;
            case Card.ElementIdentity.Wind:
                return airBackground;
            case Card.ElementIdentity.Earth:
                return earthBackground;
            case Card.ElementIdentity.Nuetral:
                return nuetralBackground;
        }
        throw new Exception("wtf");
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
