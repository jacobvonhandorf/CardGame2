public interface IScriptTile
{
    IScriptPermanent Permanent { get; }
    IScriptCreature Creature { get; }
    IScriptStructure Structure { get; }
    int X { get; }
    int Y { get; }

    int GetDistanceTo(Tile t);
    int GetDistanceTo(IScriptTile t);

}