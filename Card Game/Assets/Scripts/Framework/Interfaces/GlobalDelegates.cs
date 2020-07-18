using System;
using System.Collections.Generic;

public delegate void CardListHandler(List<Card> pickedCards);
public delegate void TileHandler(Tile tile);
public delegate void OptionBoxHandler(int selectedIndex, string selectedOption);
public delegate void XValueHandler(int x);

#region Events
public delegate void onDefendHandler(OnDefendArgs e);
public class OnDefendArgs : EventArgs { public Creature attacker { get; set; } }
public delegate void onDamagedHandler(OnDefendArgs e);
public class OnDamagedArgs : EventArgs { public Card source { get; set; } }
#endregion
