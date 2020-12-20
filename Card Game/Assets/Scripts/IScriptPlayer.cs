using UnityEngine;
using System.Collections;

public interface IScriptPlayer
{
    IScriptPile Hand { get; }
    IScriptPile Graveyard { get; }
    IScriptDeck Deck { get; }
    int Gold { get; set; }
    int Mana { get; set; }
    int Actions { get; set; }
    int GoldPerTurn { get; set; }
    int ManaPerTurn { get; set; }
    int ActionsPerTurn { get; set; }
    int NumSpellsCastThisTurn { get; }
    int NumCreaturesThisTurn { get; }
    int NumStructuresThisTurn { get; }

    void DrawCard();
    void DrawCards(int amount);
}
