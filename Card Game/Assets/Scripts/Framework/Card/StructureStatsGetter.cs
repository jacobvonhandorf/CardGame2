using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StructureStatsGetter : CardStatsGetter
{
    private const float scalingCoefficient = 2.4f;
    private const float entireCardScaleCoefficient = .165f;

    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private SpriteRenderer friendOrFoeBorder;
    public TextMeshPro effectText; // must be public so HQ can set effect text

    public override void setCardViewer(CardViewer viewer)
    {
        viewer.gameObject.SetActive(true);

        // flip everything to active that needs to be active
        // and flip everything to inactive that should be inactive
        viewer.hpGameObject.SetActive(hpText.gameObject.activeInHierarchy);
        viewer.setGoldActive(goldText.gameObject.activeInHierarchy);
        viewer.setManaActive(manaText1.gameObject.activeInHierarchy);
        viewer.setManaLowerActive(manaText2.gameObject.activeInHierarchy);
        viewer.setFullBodyTextActive(true);
        viewer.setArmorActive(false);
        viewer.setAttackActive(false);
        viewer.setMoveActive(false);
        viewer.setHalfBodyTextActive(false);

        // set all values that need to be set
        viewer.hpText.text = hpText.text;
        viewer.goldText.text = goldText.text;
        viewer.manaText1.text = manaText1.text;
        viewer.manaText2.text = manaText2.text;
        viewer.fullBodyText.text = bodyText.text;
        viewer.nameText.text = nameText.text;
        viewer.typeText.text = typeText.text;

        // set all colors that need to be set. Will need to add more things here later probably
        viewer.hpText.color = hpText.color;
        viewer.goldText.color = goldText.color;
        viewer.manaText1.color = manaText1.color;
        viewer.manaText2.color = manaText2.color;

        //throw new System.NotImplementedException();
        viewer.background.sprite = background.sprite;
    }

    internal void setStructureStats(Structure structure)
    {
        structure.setBaseHealth(int.Parse(hpText.text));
        structure.setHealth(structure.getBaseHealth());
    }

    internal void setHealth(int health, int baseHealth)
    {
        hpText.text = "" + health;
        if (health > baseHealth)
            hpText.color = aboveBaseColor;
        else if (health < baseHealth)
            hpText.color = belowBaseColor;
        else
            hpText.color = Color.white;
    }

    public void switchBetweenStructureOrCard(StructureCard structureCard)
    {
        Debug.Log("Switch called");
        if (structureCard.isStructure)
        {
            swapToCard();
        }
        else
        {
            swapToStructure();
        }
        /*
        List<Transform> iconsToResize = new List<Transform>();
        iconsToResize.Add(hpText.transform.parent);

        Vector3 newIconScale;
        if (!structureCard.isStructure)
        {
            newIconScale = new Vector3(scalingCoefficient, scalingCoefficient, 1);
            foreach (Transform icon in iconsToResize)
                icon.localScale = newIconScale;

            Vector3 newRootScale = new Vector3(entireCardScaleCoefficient, entireCardScaleCoefficient, 1);
            cardRoot.localScale = newRootScale;
        }
        else
        {
            newIconScale = new Vector3(1, 1, 1);
            foreach (Transform icon in iconsToResize)
                icon.localScale = newIconScale;

            Vector3 newRootScale = new Vector3(.5f, .5f, 1);
            cardRoot.localScale = newRootScale;
        }*/
    }

    public void swapToStructure()
    {
        List<Transform> iconsToResize = new List<Transform>();
        iconsToResize.Add(hpText.transform.parent);

        Vector3 newIconScale = new Vector3(scalingCoefficient, scalingCoefficient, 1);
        Vector3 newRootScale = new Vector3(entireCardScaleCoefficient, entireCardScaleCoefficient, 1);

        StartCoroutine(resizeToStrcture(cardRoot, newRootScale, iconsToResize, newIconScale));
    }

    [SerializeField] private float resizeSpeed = .1f;
    [SerializeField] private float pauseBetweenResize = .5f;
    [SerializeField] private float iconResizeSpeed = 2f;
    private float timePaused = 0f;
    IEnumerator resizeToStrcture(Transform cardRoot, Vector3 newRootScale, List<Transform> iconsToEnlarge, Vector3 newIconScale)
    {
        // resize root
        while (Vector3.Distance(cardRoot.localScale, newRootScale) > 0.02f)
        {
            Debug.Log("Resizing root");
            cardRoot.localScale = Vector3.MoveTowards(cardRoot.localScale, newRootScale, resizeSpeed * Time.deltaTime);
            yield return null;
        }

        // pause
        while (timePaused < pauseBetweenResize)
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


    // called when a structure leaves the field and needs to act like a card again
    // if the topology of a card is changed this method may need to be changed
    public void swapToCard()
    {
        friendOrFoeBorder.gameObject.SetActive(false);

        List<Transform> iconsToResize = new List<Transform>();
        iconsToResize.Add(hpText.transform.parent);

        Vector3 newIconScale = new Vector3(1, 1, 1);
        foreach (Transform icon in iconsToResize)
        {
            icon.localScale = newIconScale;
        }

        Vector3 newRootScale = new Vector3(.5f, .5f, 1);
        cardRoot.localScale = newRootScale;
    }

    public Transform getRootTransform()
    {
        return cardRoot;
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

    public void updateAllStats(Structure s)
    {
        updateCosts(s.sourceCard);
        setHealth(s.getHealth(), s.getBaseHealth());
    }
}
