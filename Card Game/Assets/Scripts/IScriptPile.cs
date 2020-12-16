using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public interface IScriptPile
{
    IReadOnlyList<Card> CardList { get; }
    UnityEvent NumCardsChanged { get; }
    List<Card> GetAllCardsWithTag(Tag tag);
    List<Card> GetAllCardsWithType(CardType type);
    List<Card> GetAllCardWithTagAndType(Tag tag, CardType type);
    List<Card> GetAllCardsWithLessThanOrEqualCost(int cost);
    List<Card> GetAllCardsWithinCostRange(int min, int max);
}

public interface IScriptDeck : IScriptPile
{
    void Shuffle();
}
