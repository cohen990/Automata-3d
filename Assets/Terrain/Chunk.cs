using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class Chunk : IEnumerable<KeyValuePair<Vector3Int, int>>
    {
        private readonly Dictionary<Vector3Int, int> _dictionary = new Dictionary<Vector3Int, int>();
        public readonly BoundsInt Bounds;

        public Chunk(BoundsInt bounds)
        {
            Bounds = bounds;
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
            return _dictionary[key];
        }
    }
}