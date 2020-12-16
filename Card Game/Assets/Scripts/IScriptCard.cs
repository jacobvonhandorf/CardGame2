using UnityEngine;
using System.Collections;

public interface IScriptCard
{
    int GoldCost { get; set; }
    int ManaCost { get; set; }
    int BaseManaCost { get; set; }
    int BaseGoldCost { get; set; }
    int TotalCost { get; }
    CardPile CardPile { get; }
    Player Owner { get; }
    ElementIdentity ElementalId { get; set; }

    void MoveToCardPile(CardPile newPile, Card source);
    void ShowInEffectsView();
}
