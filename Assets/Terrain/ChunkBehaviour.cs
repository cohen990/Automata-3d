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
            for (var y = chunkBounds.yMin; y < chunkBounds.yMax; y++)
            for (var z = chunkBounds.zMin; z < chunkBounds.zMax; z++)
            {
                var noiseValue = Noise.CalcPixel3D(x, y, z, 0.01f);
                noiseValue += Noise.CalcPixel3D(x, y, z, 0.02f);
                noiseValue += Noise.CalcPixel3D(x, y, z, 0.04f);
                noiseValue += Noise.CalcPixel3D(x, y, z, 0.08f);
                noiseValue /= 4;
                chunk.SetBlock(new Vector3Int(x, y, z), noiseValue >= 105 ? 1 : 0);
            }

            ChunkMesh.Generate(GetComponent<MeshFilter>(), chunk);
        }
    }
}