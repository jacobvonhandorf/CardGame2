using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellStatsGetter : CardStatsGetter
{
    public override void setCardViewer(CardViewer cardViewer)
    {
        cardViewer.gameObject.SetActive(true);

        // flip everything to active that needs to be active
        // and flip everything to inactive that should be inactive
        cardViewer.setGoldActive(goldText.gameObject.activeInHierarchy);
        cardViewer.setManaActive(manaText1.gameObject.activeInHierarchy);
        cardViewer.setManaLowerActive(manaText2.gameObject.activeInHierarchy);
        cardViewer.setHpActive(false);
        cardViewer.setAttackActive(false);
        cardViewer.setMoveActive(false);
        cardViewer.setHalfBodyTextActive(false);
        cardViewer.setFullBodyTextActive(true);


        // set all values that need to be set
        cardViewer.goldText.text = goldText.text;
        cardViewer.typeText.text = typeText.text;
        cardViewer.nameText.text = nameText.text;
        cardViewer.fullBodyText.text = effectText.text;
        cardViewer.manaText1.text = manaText1.text;
        cardViewer.manaText2.text = manaText2.text;

        cardViewer.background.sprite = background.sprite;
        cardViewer.setCardArt(cardArt.sprite);
    }
}
