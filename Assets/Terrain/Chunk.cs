using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class Chunk : IEnumerable<KeyValuePair<Vector3Int, int>>
    {
        private readonly Dictionary<Vector3Int, int> _dictionary = new Dictionary<Vector3Int, int>();
        private readonly int _yBounds;

        public Chunk(int yBounds)
        {
            _yBounds = yBounds;
        }

        public void SetBlock(Vector3Int position, int blockId)
        {
            _dictionary[position] = blockId;
        }

        public IEnumerator<KeyValuePair<Vector3Int, int>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int BlockAt(Vector3Int key)
        {
            var found = _dictionary.TryGetValue(key, out var value);
            return found ? value : -1;
        }

        public int HighestBlockAt(int x, int z)
        {
            for (var y = _yBounds; y >= 0; y--)
            {
                var block = BlockAt(new Vector3Int(x, y, z));
                if (block > 0)
                {
                    return y;
                }
            }

            return -1;
        }
    }
}