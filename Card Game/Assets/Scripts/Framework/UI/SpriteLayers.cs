using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayers
{
    public string Value { get; set; }
    private SpriteLayers(string value) { Value = value; }

    public static SpriteLayers Creature { get { return new SpriteLayers("Creature"); } }
    public static SpriteLayers CreatureAbove { get { return new SpriteLayers("Creature above"); } }
    public static SpriteLayers CardBeingDragged { get { return new SpriteLayers("Card Being Dragged"); } }
    public static SpriteLayers CardInHandMiddle { get { return new SpriteLayers("Card Middle"); } }
}
