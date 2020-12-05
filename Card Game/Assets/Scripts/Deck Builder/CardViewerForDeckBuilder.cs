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
        //deckBeingBuilt.addCard(SourceCardId);
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

    public override void SetCard(int cardId)
    {
        base.SetCard(cardId);
        if (deckBeingBuilt == null)
            return;
        count = deckBeingBuilt.getCardAmount(cardId);
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
