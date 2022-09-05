using SimplexNoise;
using UnityEngine;
using Random = System.Random;

namespace Terrain
{
    public class Terrain : MonoBehaviour
    {
        [SerializeField] public GameObject chunkPrefab;
        public World World;
        public Vector3 Spawn { get; private set; }

        private Random _rng;

        public void Start()
        {
            _rng = new Random();
            Noise.Seed = _rng.Next();
            const int chunkSize = 16;
            const int worldHeight = 64;
            const int worldSize = 1;
            
            World = new World(chunkSize);

            for (var x = 0; x < worldSize; x++)
            for (var z = 0; z < worldSize; z++)
            {
                var chunkBounds = new BoundsInt(x * chunkSize, 0, z * chunkSize, chunkSize, worldHeight, chunkSize);
                var chunk = ChunkGenerator.Generate(chunkBounds);
                World.SetChunk(new Vector2Int(x, z), chunk);
            }

            foreach (var chunk in World)
            {
                var chunkGameObject = Instantiate(chunkPrefab, transform);
                chunkGameObject.name = $"Chunk ({chunk.Key.x}, {chunk.Key.y})";
                var chunkBehaviour = chunkGameObject.GetComponent<ChunkBehaviour>();
                World.AssignBehaviourToChunk(chunkBehaviour, chunk.Value);
                chunkBehaviour.Chunk = chunk.Value;
                chunkBehaviour.World = World;
            }

            var spawnXZChunk = worldSize / 2;
            var spawnXZ = spawnXZChunk * chunkSize + chunkSize / 2;
            var spawnY = World.ChunkAt(new Vector2Int(spawnXZChunk, spawnXZChunk)).HighestBlockAt(spawnXZ, spawnXZ) + 2;
            Spawn = new Vector3(spawnXZ, spawnY, spawnXZ);
        }
    }
}