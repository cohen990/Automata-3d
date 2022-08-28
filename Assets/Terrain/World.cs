using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class World : IEnumerable<KeyValuePair<Vector2Int, Chunk>>
    {
        private readonly Dictionary<Vector2Int, Chunk> _dictionary = new Dictionary<Vector2Int, Chunk>();
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
            return found ? value : null;
        }
        
        private Chunk ChunkAt(Vector3Int blockPosition) =>
            ChunkAt(new Vector2Int(blockPosition.x / _chunkSize, blockPosition.z / _chunkSize));

        public int BlockAt(Vector3Int blockPosition)
        {
            var block = ChunkAt(blockPosition)?.BlockAt(blockPosition);
            return block ?? Block.NULL;
        }


        public void SetBlock(Vector3Int blockPosition, int blockId)
        {
            if (BlockAt(blockPosition) == Block.BEDROCK)
                return;
            
            var chunk = ChunkAt(blockPosition);
            if (chunk == null)
            {
                return;
            }
            chunk.SetBlock(blockPosition, blockId);
            chunk.Behaviour.UpdateBlock(blockPosition, blockId);
            
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
            ChunkAt(blockPosition)?.Behaviour.UpdateSingleBlock(blockPosition);
        }
    }
}