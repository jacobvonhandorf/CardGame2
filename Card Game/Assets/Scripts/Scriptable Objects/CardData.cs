using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardData : ScriptableObject
{
    public int id;
    public string cardName;
    public Sprite art;
    public string effectText;
    public int manaCost;
    public int goldCost;
    public Card.ElementIdentity elementalIdentity;
    public List<Card.Tag> tags;
    public List<Keyword> keywords;
}
