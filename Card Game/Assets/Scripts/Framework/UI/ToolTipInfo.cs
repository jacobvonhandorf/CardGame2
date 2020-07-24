using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolTipInfo
{
    #region GlobalToolTips
    public static ToolTipInfo arcaneSpell { get; } = new ArcaneSpell();
    #endregion

    // convert keyword to tool tip
    public static ToolTipInfo getToolTipInfoFromKeyword(KeywordData k) => new KeywordInfo(k);

    #region Properties
    public abstract string headerText { get; }
    public abstract string descriptionText { get; }
    #endregion

    #region Data
    private class KeywordInfo : ToolTipInfo
    {
        public KeywordData keyword;
        public override string headerText => keyword.Name;
        public override string descriptionText => keyword.DescriptionText;
        public KeywordInfo(KeywordData k)
        {
            keyword = k;
        }
    }
    private class ArcaneSpell : ToolTipInfo
    {
        public override string headerText => "Arcane Spell";
        public override string descriptionText => "Arcane spells can only be cast while you control an Arcane creature";
    }
    #endregion
}
