using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolTipInfo
{
    // enum
    /*
    public static ToolTipInfo deploy { get; } = new Deploy();
    public static ToolTipInfo defender1 { get; } = new Defender1();
    public static ToolTipInfo quick { get; } = new Quick();
    */

    // convert keyword to tool tip
    public static ToolTipInfo getToolTipInfoFromKeyword(Keyword k)
    {
        return new KeywordInfo(k);
    }

    // properties
    public abstract string headerText { get; }
    public abstract string descriptionText { get; }

    // data
    /*
    private class Deploy : ToolTipInfo
    {
        public override string headerText => "Deploy";
        public override string descriptionText => "Deploy abilites trigger when the creature is played";
    }*/
    private class KeywordInfo : ToolTipInfo
    {
        public Keyword keyword;
        public override string headerText => keyword.Name;
        public override string descriptionText => keyword.DescriptionText;
        public KeywordInfo(Keyword k)
        {
            keyword = k;
        }
    }
    /*
    private class Quick : ToolTipInfo
    {
        public override string headerText => Keyword.quick.Name;
        public override string descriptionText => Keyword.quick.DescriptionText;
    }
    private class Defender1 : ToolTipInfo
    {
        public override string headerText => Keyword.defender1.Name;
        public override string descriptionText => Keyword.defender1.DescriptionText;
    }
    */
}
