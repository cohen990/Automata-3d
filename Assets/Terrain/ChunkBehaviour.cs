using SimplexNoise;
using UnityEngine;

namespace Terrain
{
    public class ChunkBehaviour : MonoBehaviour
    {
        public void Initialize(BoundsInt chunkBounds)
        {
            var chunk = new Chunk();
            for (var x = chunkBounds.xMin; x < chunkBounds.xMax; x++)
            for (var z = chunkBounds.zMin; z < chunkBounds.zMax; z++)
            {
                var normalizedTerrainHeightNoise = Noise.CalcPixel2D(x, z, 0.004f)/256;
                var spreadFromNaturalHeight = chunkBounds.yMax / 3f;
                var terrainNaturalHeight = chunkBounds.yMax / 2f;
                var terrainHeight = normalizedTerrainHeightNoise *  spreadFromNaturalHeight + terrainNaturalHeight;
                var normalizedDirtHeightNoise = Noise.CalcPixel2D(x, z, 0.5f)/256;
                const float spreadFromDefaultDirtHeight = 1f;
                const int defaultDirtHeight = 2;
                var dirtHeight = normalizedDirtHeightNoise * spreadFromDefaultDirtHeight + defaultDirtHeight;
                for (var y = chunkBounds.yMin; y < chunkBounds.yMax; y++)
                {
                    var block = 0;
                    if (y < terrainHeight - dirtHeight)
                    {
                        var noiseValue = Noise.CalcPixel3D(x, y, z, 0.01f);
                        noiseValue += Noise.CalcPixel3D(x, y, z, 0.02f);
                        noiseValue += Noise.CalcPixel3D(x, y, z, 0.04f);
                        noiseValue += Noise.CalcPixel3D(x, y, z, 0.08f);
                        noiseValue /= 4;
                        if (noiseValue >= 105) block = 2;
                    }
                    else if (y < terrainHeight && y >= terrainHeight - dirtHeight)
                    {
                        block = 1;
                    }

                    chunk.SetBlock(new Vector3Int(x, y, z), block);
                }
            }

            ChunkMesh.Generate(GetComponent<MeshFilter>(), chunk);
            GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        }
    }
}