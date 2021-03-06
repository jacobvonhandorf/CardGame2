﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeywordUtils
{
    private static int GetDefenderDamage(Keyword keyword)
    {
        if (keyword == Keyword.Defender1)
            return 1;
        else if (keyword == Keyword.Defender2)
            return 2;
        else if (keyword == Keyword.Defender3)
            return 3;
        else
            return 0;
    }
    public static int GetDefenderValue(Card c)
    {
        int defenderValue = 0;
        foreach (Keyword k in c.KeywordList)
        {
            defenderValue += GetDefenderDamage(k);
        }
        return defenderValue;
    }
}
