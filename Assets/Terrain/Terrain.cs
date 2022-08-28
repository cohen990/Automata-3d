using SimplexNoise;
using UnityEngine;
using Random = System.Random;

namespace Terrain
{
    public class Terrain : MonoBehaviour
    {
        [SerializeField] public GameObject chunkPrefab;
        public Vector3 Spawn { get; private set; }

        private Random _rng;

        public void Start()
        {
            _rng = new Random();
            Noise.Seed = _rng.Next();
            const int chunkSize = 16;
            const int worldHeight = 32;

            const int worldSize = 5;
            var world = new World(chunkSize);

            for (var x = 0; x < worldSize; x++)
            for (var z = 0; z < worldSize; z++)
            {
                var chunkBounds = new BoundsInt(x * chunkSize, 0, z * chunkSize, chunkSize, worldHeight, chunkSize);
                var chunk = ChunkGenerator.Generate(chunkBounds);
                world.SetChunk(new Vector2Int(x, z), chunk);
            }

            foreach (var chunk in world)
            {
                var chunkGameObject = Instantiate(chunkPrefab, transform);
                chunkGameObject.name = $"Chunk ({chunk.Key.x}, {chunk.Key.y})";
                chunkGameObject.GetComponent<ChunkBehaviour>().FormMesh(chunk.Value, world);
            }

            var spawnXZChunk = worldSize / 2;
            var spawnXZ = spawnXZChunk * chunkSize + chunkSize / 2;
            var spawnY = world.ChunkAt(new Vector2Int(spawnXZChunk, spawnXZChunk)).HighestBlockAt(spawnXZ, spawnXZ) + 2;
            Spawn = new Vector3(spawnXZ, spawnY, spawnXZ);
        }
    }
}