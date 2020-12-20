using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : CardPile
{
    public static Board Instance { get; private set; }
    public Tile myPrefab;
    public int boardWidth;
    public int boardHeight;
    public Tile[,] tileArray;
    public List<Tile> AllTiles { get; private set; }
    public List<Creature> AllCreatures { get { return CardList.Where(c => c.IsType(CardType.Creature)).Select(c => (c as CreatureCard).Creature).ToList(); } }
    public List<Structure> AllStructures { get { return CardList.Where(c => c.IsType(CardType.Structure)).Select(c => (c as StructureCard).Structure).ToList(); } }
    [SerializeField] private Transform tileContainer; // used to organize tiles in inspector

    private List<Vector2> powerTileCoordinates;

    new void Awake()
    {
        base.Awake();
        Instance = this;
        AllTiles = new List<Tile>();
        powerTileCoordinates = new List<Vector2>();
        // add power tiles to power tile list
        powerTileCoordinates.Add(new Vector2(3, 3));
        powerTileCoordinates.Add(new Vector2(3, 4));
        powerTileCoordinates.Add(new Vector2(4, 3));
        powerTileCoordinates.Add(new Vector2(4, 4));

        tileArray = new Tile[boardWidth, boardHeight];
        Vector2 coordinates = transform.position;

        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                Tile newTile = Instantiate(myPrefab, tileContainer);

                newTile.X = j;
                newTile.Y = i;
                newTile.name = j + ", " + i;
                AllTiles.Add(newTile);
                tileArray[j, i] = newTile;
                if (powerTileCoordinates.Contains(new Vector2(newTile.X, newTile.Y)))
                    newTile.SetAsPowerTile();
            }
        }
    }

    #region GetMovableTiles
    public List<Tile> GetAllMovableTiles(Creature creature) => _getAllMovableTiles(creature.Movement, creature.Coordinates, new List<Tile>(), creature.Controller, new List<TileMovePair>());
    private List<Tile> _getAllMovableTiles(int remainingMove, Vector2 coord, List<Tile> returnList, Player controller, List<TileMovePair> tileMovePairs)
    {
        // base cases
        if (remainingMove == 0)
            return returnList;
        Tile tile = GetTileByCoordinate((int)coord.x, (int)coord.y);
        if (tile == null)
            return returnList;
        if (TileMoveAlreadyChecked(tileMovePairs, new TileMovePair(tile, remainingMove)))
            return returnList;
        if (tile.Creature != null && tile.Creature.Controller != controller)
            return returnList;
        if (tile.Structure != null && tile.Structure.Controller != controller)
            return returnList;

        // add to list
        remainingMove--;
        if (tile.Structure == null && tile.Creature == null)
        {
            returnList.Add(tile);
            tileMovePairs.Add(new TileMovePair(tile, remainingMove));
        }

        // make more calls
        _getAllMovableTiles(remainingMove, new Vector2(coord.x, coord.y + 1), returnList, controller, tileMovePairs); // up
        _getAllMovableTiles(remainingMove, new Vector2(coord.x + 1, coord.y), returnList, controller, tileMovePairs); // right
        _getAllMovableTiles(remainingMove, new Vector2(coord.x, coord.y - 1), returnList, controller, tileMovePairs); // down
        _getAllMovableTiles(remainingMove, new Vector2(coord.x - 1, coord.y), returnList, controller, tileMovePairs); // left

        return returnList;
    }
    private bool TileMoveAlreadyChecked(List<TileMovePair> tiles, TileMovePair t)
    {
        foreach (TileMovePair pair in tiles)
            if (pair.tile == t.tile)
                if (pair.move >= t.move)
                    return true;
        return false;
    }
    private class TileMovePair
    {
        public Tile tile;
        public int move;

        public TileMovePair(Tile tile, int remainingMove)
        {
            this.tile = tile;
            move = remainingMove;
        }
    }
    #endregion
    #region UtilityMethods
    public int getPowerTileCount(Player player)
    {
        int count = 0;
        foreach (Vector2 coordinate in powerTileCoordinates)
        {
            Tile powerTile = GetTileByCoordinate((int)coordinate.x, (int)coordinate.y);
            if (powerTile.Creature != null && powerTile.Creature.Controller == player)
                count++;
        }
        return count;
    }
    public List<Structure> getAllStructures(Player controller)
    {
        List<Structure> returnList = new List<Structure>();
        foreach (Tile tile in AllTiles)
        {
            if (tile.Structure != null && tile.Structure.Controller == controller)
                returnList.Add(tile.Structure);
        }
        return returnList;
    }
    public Tile GetTileByCoordinate(int x, int y)
    {
        if (x < 0 || y < 0 || x > boardWidth - 1 || y > boardHeight - 1)
            return null;
        return tileArray[x, y];
    }
    public List<Tile> GetAllTilesWithCreatures(Player controller, bool includeUntargetable)
    {
        List<Tile> returnList = new List<Tile>();

        foreach (Tile t in AllTiles)
        {
            if (t.Creature != null && t.Creature.Controller == controller)
                returnList.Add(t);
        }

        return returnList;
    }
    public List<Tile> GetAllTilesWithCreatures(bool includeUntargetable)
    {
        List<Tile> returnList = new List<Tile>();

        if (includeUntargetable)
        {
            foreach (Tile t in AllTiles)
            {
                if (t.Creature != null)
                    returnList.Add(t);
            }
        }
        else
        {
            foreach (Tile t in AllTiles)
            {
                if (t.Creature != null && !t.Creature.HasKeyword(Keyword.Untargetable))
                    returnList.Add(t);
            }
        }

        return returnList;
    }
    public List<Tile> GetAllTilesWithStructures()
    {
        List<Tile> returnList = new List<Tile>();
        foreach (Structure s in AllStructures)
        {
            if (!returnList.Contains(s.Tile))
                returnList.Add(s.Tile);
        }
        return returnList;
    }
    public List<Tile> GetAllTilesWithStructures(Player controller)
    {
        List<Tile> returnList = new List<Tile>();
        foreach (Structure s in AllStructures)
        {
            if (!returnList.Contains(s.Tile) && s.Controller == controller)
                returnList.Add(s.Tile);
        }
        return returnList;
    }
    public List<Tile> GetAllTilesWithinExactRangeOfTile(Tile tile, int range)
    {
        List<Tile> returnList = new List<Tile>();

        int x1 = tile.X + range;
        int y1 = tile.Y + range;
        int x2 = tile.X - range;
        int y2 = tile.Y - range;

        Tile currentTile = GetTileByCoordinate(x1, tile.Y);
        if (currentTile != null)
            returnList.Add(currentTile);
        currentTile = GetTileByCoordinate(x2, tile.Y);
        if (currentTile != null)
            returnList.Add(currentTile);
        currentTile = GetTileByCoordinate(tile.X, y1);
        if (currentTile != null)
            returnList.Add(currentTile);
        currentTile = GetTileByCoordinate(tile.X, y2);
        if (currentTile != null)
            returnList.Add(currentTile);

        return returnList;
    }
    public List<Tile> GetAllTilesWithinRangeOfTile(Tile tile, int range) => _getAllTilesWithinRangeOfTile(tile, range, new List<Tile>(), new List<TileMovePair>());
    private List<Tile> _getAllTilesWithinRangeOfTile(Tile tile, int range, List<Tile> returnList, List<TileMovePair> pairs)
    {
        // base case
        if (range == 0)
            return returnList;
        if (tile == null)
            return returnList;
        if (TileMoveAlreadyChecked(pairs, new TileMovePair(tile, range)))
            return returnList;

        // add to list
        range--;
        returnList.Add(tile);
        pairs.Add(new TileMovePair(tile, range));

        // make more calls
        _getAllTilesWithinRangeOfTile(GetTileByCoordinate(tile.X, tile.Y + 1), range, returnList, pairs); // up
        _getAllTilesWithinRangeOfTile(GetTileByCoordinate(tile.X + 1, tile.Y), range, returnList, pairs); // right
        _getAllTilesWithinRangeOfTile(GetTileByCoordinate(tile.X, tile.Y - 1), range, returnList, pairs); // down
        _getAllTilesWithinRangeOfTile(GetTileByCoordinate(tile.X - 1, tile.Y), range, returnList, pairs); // up

        return returnList;
    }
    public List<Tile> GetAllTilesOnRow(int row) => Enumerable.Range(0, tileArray.GetLength(0)).Select(x => tileArray[x, row]).ToArray().ToList();
    #endregion
    #region CardPile
    protected override void OnCardAdded(Card c) { }
    protected override void OnCardRemoved(Card c) { }
    #endregion
}
