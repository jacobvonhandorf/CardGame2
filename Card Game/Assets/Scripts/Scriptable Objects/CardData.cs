using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class CardData : ScriptableObject, IComparable<CardData>, ICanBeCardViewed
{
    public int id;
    public string cardName;
    public Sprite art;
    [TextArea(5, 20)]
    public string effectText;
    public int manaCost = 0;
    public int goldCost = 0;
    public Card.ElementIdentity elementalIdentity;
    public List<Card.Tag> tags;
    public List<Card.Tag> Tags => tags;
    public List<Keyword> keywords;
    public bool visibleInDeckBuilder = true;

    public int TotalCost { get { return Mathf.Clamp(manaCost, 0, 999999) + Mathf.Clamp(manaCost, 0, 999999); } }
    public Card.CardType CardType
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

    public abstract IHaveReadableStats ReadableStats { get; }
    public abstract string CardTypeString { get; }
    public Card AsCard => CardBuilder.Instance.BuildFromCardData(this);

    public int CompareTo(CardData other)
    {
        int elementCompareResult = elementalIdentity.CompareTo(other.elementalIdentity);
        if (elementCompareResult != 0)
            return elementCompareResult;
        int costCompareResult = TotalCost.CompareTo(other.TotalCost);
        if (costCompareResult != 0)
            return costCompareResult;
        // if cost and type are equal then go off names
        return cardName.CompareTo(other.cardName);
    }

    public string TypeText
    {
        get
        {
            if (Tags.Count == 0)
            {
                return CardTypeString;
            }
            else
            {
                string tagsText = "";
                foreach (Card.Tag tag in Tags)
                {
                    tagsText += tag.ToString() + " ";
                }
                return CardTypeString + " - " + tagsText;
            }
        }
    }

    public void OnAddedToViewer() { }
    public void OnRemovedFromViewer(CardViewer cardViewer) { }
}
