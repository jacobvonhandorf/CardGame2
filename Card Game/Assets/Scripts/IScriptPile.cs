using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public interface IScriptPile
{
    IReadOnlyList<IScriptCard> CardList { get; }
    UnityEvent NumCardsChanged { get; }
    /*
    List<IScriptCard> GetAllCardsWithTag(Tag tag);
    List<IScriptCard> GetAllCardsWithType(CardType type);
    List<IScriptCard> GetAllCardWithTagAndType(Tag tag, CardType type);
    List<IScriptCard> GetAllCardsWithLessThanOrEqualCost(int cost);
    List<IScriptCard> GetAllCardsWithinCostRange(int min, int max);
    */
}

public interface IScriptDeck : IScriptPile
{
    void Shuffle();
}
