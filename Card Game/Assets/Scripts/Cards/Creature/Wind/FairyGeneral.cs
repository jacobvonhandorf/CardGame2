using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class FairyGeneral : Creature
{
    public override int cardId => 49;
    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Fairy };

    private void OnEnable()
    {
        GameEvents.E_CreaturePlayed += GameEvents_E_CreaturePlayed;
    }
    private void OnDisable()
    {
        GameEvents.E_CreaturePlayed -= GameEvents_E_CreaturePlayed;
    }

    private void GameEvents_E_CreaturePlayed(object sender, GameEvents.CreaturePlayedArgs e)
    {
        if (e.creature.controller == controller && e.creature != this)
        {
            addAttack(1);
            addHealth(1);
        }
    }
}
*/