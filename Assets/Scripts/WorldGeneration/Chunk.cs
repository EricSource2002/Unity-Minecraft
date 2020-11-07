using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Profiling;
using UnityEngine;

public class Chunk
{
    public ChunkPos pos;
    //chunk size
    public const int chunkWidth = 16;
    public const int chunkHeight = 256;
    //GENERATE IF CHUNK IS AVAIBLE
    //0 = air, 1 = land
    public BlockType[,,] blocks = new BlockType[chunkWidth + 2, chunkHeight, chunkWidth + 2];
    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    public static FastNoise.FastNoise noise1 = new FastNoise.FastNoise(new System.Random().Next(1000, 9999));
    public Chunk(ChunkPos pos)
    {
        this.pos = pos;
    }

    public void PopulateOffthread()
    {
        GenerateBlocks();
        TreeGeneration.TreeGeneration.GenerateTrees(pos, blocks);
        GenerateTrig();
    }

    public void UpdateTrig()
    {
        GenerateTrig();
    }

    bool Transparent(BlockType bt)
    {
        if (bt == BlockType.Air || bt == BlockType.Water)
            return true;
        else
        {
            return false;
        }
    }
    void GenerateTrig()
    {
        verts = new List<Vector3>();
        tris = new List<int>();
        uvs = new List<Vector2>();
        for (int x = 1; x < chunkWidth + 1; x++)
        {
            for (int z = 1; z < chunkWidth + 1; z++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    var current = blocks[x, y, z];
                    if (current != BlockType.Air && current != BlockType.Water)
                    {
                        Vector3 blockPos = new Vector3(x - 1, y, z - 1);
                        int numFaces = 0;
                        //build top face
                        if (y < chunkHeight - 1 && Transparent(blocks[x, y + 1, z]))
                        {
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            numFaces++;

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].topPos.GetUVs());
                        }

                        //bottom
                        if (y > 0 && Transparent(blocks[x, y - 1, z]))
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].bottomPos.GetUVs());
                        }

                        //front
                        if (Transparent(blocks[x, y, z - 1]))
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            numFaces++;

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].sidePos.GetUVs());
                        }

                        //right
                        if (Transparent(blocks[x + 1, y, z]))
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            numFaces++;

                           uvs.AddRange(Block.blocks[blocks[x, y, z]].sidePos.GetUVs());
                        }

                        //back
                        if (Transparent(blocks[x, y, z + 1]))
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;

                           uvs.AddRange(Block.blocks[blocks[x, y, z]].sidePos.GetUVs());
                        }

                        //left
                        if (Transparent(blocks[x - 1, y, z]))
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            numFaces++;

                           uvs.AddRange(Block.blocks[blocks[x, y, z]].sidePos.GetUVs());
                        }

                        int tl = verts.Count - 4 * numFaces;
                        for (int i = 0; i < numFaces; i++)
                        {
                            tris.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });

                        }
                    }
                }
            }
        }
    }

    void GenerateBlocks()
    {
        for (int x = 0; x < Chunk.chunkWidth + 2; x++)
        {
            for (int z = 0; z < Chunk.chunkWidth + 2; z++)
            {
                for (int y = 0; y < Chunk.chunkHeight; y++)
                {
                    blocks[x, y, z] = GetBlockType(pos.x + x - 1, y, pos.z + z - 1);
                }
            }
        }
    }
    //get the block type at a specific coordinate
    BlockType GetBlockType(int x, int y, int z)
    {

        //Core block placement at the bottom.
        if (y <= 1)
        {
            float simplexCore = noise1.GetSimplex(x * 20f, z * 20f);
            int coreLevel = simplexCore > 0 ? 1 : 0;
            if (y <= coreLevel) return BlockType.Core;
        }

        float simplex1 = noise1.GetSimplexFractal(x * .8f, z * .8f) * 15;
        float simplex2 = noise1.GetSimplex(x * 3f, z * 3f) * 10 * (noise1.GetSimplex(x * .3f, z * .3f) + .5f);

        float heightMap = simplex1 + simplex2;

        //add the 2d noise to the middle of the terrain chunk
        float baseLandHeight = Chunk.chunkHeight * 0.48f + heightMap;

        //stone layer heightmap
        float simplexStone1 = noise1.GetSimplex(x * 1f, z * 1f) * 10;
        float simplexStone2 = (noise1.GetSimplex(x * 5f, z * 5f) + 0.5f) * 20 * (noise1.GetSimplex(x * 0.3f, z * 0.3f) + 0.5f);

        float stoneHeightMap = simplexStone1 + simplexStone2;
        float baseStoneHeight = Chunk.chunkHeight * 0.40f + stoneHeightMap;


        BlockType blockType = BlockType.Air;
        //under the surface, dirt block
        if (y <= baseLandHeight)
        {
            blockType = BlockType.Dirt;

            //just on the surface, use a gr+ass type
            if (y > baseLandHeight - 1 && y > WaterChunkObject.waterHeight - 1)
                blockType = BlockType.Grass;

            if (y <= baseStoneHeight)
                blockType = BlockType.Stone;
        }
        //cave generation

        return blockType;
    }

    void AddSquare(List<Vector3> verts, List<int> tris)
    {

    }

    public List<Vector3> getVerts()
    {
        return verts;
    }

    public List<int> getTris()
    {
        return tris;
    }

    public List<Vector2> getUVs()
    {
        return uvs;
    }
}