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
    [SerializeField] private TextMeshPro moveValueText;
    [SerializeField] private TextMeshPro moveText;

    [SerializeField] private TextMeshPro hasActedTextIndicator;
    [SerializeField] private SpriteRenderer friendOrFoeBorder;

    public override void setCardViewer(CardViewer viewer)
    {
        throw new NotImplementedException();
    }

    // sets the card viewer to the card attached to this stats getter
    // if the topology of cards change this method will need to be changed
    /*
    public override void setCardViewer(CardViewer cardViewer)
    {
        cardViewer.gameObject.SetActive(true);

        // flip everything to active that needs to be active
        // and flip everything to inactive that should be inactive
        cardViewer.hpGameObject.SetActive(hpText.gameObject.activeInHierarchy);
        cardViewer.setAttackActive(attackText.gameObject.activeInHierarchy);
        cardViewer.setGoldActive(goldText.gameObject.activeInHierarchy);
        cardViewer.setManaActive(manaText1.gameObject.activeInHierarchy);
        cardViewer.setManaLowerActive(manaText2.gameObject.activeInHierarchy);
        cardViewer.setMoveActive(true);
        cardViewer.setHalfBodyTextActive(true);
        cardViewer.setFullBodyTextActive(false);

        // set all values that need to be set
        cardViewer.hpText.text = hpText.text;
        cardViewer.attackText.text = attackText.text;
        cardViewer.goldText.text = goldText.text;
        cardViewer.moveText.text = moveText.text;
        cardViewer.moveValueText.text = moveValueText.text;
        cardViewer.typeText.text = typeText.text;
        cardViewer.nameText.text = nameText.text;
        cardViewer.halfBodyText.text = effectText.text;

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
    */

    /*
public void setTextSortingLayer(SpriteLayers layer)
{
    int layerId = SortingLayer.NameToID(layer.Value);
    hpText.sortingLayerID = layerId;
    attackText.sortingLayerID = layerId;
    moveValueText.sortingLayerID = layerId;
    moveText.sortingLayerID = layerId;
    effectText.sortingLayerID = layerId;
    typeText.sortingLayerID = layerId;
    goldText.sortingLayerID = layerId;
    nameText.sortingLayerID = layerId;

    // might now be active but set anyways
    manaText1.sortingLayerID = layerId;
    manaText2.sortingLayerID = layerId;
}
*/

    public void updateHasActedIndicator(bool hasDoneActionThisTurn, bool hasMovedThisTurn)
    {
        if (!hasDoneActionThisTurn && hasMovedThisTurn)
            hasActedTextIndicator.text = "A";
        else
            hasActedTextIndicator.text = "";
    }
}
