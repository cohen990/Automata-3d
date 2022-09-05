using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class World : IEnumerable<KeyValuePair<Vector2Int, Chunk>>
    {
        private readonly Dictionary<Vector2Int, Chunk> _dictionary = new();
        private readonly Dictionary<Chunk, ChunkBehaviour> _chunkBehaviours = new();
        private readonly int _chunkSize;

        public World(int chunkSize)
        {
            _chunkSize = chunkSize;
        }

        public void SetChunk(Vector2Int position, Chunk chunk)
        {
            _dictionary[position] = chunk;
        }

        public IEnumerator<KeyValuePair<Vector2Int, Chunk>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Chunk ChunkAt(Vector2Int key)
        {
            var found = _dictionary.TryGetValue(key, out var value);
            return found ? value : Chunk.Empty;
        }
        
        private Chunk ChunkAt(Vector3Int blockPosition) =>
            ChunkAt(new Vector2Int(blockPosition.x / _chunkSize, blockPosition.z / _chunkSize));

        public int BlockAt(Vector3Int blockPosition)
        {
            return ChunkAt(blockPosition).BlockAt(blockPosition);
        }

        public void SetBlock(Vector3Int blockPosition, int blockId)
        {
            if (BlockAt(blockPosition) == Block.BEDROCK)
                return;
            
            var chunk = ChunkAt(blockPosition);
            if (chunk.IsEmpty)
                return;
            
            chunk.SetBlock(blockPosition, blockId);
            _chunkBehaviours[chunk].UpdateBlock(blockPosition);
            
            if (blockPosition.x == chunk.Bounds.xMin)
            {
                ForceUpdateBlock(blockPosition + new Vector3Int(-1, 0, 0));
            }
            if (blockPosition.x == chunk.Bounds.xMax - 1)
            {
                ForceUpdateBlock(blockPosition + new Vector3Int(1, 0, 0));
            }
            if (blockPosition.z == chunk.Bounds.zMin)
            {
                ForceUpdateBlock(blockPosition + new Vector3Int(0, 0, -1));
            }
            if (blockPosition.z == chunk.Bounds.zMax - 1)
            {
                ForceUpdateBlock(blockPosition + new Vector3Int(0, 0, 1));
            }
        }

        private void ForceUpdateBlock(Vector3Int blockPosition)
        {
            if (!_chunkBehaviours.TryGetValue(ChunkAt(blockPosition), out var behaviour))
                return;
            
            behaviour.UpdateSingleBlock(blockPosition);
        }

        public void AssignBehaviourToChunk(ChunkBehaviour chunkBehaviour, Chunk chunk)
        {
            _chunkBehaviours[chunk] = chunkBehaviour;
        }

    }
}