using System.Collections.Generic;

public delegate void CardListHandler(List<Card> pickedCards);
public delegate void TileHandler(Tile tile);
public delegate void OptionBoxHandler(int selectedIndex, string selectedOption);
public delegate void XValueHandler(int x);
