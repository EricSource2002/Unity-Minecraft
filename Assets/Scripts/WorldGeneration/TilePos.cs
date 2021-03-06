﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePos
{
    int xPos, yPos;
    float res = 16f;
    Vector2[] uvs;

 public TilePos(int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        uvs = new Vector2[]
        {
            new Vector2(xPos/res + .001f, yPos/res + .001f),
            new Vector2(xPos/res + .001f, (yPos+1)/res - .001f),
            new Vector2((xPos+1)/res - .001f, (yPos+1)/res - .001f),
            new Vector2((xPos+1)/res - .001f, yPos/res + .001f),
            
        };
    }

    public Vector2[] GetUVs()
    {
        return uvs;
    }


    public static Dictionary<Tile, TilePos> tiles = new Dictionary<Tile, TilePos>()
    {
        {Tile.Dirt, new TilePos(0,0)},
        {Tile.Grass, new TilePos(1,0)},
        {Tile.GrassSide, new TilePos(0,1)},
        {Tile.Stone, new TilePos(0,2)},
        {Tile.TreeSide, new TilePos(0,4)},
        {Tile.TreeCX, new TilePos(0,3)},
        {Tile.Leaves, new TilePos(0,5)},
        {Tile.Sand, new TilePos(2,6)},
        {Tile.Core, new TilePos(3,0)},
    };
}

public enum Tile { Dirt, Grass, GrassSide, Stone, TreeSide, TreeCX, Leaves, Sand, Core }
