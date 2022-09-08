using System.Collections;
using Terrain.Mesh;
using UnityEngine;

namespace Terrain
{
    public class ChunkBehaviour : MonoBehaviour
    {
        private MeshFilter _filter;
        private MeshCollider _collider;
        public Chunk Chunk { get; set; }
        public World World { get; set; }

        private ChunkMesh _chunkMesh;
        private bool _initialized;
        private bool _instantiated;

        private void Start()
        {
            _collider = GetComponent<MeshCollider>();
            _filter = GetComponent<MeshFilter>();
            _instantiated = true;
        }
        
        public IEnumerator Initialize()
        {
            yield return ChunkMesh.Generate(_filter, Chunk, World);
            _chunkMesh = ChunkMesh.Latest;
            _collider.sharedMesh = _filter.sharedMesh;
            _initialized = true;
        }

        public void UpdateBlock(Vector3Int blockPosition)
        {
            _chunkMesh.UpdateBlock(blockPosition);
            _collider.sharedMesh = _filter.sharedMesh;
        }

        public void UpdateSingleBlock(Vector3Int blockPosition)
        {
            _chunkMesh.UpdateSingleBlock(blockPosition);
            _collider.sharedMesh = _filter.sharedMesh;
        }

        public bool IsInitialized() => _initialized;
        public bool IsInstantiated() => _instantiated;

    }
}