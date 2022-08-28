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

        private void Start()
        {
            _collider = GetComponent<MeshCollider>();
            _filter = GetComponent<MeshFilter>();
            Chunk.Behaviour = this;
            _chunkMesh = ChunkMesh.Generate(_filter, Chunk, World);
            _collider.sharedMesh = _filter.sharedMesh;
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
    }
}