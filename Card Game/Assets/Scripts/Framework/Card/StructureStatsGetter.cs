using System;
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
        Debug.Log("Structure stats on initialization = " + int.Parse(hpText.text));
        structure.setBaseHealth(int.Parse(hpText.text));
        structure.setHealth(structure.getBaseHealth());
    }

    internal void setHealth(int health, int baseHealth)
    {
        Debug.Log("s health " + health + "/" + baseHealth);
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
        }
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
