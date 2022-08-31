using UnityEngine;

namespace Terrain
{
    public readonly struct Chunk
    {
        private readonly int[] _blocks;
        private readonly int _xFactor;
        private readonly int _yFactor;
        public readonly BoundsInt Bounds;
        public int BlockCount => _blocks.Length;
        public bool IsEmpty => _blocks.Length == 0;

        public Chunk(BoundsInt bounds)
        {
            _blocks = new int[bounds.size.x * bounds.size.y * bounds.size.z];
            Bounds = bounds;
            _xFactor = bounds.size.x * bounds.size.y;
            _yFactor = bounds.size.z;
        }

        public static Chunk Empty = new Chunk(new BoundsInt(0, 0, 0, 0, 0, 0));


        public void SetBlock(Vector3Int position, int blockId)
        {
            var index = IndexFor(position);
            if (!InRange(index))
            {
                return;
            }
            _blocks[index] = blockId;
        }

        public int BlockAt(Vector3Int key)
        {
            var index = IndexFor(key);
            return InRange(index) ? _blocks[index] : Block.NULL;
        }

        private bool InRange(int index)
        {
            var inRange = index <= _blocks.Length - 1 && index >= 0;
            return inRange;
        }

        public int HighestBlockAt(int x, int z)
        {
            for (var y = Bounds.yMax - 1; y >= 0; y--)
            {
                var index = IndexFor(x, y, z);
                
                if (index < _blocks.Length && !Block.IsEmpty(_blocks[index]))
                {
                    return y;
                }
            }

            return -1;
        }
        
        private int IndexFor(Vector3Int position)
        {
            return IndexFor(position.x, position.y, position.z);
        }
        
        private int IndexFor(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0)
                return -1;
            if (x >= Bounds.xMax || y >= Bounds.yMax || z >= Bounds.zMax)
                return -1;
            
            var index = _xFactor * (x - Bounds.xMin) + _yFactor * (y - Bounds.yMin) + z - Bounds.zMin;
            return index;
        }
        
        public Vector3Int Vector3PositionOf(int index)
        {
            var x = index / _xFactor;
            var y = (index - x * _xFactor) / _yFactor;
            var z = index % _yFactor;
            var vector3PositionOf = new Vector3Int(x + Bounds.xMin, y + Bounds.yMin, z + Bounds.zMin);
            return vector3PositionOf;
        }
    }
}