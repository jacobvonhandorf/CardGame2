using System;
using System.Collections.Generic;

public delegate void CardListHandler(List<Card> pickedCards);
public delegate void TileHandler(Tile tile);
public delegate void OptionBoxHandler(int selectedIndex, string selectedOption);
public delegate void XValueHandler(int x);
public delegate void EmptyHandler();

#region Events
public delegate void OnDefendHandler(OnDefendArgs e);
public class OnDefendArgs : EventArgs { public Creature attacker { get; set; } }
public delegate void OnDamagedHandler(OnDefendArgs e);
public class OnDamagedArgs : EventArgs { public Card source { get; set; } }
public delegate void StructureEventHandler(StructureEventArgs e);
public class StructureEventArgs : EventArgs
{
    public Card card { get; set; }
    public Structure structure { get; set; }
}
#endregion
