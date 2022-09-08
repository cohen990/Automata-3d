using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimplexNoise;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Terrain
{
    public class Terrain : MonoBehaviour
    {
        private const int WorldHeight = 64;
        private const int ChunkSize = 16;
        private const int WorldSize = 5;
        [SerializeField] public GameObject chunkPrefab;
        [SerializeField] public GameObject player;
        public World World;
        public Vector3 Spawn { get; private set; }
        public bool HasSpawn { get; private set; }

        private Random _rng;
        private List<Vector2Int> _chunksToLoad;
        private ChunkGenerationState _chunkGenerationState;
        private Vector2Int _currentlyGeneratingChunk;
        private GameObject _currentlyGeneratingChunkObject;
        private ChunkBehaviour _currentlyGeneratingChunkBehaviour;
        private Chunk _latestGeneratedChunk;
        private Stopwatch _timer;

        public void Start()
        {
            _chunksToLoad = new List<Vector2Int>();
            _rng = new Random();
            Noise.Seed = _rng.Next();

            World = new World(ChunkSize);
            for (var x = -WorldSize; x < WorldSize; x++)
            for (var z = -WorldSize; z < WorldSize; z++)
            {
                _chunksToLoad.Add(new Vector2Int(x, z));
            }
            _timer = new Stopwatch();
        }

        public void Update()
        {
            if (World.ChunkAt(Vector2Int.zero) == null) return;
            if (_currentlyGeneratingChunk == Vector2Int.zero) return;
            
            var spawnXZ = ChunkSize / 2;
            var spawnY = World.ChunkAt(Vector2Int.zero).HighestBlockAt(spawnXZ, spawnXZ) + 2;
            Spawn = new Vector3(spawnXZ, spawnY, spawnXZ);
            HasSpawn = true;
        }

        public void LateUpdate()
        {
            if (_chunksToLoad.Count == 0) return;

            if (_chunkGenerationState == ChunkGenerationState.NotGenerating)
            {
                var playerPosition = player.transform.position;
                var playerPositionXZ = new Vector2(playerPosition.x / ChunkSize, playerPosition.z / ChunkSize);
                _chunksToLoad = _chunksToLoad.OrderBy(x => Vector2.Distance(playerPositionXZ, x)).ToList(); 
                _currentlyGeneratingChunk = _chunksToLoad[0];
                _chunksToLoad.RemoveAt(0);
            }

            GenerateChunk(_currentlyGeneratingChunk);
        }

        private void GenerateChunk(Vector2Int coord)
        {
            switch (_chunkGenerationState)
            {
                case ChunkGenerationState.NotGenerating:
                    _timer.Start();
                    _chunkGenerationState = ChunkGenerationState.InstantiatingChunk;
                    StartCoroutine(ChunkGenerator.Generate(coord, ChunkSize, WorldHeight));
                    break;
                case ChunkGenerationState.InstantiatingChunk:
                    if (ChunkGenerator.HasChunk())
                    {
                        _chunkGenerationState = ChunkGenerationState.GeneratingChunk;
                        _latestGeneratedChunk = ChunkGenerator.LatestChunk();
                        World.SetChunk(coord, _latestGeneratedChunk);
                        var chunkObject = Instantiate(chunkPrefab, transform);
                        _currentlyGeneratingChunkBehaviour = chunkObject.GetComponent<ChunkBehaviour>();
                    }
                    break;
                case ChunkGenerationState.GeneratingChunk:
                    if (_currentlyGeneratingChunkBehaviour.IsInstantiated())
                    {
                        _chunkGenerationState = ChunkGenerationState.GeneratingChunkMesh;
                        _currentlyGeneratingChunkBehaviour.name = $"Chunk ({coord.x}, {coord.y})";
                        World.AssignBehaviourToChunk(_currentlyGeneratingChunkBehaviour, _latestGeneratedChunk);
                        _currentlyGeneratingChunkBehaviour.Chunk = _latestGeneratedChunk;
                        _currentlyGeneratingChunkBehaviour.World = World;
                        StartCoroutine(_currentlyGeneratingChunkBehaviour.Initialize());
                    }
                    break;
                case ChunkGenerationState.GeneratingChunkMesh:
                    if (_currentlyGeneratingChunkBehaviour.IsInitialized())
                    {
                        Debug.Log($"Initialized {_currentlyGeneratingChunkBehaviour.name} in {_timer.Elapsed}");
                        _timer.Reset();
                        _chunkGenerationState = ChunkGenerationState.NotGenerating;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum ChunkGenerationState 
        {
            NotGenerating,
            InstantiatingChunk,
            GeneratingChunk,
            GeneratingChunkMesh,
        }
    }
}