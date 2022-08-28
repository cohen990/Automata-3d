using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class World : IEnumerable<KeyValuePair<Vector2Int, Chunk>>
    {
        private readonly Dictionary<Vector2Int, Chunk> _dictionary = new Dictionary<Vector2Int, Chunk>();

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
    }
}