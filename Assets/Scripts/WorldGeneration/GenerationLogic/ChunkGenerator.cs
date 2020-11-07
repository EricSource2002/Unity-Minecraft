
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
namespace ChunkGenerator
{
    public class ChunkGenerator
    {
        public static HashSet<ChunkPos> requestedChunk = new HashSet<ChunkPos>();
        public static Dictionary<ChunkPos, Chunk> chunkBlocks = new Dictionary<ChunkPos, Chunk>();

        public static Chunk request(ChunkPos pos, bool instant)
        {
            lock (chunkBlocks)
            {
                if (chunkBlocks.ContainsKey(pos))
                {
                    return chunkBlocks[pos];
                }
                else if (!requestedChunk.Contains(pos))
                {
                    Monitor.PulseAll(chunkBlocks);
                    requestedChunk.Add(pos);
                }
                if (instant)
                {
                    Chunk chunk = new Chunk(pos);
                    chunk.PopulateOffthread();
                    chunkBlocks[pos] = chunk;
                    return chunk;
                }
                return null;
            }
        }

        public static int getRequestSize()
        {
            return requestedChunk.Count;
        }

        public static void Start()
        {
            for (int i = 0; i <= 16; i++)
            {
                SpawnThread();
            }

        }
        static void SpawnThread()
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    Nullable<ChunkPos> toCompute = null;
                    lock (chunkBlocks)
                    {
                        if (requestedChunk.Count == 0)
                        {
                            Monitor.Wait(chunkBlocks);
                            continue;
                        }
                        ChunkPos pos = requestedChunk.First();
                        requestedChunk.Remove(pos);
                        if (chunkBlocks.ContainsKey(pos))
                        {
                            Monitor.PulseAll(chunkBlocks);
                            continue;
                        }
                        else
                        {
                            Monitor.PulseAll(chunkBlocks);
                            toCompute = pos;
                        }
                    }
                    Chunk chunk = new Chunk(toCompute.Value);
                    chunk.PopulateOffthread();
                    lock (chunkBlocks)
                    {
                        chunkBlocks[toCompute.Value] = chunk;
                        Monitor.PulseAll(chunkBlocks);
                    }

                }

            });
            thread.Start();
        }
    }
}