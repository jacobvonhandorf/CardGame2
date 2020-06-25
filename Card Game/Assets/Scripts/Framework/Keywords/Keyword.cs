using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Keyword
{
    // enum
    public static Keyword quick { get; } = new Quick();
    public static Keyword defender1 { get; } = new Defender1();
    public static Keyword defender2 { get; } = new Defender2();
    public static Keyword defender3 { get; } = new Defender3();
    public static Keyword ward { get; } = new Ward();
    public static Keyword armored1 { get; } = new Armored1();
    public static Keyword anthem { get; } = new Anthem();
    public static Keyword deploy { get; } = new Deploy();
    public static Keyword combatant { get; } = new Combatant();
    public static Keyword lastBreath { get; } = new LastBreath();
    public static Keyword untargetable { get; } = new Untargetable();

    // map TODO

    // properties
    public abstract string DescriptionText { get; }
    public abstract string Name { get; }
    public abstract int id { get; }
    public abstract ToolTipInfo info { get; }

    // keyword data
    private class Quick : Keyword
    {
        public override string DescriptionText => "This creature can move and act the turn it is played";
        public override string Name => "Quick";
        public override int id => 1;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Defender1 : Keyword
    {
        public override string DescriptionText => "When this creature is attacked deal 1 damage to the attacker";
        public override string Name => "Defender 1";
        public override int id => 2;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Defender2 : Keyword
    {
        public override string DescriptionText => "When this creature is attacked deal 2 damage to the attacker";
        public override string Name => "Defender 2";
        public override int id => 3;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Defender3 : Keyword
    {
        public override string DescriptionText => "When this creature is attacked deal 3 damage to the attacker";
        public override string Name => "Defender 3";
        public override int id => 4;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Ward : Keyword
    {
        public override string DescriptionText => "Any damage dealt to adjacent allies is dealt to this creature instead";
        public override string Name => "Ward";
        public override int id => 5;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Armored1 : Keyword
    {
        public override string DescriptionText => "Damage dealt to this creature is reduced by one";
        public override string Name => "Armored";
        public override int id => 6;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Anthem : Keyword
    {
        public override string DescriptionText => "Adjacent allied creatures gain 1 attack";
        public override string Name => "Anthem";
        public override int id => 7;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Deploy : Keyword
    {
        public override string DescriptionText => "Deploy effects are activated when the creature enters the battlefield";
        public override string Name => "Deploy";
        public override int id => 8;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Combatant : Keyword
    {
        public override string DescriptionText => "Combatant effects activate when the creature attacks or defends";
        public override string Name => "Combatant";
        public override int id => 9;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class LastBreath : Keyword
    {
        public override string DescriptionText => "Last breath effect activate when the creature dies";
        public override string Name => "Last Breath";
        public override int id => 10;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
    private class Untargetable : Keyword
    {
        public override string DescriptionText => "Cannot be the target of spells or abilities your opponent controls";
        public override string Name => "Untargetable";
        public override int id => 11;
        public override ToolTipInfo info => ToolTipInfo.getToolTipInfoFromKeyword(this);
    }
}
