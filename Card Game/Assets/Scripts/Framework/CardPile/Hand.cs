using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Hand : CardPile
{
    private bool hidden = false;
    [SerializeField] private float desiredCardSeperation;

    private new void Awake()
    {
        base.Awake();
    }

    protected override void OnCardAdded(Card c)
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
        {
            /*
            if (GameManager.Get().activePlayer == handOwner)
                c.returnGraphicsAndCollidersToScene();
            else
                c.removeGraphicsAndCollidersFromScene();
                */
        }
        else if (GameManager.gameMode == GameManager.GameMode.online)
        {
            if (c.owner != NetInterface.Get().localPlayer)
                c.removeGraphicsAndCollidersFromScene();
            else
                c.returnGraphicsAndCollidersToScene();
        }
        c.returnGraphicsAndCollidersToScene(); // Cards know when to show themselves
        if (c is CreatureCard)
            (c as CreatureCard).SwapToCard();
        else if (c is StructureCard)
            (c as StructureCard).swapToCard();
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
        // move hand off screen
    }
}
