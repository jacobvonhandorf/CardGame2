using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Hand : CardPile
{
    public float maxWidth;
    public float spaceBetweenCards;
    public int maxCardsBeforeSquishingHand = 6;
    private bool hidden = false;
    [SerializeField] private TextMeshPro cardCountText;
    [SerializeField] private Player handOwner;

    private void Start()
    {
        // on start add all cards in players hand to cardList
        cardList = GetComponentsInChildren<Card>().ToList();
        foreach (Card c in cardList)
        {
            c.owner = handOwner;
            c.moveToCardPile(this, null);
        }
        resetCardPositions();

        if (cardCountText != null)
            cardCountText.text = cardList.Count + "";
    }

    public void resetCardPositions()
    {
        if (hidden)
            return;
        int totalCards = cardList.Count;
        int index = 0;
        foreach(Card c in cardList)
        {
            Transform t = c.transform; // the transform that needs to be moved

            // calculate new position
            Vector3 newPosition = new Vector3();
            if (totalCards > maxCardsBeforeSquishingHand)
                newPosition.x = transform.position.x + maxWidth / totalCards * index;
            else
                newPosition.x = transform.position.x + index * spaceBetweenCards;

            newPosition.y = transform.position.y;
            newPosition.z = 1 - .01f * index;

            // move card
            c.moveTo(newPosition);
            c.positionInHand = newPosition;

            // change sorting layer
            c.setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle, index * 10);

            index++;
        }
    }

    protected override void onCardAdded(Card c)
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
        {
            if (GameManager.Get().activePlayer == handOwner)
                c.returnGraphicsAndCollidersToScene();
            else
                c.removeGraphicsAndCollidersFromScene();
        }
        else if (GameManager.gameMode == GameManager.GameMode.online)
        {
            if (c.owner != NetInterface.Get().getLocalPlayer())
            {
                c.removeGraphicsAndCollidersFromScene();
            }
            else
            {
                c.returnGraphicsAndCollidersToScene();
            }
        }
        c.returnGraphicsAndCollidersToScene(); // Cards know when to show themselves
        if (c is CreatureCard)
            (c as CreatureCard).swapToCard();
        else if (c is StructureCard)
            (c as StructureCard).swapToCard();
        c.setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);
        resetCardPositions();
        if (cardCountText != null)
            cardCountText.text = cardList.Count + "";
    }

    protected override void onCardRemoved(Card c)
    {
        cardCountText.text = cardList.Count + "";
        resetCardPositions();
    }

    public void printCardList()
    {
        Debug.Log("Printing cards in hand");
        foreach(Card c in cardList)
        {
            Debug.Log(c.transform.name);
        }
    }

    public void show()
    {
        if (GameManager.gameMode == GameManager.GameMode.online)
            return;
        hidden = false;
        foreach (Card c in cardList)
        {
            c.returnGraphicsAndCollidersToScene();
            //resetCardPositions();
            //c.setSpriteMaskInteraction(SpriteMaskInteraction.None);
        }
    }

    public void hide()
    {
        if (GameManager.gameMode == GameManager.GameMode.online)
            return;
        hidden = true;
        foreach (Card c in cardList)
        {
            c.removeGraphicsAndCollidersFromScene();
            //c.setSpriteMaskInteraction(SpriteMaskInteraction.VisibleInsideMask);
        }
    }
}
