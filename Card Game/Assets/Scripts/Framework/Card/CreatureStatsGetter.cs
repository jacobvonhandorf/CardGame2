using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CreatureStatsGetter : CardStatsGetter
{
    private const float scalingCoefficient = 2.4f;
    private const float entireCardScaleCoefficient = .165f;

    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private TextMeshPro attackText;
    [SerializeField] private TextMeshPro armorText;
    [SerializeField] private TextMeshPro moveValueText;
    [SerializeField] private TextMeshPro moveText;

    [SerializeField] private TextMeshPro hasActedTextIndicator;
    [SerializeField] private SpriteRenderer friendOrFoeBorder;

    //[SerializeField] private CounterController counterController;

    // called to set creature stats on creature cards when they are created
    public void setCreatureStats(Creature creature)
    {
        creature.baseHealth = int.Parse(hpText.text);
        creature.baseAttack = int.Parse(attackText.text);
        //creature.baseDefense = int.Parse(armorText.text);
        creature.baseMovement = int.Parse(moveValueText.text);
        creature.cardName = nameText.text;

        // reset to base stats manually
        creature.resetToBaseStatsWithoutSyncing();
    }

    public void switchBetweenCreatureOrCard(CreatureCard card)
    {
        if (card.isCreature)
        {
            swapToCard(card);
        }
        else
        {
            swapToCreature(card);
        }
    }

    // Called when a card is played to a creature.
    // if the topology of a card is changed this method may need to be changed
    public void swapToCreature(CreatureCard card)
    {

        List<Transform> iconsToResize = new List<Transform>();
        iconsToResize.Add(hpText.transform.parent);
        iconsToResize.Add(armorText.transform.parent);
        iconsToResize.Add(attackText.transform.parent);

        Vector3 newIconScale = new Vector3(scalingCoefficient, scalingCoefficient, 1);
        Vector3 newRootScale = new Vector3(entireCardScaleCoefficient, entireCardScaleCoefficient, 1);

        StartCoroutine(resizeToCreature(cardRoot, newRootScale, iconsToResize, newIconScale));
    }

    [SerializeField] private float resizeSpeed = .1f;
    [SerializeField] private float pauseBetweenResize = .5f;
    [SerializeField] private float iconResizeSpeed = 2f;
    private float timePaused = 0f;
    IEnumerator resizeToCreature(Transform cardRoot, Vector3 newRootScale, List<Transform> iconsToEnlarge, Vector3 newIconScale)
    {
        // resize root
        while (Vector3.Distance(cardRoot.localScale, newRootScale) > 0.02f)
        {
            cardRoot.localScale = Vector3.MoveTowards(cardRoot.localScale, newRootScale, resizeSpeed * Time.deltaTime);
            yield return null;
        }

        // pause
        while (timePaused <  pauseBetweenResize)
        {
            timePaused += Time.deltaTime;
        }
        timePaused = 0f;

        // resize icons
        while (Vector3.Distance(iconsToEnlarge[0].localScale, newIconScale) > 0.02f)
        {
            foreach (Transform t in iconsToEnlarge)
            {
                t.localScale = Vector3.MoveTowards(t.localScale, newIconScale, iconResizeSpeed * Time.deltaTime);
            }
            //cardRoot.localScale = Vector3.MoveTowards(cardRoot.localScale, newRootScale, iconResizeSpeed * Time.deltaTime);
            yield return null;
        }
    }


    // called when a creature leaves the field and needs to act like a card again
    // if the topology of a card is changed this method may need to be changed
    public void swapToCard(CreatureCard card)
    {
        friendOrFoeBorder.gameObject.SetActive(false);

        List<Transform> iconsToResize = new List<Transform>();
        iconsToResize.Add(hpText.transform.parent);
        iconsToResize.Add(armorText.transform.parent);
        iconsToResize.Add(attackText.transform.parent);

        Vector3 newIconScale = new Vector3(1, 1, 1);
        foreach(Transform icon in iconsToResize)
        {
            icon.localScale = newIconScale;
        }

        Vector3 newRootScale = new Vector3(.5f, .5f, 1);
        cardRoot.localScale = newRootScale;
    }

    // sets the card viewer to the card attached to this stats getter
    // if the topology of cards change this method will need to be changed
    public override void setCardViewer(CardViewer cardViewer)
    {
        cardViewer.gameObject.SetActive(true);

        // flip everything to active that needs to be active
        // and flip everything to inactive that should be inactive
        cardViewer.hpGameObject.SetActive(hpText.gameObject.activeInHierarchy);
        cardViewer.setArmorActive(armorText.gameObject.activeInHierarchy);
        cardViewer.setAttackActive(attackText.gameObject.activeInHierarchy);
        cardViewer.setGoldActive(goldText.gameObject.activeInHierarchy);
        cardViewer.setManaActive(manaText1.gameObject.activeInHierarchy);
        cardViewer.setManaLowerActive(manaText2.gameObject.activeInHierarchy);
        cardViewer.setMoveActive(true);
        cardViewer.setHalfBodyTextActive(true);
        cardViewer.setFullBodyTextActive(false);

        // set all values that need to be set
        cardViewer.hpText.text = hpText.text;
        cardViewer.armorText.text = armorText.text;
        cardViewer.attackText.text = attackText.text;
        cardViewer.goldText.text = goldText.text;
        cardViewer.moveText.text = moveText.text;
        cardViewer.moveValueText.text = moveValueText.text;
        cardViewer.typeText.text = typeText.text;
        cardViewer.nameText.text = nameText.text;
        cardViewer.halfBodyText.text = bodyText.text;

        // set all colors that need to be set. Will need to add more things here later probably
        cardViewer.hpText.color = hpText.color;
        cardViewer.attackText.color = attackText.color;
        cardViewer.goldText.color = goldText.color;

        if (manaText1.gameObject.activeInHierarchy)
        {
            cardViewer.manaText1.text = manaText1.text;
            cardViewer.manaText1.color = manaText1.color;
        }
        if (manaText2.gameObject.activeInHierarchy)
        {
            cardViewer.manaText2.text = manaText2.text;
            cardViewer.manaText2.text = manaText2.text;
        }

        // set sprites to be equivalent
        cardViewer.background.sprite = background.sprite;
        cardViewer.setCardArt(cardArt.sprite);
    }

    public void setTextSortingLayer(SpriteLayers layer)
    {
        int layerId = SortingLayer.NameToID(layer.Value);
        hpText.sortingLayerID = layerId;
        attackText.sortingLayerID = layerId;
        armorText.sortingLayerID = layerId;
        moveValueText.sortingLayerID = layerId;
        moveText.sortingLayerID = layerId;
        bodyText.sortingLayerID = layerId;
        typeText.sortingLayerID = layerId;
        goldText.sortingLayerID = layerId;
        nameText.sortingLayerID = layerId;

        // might now be active but set anyways
        manaText1.sortingLayerID = layerId;
        manaText2.sortingLayerID = layerId;
    }

    public void updateHasActedIndicator(bool hasDoneActionThisTurn, bool hasMovedThisTurn)
    {
        //if (hasDoneActionThisTurn == hasMovedThisTurn)
            //hasActedTextIndicator.text = "";
        //else if (hasDoneActionThisTurn && !hasMovedThisTurn)
        //    hasActedTextIndicator.text = "M";
        if (!hasDoneActionThisTurn && hasMovedThisTurn)
            hasActedTextIndicator.text = "A";
        else
            hasActedTextIndicator.text = "";
    }

    internal void setHealth(int value, int baseValue)
    {
        hpText.text = "" + value;
        if (value > baseValue)
            hpText.color = aboveBaseColor;
        else if (value < baseValue)
            hpText.color = belowBaseColor;
        else
            hpText.color = Color.white;
    }
    internal void setArmor(int value, int baseValue)
    {
        armorText.text = "" + value;
        if (value > baseValue)
            armorText.color = aboveBaseColor;
        else if (value < baseValue)
            armorText.color = belowBaseColor;
        else
            armorText.color = Color.white;
    }
    internal void setAttack(int value, int baseValue)
    {
        attackText.text = "" + value;
        if (value > baseValue)
            attackText.color = aboveBaseColor;
        else if (value < baseValue)
            attackText.color = belowBaseColor;
        else
            attackText.color = Color.white;
    }
    internal void setMovement(int value, int baseValue)
    {
        moveValueText.text = "" + value;
        /*
        if (value > baseValue)
            moveValueText.color = aboveBaseColor;
        else if (value < baseValue)
            moveValueText.color = belowBaseColor;
        else
            moveValueText.color = Color.white;
            */
    }

    // used to update the friend or foe border
    public void setAsAlly(bool isAlly)
    {
        friendOrFoeBorder.gameObject.SetActive(true);
        if (isAlly)
            friendOrFoeBorder.color = Color.blue;
        else
            friendOrFoeBorder.color = Color.red;
    }

    public void updateAllStats(Creature c)
    {
        updateCosts(c.sourceCard);
        setAttack(c.getAttack(), c.baseAttack);
        setHealth(c.getHealth(), c.baseHealth);
        setMovement(c.getMovement(), c.baseMovement);

        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            setAsAlly(c.controller == NetInterface.Get().getLocalPlayer());
        }
        else
        {
            throw new NotImplementedException();
        }

    }

    //public CounterController getCounterController() => counterController;
}
