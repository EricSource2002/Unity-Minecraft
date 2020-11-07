using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeGeneration{
public class TreeGeneration
{
    const int chunkWidth = Chunk.chunkWidth;
    const int chunkHeight = Chunk.chunkHeight;
    public static void GenerateTrees(ChunkPos pos,  BlockType[,,] blocks)
    {
        FastNoise.FastNoise noise1 = Chunk.noise1;
        int x = pos.x;
        int z = pos.z;
        System.Random rand = new System.Random(x * 10000 + z);

        float simplex = noise1.GetSimplex(x * .8f, z * .8f);

        if (simplex > 0)
        {
            simplex *= 2f;
            int treeCount = Mathf.FloorToInt((float)rand.NextDouble() * 5 * simplex);

            for (int i = 0; i < treeCount; i++)
            {
                int xPos = (int)(rand.NextDouble() * 14) + 1;
                int zPos = (int)(rand.NextDouble() * 14) + 1;

                int y = Chunk.chunkHeight - 1;
                //find the ground
                while (y > WaterChunkObject.waterHeight && blocks[xPos, y, zPos] == BlockType.Air)
                {
                    y--;
                }
                if (blocks[xPos, y, zPos] == BlockType.Leaves || blocks[xPos, y, zPos] == BlockType.Air) return;
                //dirt under tree
                if (blocks[xPos, y, zPos] == BlockType.Grass)
                {
                    blocks[xPos, y, zPos] = BlockType.Dirt;
                }
                y++;
                int treeHeight = 4 + (int)(rand.NextDouble() * 4);

                for (int j = 0; j < treeHeight; j++)
                {
                    if (y + j < chunkHeight)
                        blocks[xPos, y + j, zPos] = BlockType.Wood;
                }

                int leavesWidth = 1 + (int)(rand.NextDouble() * 6);
                int leavesHeight = (int)(rand.NextDouble() * 3);
                if (leavesHeight + leavesWidth < 5)
                {
                    leavesHeight += 2;
                    leavesWidth += 2;
                }

                int iter = 0;
                for (int y_t = y + treeHeight - 1; y_t <= y + treeHeight - 1 + treeHeight; y_t++)
                {
                    for (int x_t = xPos - (int)(leavesWidth * .5f) + iter / 2; x_t <= xPos + (int)(leavesWidth * .5f) - iter / 2; x_t++)
                        for (int z_t = zPos - (int)(leavesWidth * .5f) + iter / 2; z_t <= zPos + (int)(leavesWidth * .5f) - iter / 2; z_t++)
                        {
                            if (y_t >= 0 && y_t < chunkHeight && rand.NextDouble() < .8f)
                                if (x_t >= chunkWidth || z_t >= chunkWidth || x_t <= 0 || z_t <= 0)
                                {
                                    Vector3 blockPos = new Vector3();
                                    ChunkPos cpNextTo = new ChunkPos();
                                    if (z_t > chunkWidth && x_t < 0)
                                    {
                                        cpNextTo = new ChunkPos(pos.x - chunkWidth, pos.z + chunkWidth);
                                        blockPos = new Vector3(x_t + chunkWidth + 1, y_t, z_t - chunkWidth - 1);
                                    }
                                    else if (z_t < 0 && x_t > chunkWidth)
                                    {
                                        cpNextTo = new ChunkPos(pos.x + chunkWidth, pos.z - chunkWidth);
                                        blockPos = new Vector3(x_t - chunkWidth - 1, y_t, z_t + chunkWidth + 1);
                                    }
                                    else if (z_t > chunkWidth && x_t > chunkWidth)
                                    {
                                        cpNextTo = new ChunkPos(pos.x + chunkWidth, pos.z + chunkWidth);
                                        blockPos = new Vector3(x_t - chunkWidth - 1, y_t, z_t - chunkWidth - 1);
                                    }
                                    else if (z_t < 0 && x_t < 0)
                                    {
                                        cpNextTo = new ChunkPos(pos.x - chunkWidth, pos.z - chunkWidth);
                                        blockPos = new Vector3(x_t + chunkWidth + 1, y_t, z_t + chunkWidth + 1);
                                    }
                                    else if (x_t > chunkWidth)
                                    {
                                        cpNextTo = new ChunkPos(pos.x + chunkWidth, pos.z);
                                        blockPos = new Vector3(x_t - chunkWidth - 1, y_t, z_t);
                                    }
                                    else if (x_t < 0)
                                    {
                                        cpNextTo = new ChunkPos(pos.x - chunkWidth, pos.z);
                                        blockPos = new Vector3(x_t + chunkWidth + 1, y_t, z_t);
                                    }
                                    else if (z_t > chunkWidth)
                                    {
                                        cpNextTo = new ChunkPos(pos.x, pos.z + chunkWidth);
                                        blockPos = new Vector3(x_t, y_t, z_t - chunkWidth - 1);
                                    }
                                    else if (z_t < 0)
                                    {
                                        cpNextTo = new ChunkPos(pos.x, pos.z - chunkWidth);
                                        blockPos = new Vector3(x_t, y_t, z_t + chunkWidth + 1);
                                    }
                                    if (TerrainGenerator.holdsToGenerate.ContainsKey(cpNextTo))
                                    {
                                        if (!TerrainGenerator.holdsToGenerate[cpNextTo].ContainsKey(blockPos))
                                            TerrainGenerator.holdsToGenerate[cpNextTo].Add(blockPos, BlockType.Leaves);
                                    }
                                    else
                                    {
                                        TerrainGenerator.holdsToGenerate.Add(cpNextTo, new Dictionary<Vector3, BlockType>() { { blockPos, BlockType.Leaves } });
                                    }
                                }
                                else
                                {
                                    blocks[x_t, y_t, z_t] = BlockType.Leaves;
                                }
                        }
                    iter++;
                }

            }
        }
    }

}
}