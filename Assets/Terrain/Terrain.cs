using UnityEngine;
using Random = System.Random;

namespace Terrain
{
    public class Terrain : MonoBehaviour
    {
        [SerializeField] public GameObject chunkPrefab;

        private Random _rng;

        public void Start()
        {
            _rng = new Random();
            SimplexNoise.Noise.Seed = _rng.Next();
            const int chunkSize = 16;
            const int worldHeight = 64;
            
            const int worldSize = 3;

            for (var x = 0; x < worldSize; x++)
            for (var z = 0; z < worldSize; z++)
            {
                var chunk = Instantiate(chunkPrefab, transform);
                chunk.name = $"Chunk ({x}, {z})";
                var chunkBounds = new BoundsInt(x * chunkSize, 0, z * chunkSize,  chunkSize, worldHeight, chunkSize);
                chunk.GetComponent<ChunkBehaviour>().Initialize(chunkBounds);
            }
        }
    }
}