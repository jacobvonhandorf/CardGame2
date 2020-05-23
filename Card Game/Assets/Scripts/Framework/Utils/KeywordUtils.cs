using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public static class KeywordUtils
{
    private static int getDefenderDamage(CardKeywords keyword)
    {
        switch (keyword)
        {
            case CardKeywords.Defender1:
                return 1;
            case CardKeywords.Defender2:
                return 2;
            case CardKeywords.Defender3:
                return 3;
            default:
                return 0;
        }
    }
    public static int getDefenderValue(Card c)
    {
        int defenderValue = 0;
        foreach (CardKeywords k in c.getKeywords())
        {
            defenderValue += getDefenderDamage(k);
        }
        return defenderValue;
    }
}
