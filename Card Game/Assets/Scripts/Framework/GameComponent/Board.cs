using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : CardPile
{
    public Tile myPrefab;
    public int boardWidth;
    public int boardHeight;
    public Tile[,] tileArray;
    public List<Tile> allTiles { get; private set; }
    [SerializeField] private Transform tileContainer; // used to organize tiles in inspector

    private List<Vector2> powerTileCoordinates;
    public static Board instance;

    new void Awake()
    {
        base.Awake();
        instance = this;
        allTiles = new List<Tile>();
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
                Tile newTile = Instantiate(myPrefab, new Vector3(coordinates.x + j, coordinates.y + i, 100), Quaternion.identity);
                newTile.transform.parent = tileContainer;

                newTile.x = j;
                newTile.y = i;
                allTiles.Add(newTile);
                tileArray[j, i] = newTile;
                if (powerTileCoordinates.Contains(new Vector2(newTile.x, newTile.y)))
                    newTile.setAsPowerTile();
            }
        }
    }

    #region GetMovableTiles
    public List<Tile> getAllMovableTiles(Creature creature) => _getAllMovableTiles(creature.getMovement(), creature.getCoordinates(), new List<Tile>(), creature.controller, new List<TileMovePair>());
    private List<Tile> _getAllMovableTiles(int remainingMove, Vector2 coord, List<Tile> returnList, Player controller, List<TileMovePair> tileMovePairs)
    {
        // base cases
        if (remainingMove == 0)
            return returnList;
        Tile tile = getTileByCoordinate((int)coord.x, (int)coord.y);
        if (tile == null)
            return returnList;
        if (tileMoveAlreadyChecked(tileMovePairs, new TileMovePair(tile, remainingMove)))
            return returnList;
        if (tile.creature != null && tile.creature.controller != controller)
            return returnList;
        if (tile.structure != null && tile.structure.controller != controller)
            return returnList;

        // add to list
        remainingMove--;
        if (tile.structure == null && tile.creature == null)
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
    private bool tileMoveAlreadyChecked(List<TileMovePair> tiles, TileMovePair t)
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
            Tile powerTile = getTileByCoordinate((int)coordinate.x, (int)coordinate.y);
            if (powerTile.creature != null && powerTile.creature.controller == player)
                count++;
        }
        return count;
    }
    public List<Structure> getAllStructures()
    {
        List<Structure> returnList = new List<Structure>();
        foreach (Tile tile in allTiles)
        {
            if (tile.structure != null)
                returnList.Add(tile.structure);
        }
        return returnList;
    }
    public void setAllTilesToDefault()
    {
        foreach (Tile t in allTiles)
        {
            t.setActive(false);
            t.setAttackable(false);
            t.setEffectableFalse();
        }
    }
    public List<Structure> getAllStructures(Player controller)
    {
        List<Structure> returnList = new List<Structure>();
        foreach (Tile tile in allTiles)
        {
            if (tile.structure != null && tile.structure.controller == controller)
                returnList.Add(tile.structure);
        }
        return returnList;
    }
    public Tile getTileByCoordinate(int x, int y)
    {
        if (x < 0 || y < 0 || x > boardWidth - 1 || y > boardHeight - 1)
            return null;
        return tileArray[x, y];
    }
    public List<Tile> getAllTilesWithCreatures(Player controller, bool includeUntargetable)
    {
        List<Tile> returnList = new List<Tile>();

        foreach (Tile t in allTiles)
        {
            if (t.creature != null && t.creature.controller == controller)
                returnList.Add(t);
        }

        return returnList;
    }
    public List<Tile> getAllTilesWithCreatures(bool includeUntargetable)
    {
        List<Tile> returnList = new List<Tile>();

        if (includeUntargetable)
        {
            foreach (Tile t in allTiles)
            {
                if (t.creature != null)
                    returnList.Add(t);
            }
        }
        else
        {
            foreach (Tile t in allTiles)
            {
                if (t.creature != null && !t.creature.hasKeyword(Keyword.untargetable))
                    returnList.Add(t);
            }
        }

        return returnList;
    }
    public List<Tile> getAllTilesWithStructures()
    {
        List<Structure> allStructures = GameManager.Get().allStructures;
        List<Tile> returnList = new List<Tile>();
        foreach (Structure s in allStructures)
        {
            if (!returnList.Contains(s.tile))
                returnList.Add(s.tile);
        }
        return returnList;
    }
    public List<Tile> getAllTilesWithStructures(Player controller)
    {
        List<Structure> allStructures = GameManager.Get().allStructures;
        List<Tile> returnList = new List<Tile>();
        foreach (Structure s in allStructures)
        {
            if (!returnList.Contains(s.tile) && s.controller == controller)
                returnList.Add(s.tile);
        }
        return returnList;
    }
    public List<Tile> getAllTilesWithinExactRangeOfTile(Tile tile, int range)
    {
        List<Tile> returnList = new List<Tile>();

        int x1 = tile.x + range;
        int y1 = tile.y + range;
        int x2 = tile.x - range;
        int y2 = tile.y - range;

        Tile currentTile = getTileByCoordinate(x1, tile.y);
        if (currentTile != null)
            returnList.Add(currentTile);
        currentTile = getTileByCoordinate(x2, tile.y);
        if (currentTile != null)
            returnList.Add(currentTile);
        currentTile = getTileByCoordinate(tile.x, y1);
        if (currentTile != null)
            returnList.Add(currentTile);
        currentTile = getTileByCoordinate(tile.x, y2);
        if (currentTile != null)
            returnList.Add(currentTile);

        return returnList;
    }
    public List<Tile> getAllTilesWithinRangeOfTile(Tile tile, int range)
    {
        return _getAllTilesWithinRangeOfTile(tile, range, new List<Tile>(), new List<TileMovePair>());
    }
    private List<Tile> _getAllTilesWithinRangeOfTile(Tile tile, int range, List<Tile> returnList, List<TileMovePair> pairs)
    {
        // base case
        if (range == 0)
            return returnList;
        if (tile == null)
            return returnList;
        if (tileMoveAlreadyChecked(pairs, new TileMovePair(tile, range)))
            return returnList;

        // add to list
        range--;
        returnList.Add(tile);
        pairs.Add(new TileMovePair(tile, range));

        // make more calls
        _getAllTilesWithinRangeOfTile(getTileByCoordinate(tile.x, tile.y + 1), range, returnList, pairs); // up
        _getAllTilesWithinRangeOfTile(getTileByCoordinate(tile.x + 1, tile.y), range, returnList, pairs); // right
        _getAllTilesWithinRangeOfTile(getTileByCoordinate(tile.x, tile.y - 1), range, returnList, pairs); // down
        _getAllTilesWithinRangeOfTile(getTileByCoordinate(tile.x - 1, tile.y), range, returnList, pairs); // up

        return returnList;
    }
    public List<Tile> getAllTilesOnRow(int row) => Enumerable.Range(0, tileArray.GetLength(0)).Select(x => tileArray[x, row]).ToArray().ToList();
    #endregion
}
