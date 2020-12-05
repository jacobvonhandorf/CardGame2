using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Graveyard : ViewableCardPile
{
    // private Card topCard; // most recent card added to grave. Will be shown on top.
    [SerializeField] private CardViewerForPiles topCardViewer;
    [SerializeField] private TextMeshPro cardCountText;
    private Card topCard;

    private void Start()
    {
        cardCountText.text = cardList.Count + "";
    }

    protected override string getPileName()
    {
        return "Graveyard";
    }

    protected override void onCardAdded(Card c)
    {
        if (c is CreatureCard)
            removeCreatureFromTile((c as CreatureCard).Creature);
        else if (c is StructureCard)
            removeStructureFromTile((c as StructureCard).structure);

        // remove the card from the scene
        c.removeGraphicsAndCollidersFromScene();

        // change top card
        topCard = c;

        // update the number of cards in grave text
        cardCountText.text = cardList.Count + "";

        // when cards are sent to grave they forget stat madifications
        c.resetToBaseStatsWithoutSyncing();
        // if network game mode need to sync creature stats here because the creature is set to not active so it won't sync itself
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            NetInterface.Get().SyncCardStats(c);
            if (c is CreatureCard)
                NetInterface.Get().SyncCreatureStats((c as CreatureCard).Creature);
            else if (c is StructureCard)
                NetInterface.Get().SyncStructureStats((c as StructureCard).structure);
        }

        // change image being displayed
        topCardViewer.SetCard(c);
    }

    private void removeStructureFromTile(Structure structure)
    {
        if (structure.Tile != null)
        {
            structure.Tile.structure = null;
            structure.Tile = null;
        }
    }

    private void removeCreatureFromTile(Creature creature)
    {
        if (creature.Tile != null)
        {
            creature.Tile.creature = null;
            creature.Tile = null;
        }
    }

    protected override void onCardRemoved(Card c)
    {
        // if the card being removed is the being displayed on top of the grave then update the card
        if (c == topCard)
        {
            if (cardList.Count == 0) // if there are no cards left then hide the top card
            {
                topCardViewer.gameObject.SetActive(false);
            }
            else // else set the top card to the last one added to grave (highest index in list)
            {
                topCardViewer.SetCard(cardList[cardList.Count - 1]);
            }
        }
        cardCountText.text = cardList.Count + "";
    }
}
