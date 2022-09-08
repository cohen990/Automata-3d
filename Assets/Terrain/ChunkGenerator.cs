using System;
using System.Collections;
using System.Diagnostics;
using GameEngine;
using SimplexNoise;
using UnityEngine;

namespace Terrain
{
    public static class ChunkGenerator
    {
        private static Chunk _latestChunk;

        public static IEnumerator Generate(Vector2Int chunkCoordinates, int chunkSize, int worldHeight)
        {
            var chunkBounds = new BoundsInt(chunkCoordinates.x * chunkSize, 0, chunkCoordinates.y * chunkSize, chunkSize, worldHeight, chunkSize);
            
            var chunk = new Chunk(chunkBounds);
            var timer = Stopwatch.StartNew();
            for (var x = chunkBounds.xMin; x < chunkBounds.xMax; x++)
            for (var z = chunkBounds.zMin; z < chunkBounds.zMax; z++)
            {
                var normalizedTerrainHeightNoise = Noise.CalcPixel2D(x, z, 0.004f)/256;
                var spreadFromNaturalHeight = chunkBounds.yMax / 3f;
                var terrainNaturalHeight = chunkBounds.yMax / 2f;
                var terrainHeight = (int)Math.Round(normalizedTerrainHeightNoise *  spreadFromNaturalHeight + terrainNaturalHeight);
                var normalizedDirtHeightNoise = Noise.CalcPixel2D(x, z, 0.5f)/256;
                const float spreadFromDefaultDirtHeight = 1f;
                const int defaultDirtHeight = 2;
                var dirtHeight = normalizedDirtHeightNoise * spreadFromDefaultDirtHeight + defaultDirtHeight;
                
                for (var y = chunkBounds.yMin; y < chunkBounds.yMax; y++)
                {
                    var block = ChooseBlock(y, terrainHeight, dirtHeight, x, z);

                    chunk.SetBlock(new Vector3Int(x, y, z), block);
                    
                    if (timer.ElapsedMilliseconds <= FrameTimer.MAXIMUM_MILLISECONDS_PER_COMPUTATION_BATCH) continue;
                    timer.Reset();
                    yield return null;
                }
            }

            _latestChunk = chunk;
            yield return null;
        }

        private static int ChooseBlock(int y, int terrainHeight, float dirtHeight, int x, int z)
        {
            if (y == 0)
            {
                return Block.BEDROCK;
            }
            if (y < terrainHeight - dirtHeight)
            {
                var noiseValue = Noise.CalcPixel3D(x, y, z, 0.01f);
                noiseValue += Noise.CalcPixel3D(x, y, z, 0.02f);
                noiseValue += Noise.CalcPixel3D(x, y, z, 0.04f);
                noiseValue += Noise.CalcPixel3D(x, y, z, 0.08f);
                noiseValue /= 4;
                if (noiseValue >= 105) return Block.STONE;
            }
            else if (y == terrainHeight - 1)
            {
                return Block.GRASS;
            }
            else if (y < terrainHeight - 1 && y >= terrainHeight - dirtHeight)
            {
                return Block.DIRT;
            }

            return Block.AIR;
        }

        public static bool HasChunk() => _latestChunk != null;

        public static Chunk LatestChunk() => _latestChunk;
    }
}