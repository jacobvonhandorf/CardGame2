using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardViewerForDeckBuilder : CardViewer
{
    public DeckBuilderDeck deckBeingBuilt;
    [SerializeField] private TextMeshPro countText;
    [SerializeField] private GameObject countGameObject;
    private int count = 0;

    private void OnMouseUpAsButton()
    {
        deckBeingBuilt.addCard(sourceCard);
    }

    public void incrementCountText()
    {
        count++;
        countGameObject.SetActive(true);
        countText.text = "x" + count;
    }

    public void decrementCountText()
    {
        count--;
        if (count <= 0)
            countGameObject.SetActive(false);
        else
            countText.text = "x" + count;
    }

    public override void setCard(Card c)
    {
        base.setCard(c);
        if (deckBeingBuilt == null)
            return;
        count = deckBeingBuilt.getCardAmount(c);
        if (count > 0)
        {
            countGameObject.SetActive(true);
            countText.text = "x" + count;
        }
        else
        {
            countGameObject.SetActive(false);
        }
    }
}
