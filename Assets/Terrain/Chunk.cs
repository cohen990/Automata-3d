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

        public ChunkBehaviour Behaviour { get; set; }

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
            return found ? value : Block.NULL;
        }

        public int HighestBlockAt(int x, int z)
        {
            for (var y = Bounds.yMax; y >= 0; y--)
            {
                var block = BlockAt(new Vector3Int(x, y, z));
                if (!Block.IsEmpty(block))
                {
                    return y;
                }
            }

            return -1;
        }
    }
}