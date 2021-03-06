﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// mostly exists so that CardVis can change what card type they're displaying
public class CardTypeChangeListener : StatChangeListener
{
    [SerializeField] private List<CardTypeBoolPair> listenerList;

    protected override void OnValueUpdated(object value)
    {
        CardType cardType = (CardType)value;
        foreach (CardTypeBoolPair pair in listenerList)
        {
            if (pair.listenerType == cardType)
                gameObject.SetActive(pair.setActive);
        }
    }
}

[Serializable]
public struct CardTypeBoolPair
{
    public CardType listenerType;
    public bool setActive;
}
