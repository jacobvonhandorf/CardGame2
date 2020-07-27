using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Engineer : Creature
{
    public const int CARD_ID = 61;
    public override int cardId => CARD_ID;
    public CreatureCardData data;
}
