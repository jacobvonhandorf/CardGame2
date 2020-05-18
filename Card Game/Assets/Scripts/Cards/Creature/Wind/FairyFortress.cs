using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyFortress : Structure, Effect, CanReceivePickedCards
{
    public override bool canDeployFrom()
    {
        return false;
    }

    public override bool canWalkOn()
    {
        return false;
    }

    public override Effect getEffect()
    {
        return this;
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> returnList = new List<Card.Tag>();
        returnList.Add(Card.Tag.Fairy);
        return returnList;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (sourcePlayer.GetActions() <= 0)
        {
            GameManager.Get().showToast("You do not have enough actions to use this effect");
            return;
        }
        List<Card> controlledCreatureCards = new List<Card>();
        foreach (Creature c in controller.getAllControlledCreatures())
            controlledCreatureCards.Add(c.sourceCard);
        if (controlledCreatureCards.Count <= 0)
        {
            GameManager.Get().showToast("You must control a creature to activate this effect");
            return;
        }


        GameManager.Get().queueCardPickerEffect(controller, controlledCreatureCards, this, 1, 1, "Select your creature to bounce");
    }

    public void receiveCardList(List<Card> cardList)
    {
        Creature firstCreature = (cardList[0] as CreatureCard).creature;
        List<Card> pickableCards = new List<Card>();
        foreach (Creature c in GameManager.Get().getOppositePlayer(controller).getAllControlledCreatures())
            pickableCards.Add(c.sourceCard);
        if (pickableCards.Count <= 0)
        {
            GameManager.Get().showToast("Your opponent must control a creature for you to activate this effect");
            return;
        }

        GameManager.Get().queueCardPickerEffect(controller, pickableCards, new EffectPart2(firstCreature), 1, 1, "Select opponent's creature to bounce");
    }

    public override int getCardId()
    {
        return 50;
    }

    private class EffectPart2 : CanReceivePickedCards
    {
        Creature firstCreature;

        public EffectPart2(Creature firstCreature)
        {
            this.firstCreature = firstCreature;
        }

        public void receiveCardList(List<Card> cardList)
        {
            Creature secondCreature = (cardList[0] as CreatureCard).creature;
            firstCreature.controller.subtractActions(1);
            firstCreature.bounce();
            secondCreature.bounce();

        }
    }
}
