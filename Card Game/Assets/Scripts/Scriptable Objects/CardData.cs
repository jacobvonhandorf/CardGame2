using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CardData : ScriptableObject, IComparable<CardData>
{
    public int id;
    public string cardName;
    public Sprite art;
    [TextArea(5, 20)]
    public string effectText;
    public int manaCost;
    public int goldCost;
    public Card.ElementIdentity elementalIdentity;
    public List<Card.Tag> tags;
    public List<Keyword> keywords;
    public bool visibleInDeckBuilder = true;

    [HideInInspector] public int totalCost
    {
        get
        {
            return manaCost + goldCost;
        }
    }
    [HideInInspector] public Card.CardType cardType
    {
        get
        {
            switch (this)
            {
                case CreatureCardData d:
                    return Card.CardType.Creature;
                case StructureCardData d:
                    return Card.CardType.Structure;
                case SpellCardData d:
                    return Card.CardType.Spell;
            }
            throw new Exception("wtf");
        }
    }

    public int CompareTo(CardData other)
    {
        int elementCompareResult = elementalIdentity.CompareTo(other.elementalIdentity);
        if (elementCompareResult != 0)
            return elementCompareResult;
        int costCompareResult = totalCost.CompareTo(other.totalCost);
        if (costCompareResult != 0)
            return costCompareResult;
        // if cost and type are equal then go off names
        return cardName.CompareTo(other.cardName);
    }
}
