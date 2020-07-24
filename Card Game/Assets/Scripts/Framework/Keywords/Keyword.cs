using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Keyword
{
    Quick = 0,
    Defender1 = 1,
    Defender2 = 2,
    Defender3 = 3,
    Ward = 4,
    Armored1 = 5,
    Anthem = 6,
    Deploy = 7,
    Combatant = 8,
    LastBreath = 9,
    Untargetable = 10,
    Poison = 11,
}

public abstract class KeywordData
{
    // map
    private static readonly Dictionary<Keyword, KeywordData> Map = new Dictionary<Keyword, KeywordData>()
    {
        {Keyword.Quick, new Quick() },
        {Keyword.Defender1, new Defender1() },
        {Keyword.Defender2, new Defender2() },
        {Keyword.Defender3, new Defender3() },
        {Keyword.Ward, new Ward() },
        {Keyword.Armored1, new Armored1() },
        {Keyword.Anthem, new Anthem() },
        {Keyword.Deploy, new Deploy() },
        {Keyword.Combatant, new Combatant() },
        {Keyword.LastBreath, new LastBreath() },
        {Keyword.Untargetable, new Untargetable() },
        {Keyword.Poison, new Poison() },
    };

    public static KeywordData getData(Keyword keyword) => Map[keyword];

    // properties
    public abstract string DescriptionText { get; }
    public abstract string Name { get; }
    public abstract int id { get; }
    public abstract ToolTipInfo info { get; }

    // keyword data
    private class Quick : KeywordData
    {
        public override string DescriptionText => "This creature can move and act the turn it is played";
        public override string Name => "Quick";
        public override int id => 1;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Defender1 : KeywordData
    {
        public override string DescriptionText => "When this creature is attacked deal 1 damage to the attacker";
        public override string Name => "Defender 1";
        public override int id => 2;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Defender2 : KeywordData
    {
        public override string DescriptionText => "When this creature is attacked deal 2 damage to the attacker";
        public override string Name => "Defender 2";
        public override int id => 3;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Defender3 : KeywordData
    {
        public override string DescriptionText => "When this creature is attacked deal 3 damage to the attacker";
        public override string Name => "Defender 3";
        public override int id => 4;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Ward : KeywordData
    {
        public override string DescriptionText => "Any damage dealt to adjacent allies is dealt to this creature instead";
        public override string Name => "Ward";
        public override int id => 5;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Armored1 : KeywordData
    {
        public override string DescriptionText => "Damage dealt to this creature is reduced by one";
        public override string Name => "Armored";
        public override int id => 6;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Anthem : KeywordData
    {
        public override string DescriptionText => "Adjacent allied creatures gain 1 attack";
        public override string Name => "Anthem";
        public override int id => 7;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Deploy : KeywordData
    {
        public override string DescriptionText => "Deploy effects are activated when the creature enters the battlefield";
        public override string Name => "Deploy";
        public override int id => 8;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Combatant : KeywordData
    {
        public override string DescriptionText => "Combatant effects activate when the creature attacks or defends";
        public override string Name => "Combatant";
        public override int id => 9;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class LastBreath : KeywordData
    {
        public override string DescriptionText => "Last breath effect activate when the creature dies";
        public override string Name => "Last Breath";
        public override int id => 10;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Untargetable : KeywordData
    {
        public override string DescriptionText => "Cannot be the target of spells or abilities your opponent controls";
        public override string Name => "Untargetable";
        public override int id => 11;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Poison : KeywordData
    {
        public override string DescriptionText => "After combat with a creature destroy that creature";
        public override string Name => "Poison";
        public override int id => 12;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
}
