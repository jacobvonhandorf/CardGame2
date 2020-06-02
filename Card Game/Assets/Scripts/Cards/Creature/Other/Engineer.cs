using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Engineer : Creature
{
    public const int CARD_ID = 61;

    private new void Awake()
    {
        base.Awake();
        addKeyword(Card.CardKeywords.Quick);
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override Effect getEffect()
    {
        return new EngineerEffect(controller, this);
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    private class EngineerEffect : SingleTileTargetEffect
    {
        private Player owner;
        private Engineer engineer;

        public EngineerEffect(Player owner, Engineer engineer)
        {
            this.owner = owner;
            this.engineer = engineer;
        }

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            List<Tile> returnList = GameManager.Get().getLegalStructurePlacementTiles(sourcePlayer);
            returnList.RemoveAll(t => t.getDistanceTo(sourceTile) > 1);
            returnList.RemoveAll(t => t.creature != null); // remove tiles with creatures
            return returnList;
        }

        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            if (engineer.hasCounter(Counters.build) <= 0)
            {
                GameManager.Get().showToast("Your engineer has no build counters left");
                return;
            }
            List<string> options = new List<string>();
            options.Add("Market");
            options.Add("Altar");
            options.Add("Tower");

            GameManager.Get().queueOptionSelectBoxEffect(options, new EngineerOptionHandler(targetTile, owner), "Select which structure you would like to place", true, engineer.controller);
            engineer.removeCounters(Counters.build, 1);
            engineer.hasDoneActionThisTurn = true;
            if (!engineer.hasMovedThisTurn)
                engineer.controller.subtractActions(1);
            engineer.updateHasActedIndicators();
        }

        public bool canBeCancelled()
        {
            return true;
        }

        public class EngineerOptionHandler : OptionBoxHandler
        {
            private Tile targetTile;
            private Player controller;

            public EngineerOptionHandler(Tile targetTile, Player controller)
            {
                this.targetTile = targetTile;
                this.controller = controller;
            }

            public void receiveOptionBoxSelection(int selectedOptionIndex, string selectedOption)
            {
                StructureCard structureToPlace = null;
                if (selectedOptionIndex == 0)
                    structureToPlace = GameManager.Get().createCardById(Market.CARD_ID, controller) as StructureCard;
                if (selectedOptionIndex == 1)
                    structureToPlace = GameManager.Get().createCardById(Altar.CARD_ID, controller) as StructureCard;
                //GameManager.Get().createStructureOnTile("Altar", targetTile, controller);
                if (selectedOptionIndex == 2)
                    structureToPlace = GameManager.Get().createCardById(CommandTower.CARD_ID, controller) as StructureCard;
                GameManager.Get().createStructureOnTile(structureToPlace.structure, targetTile, controller, structureToPlace);
            }
        }
    }
}
