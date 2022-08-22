using System.Collections.Generic;

namespace Terrain
{
    public static class Corners
    {
        private static readonly int[] NegativeXFace =
        {
            0, 1, 2,
            0, 2, 3
        };

        private static readonly int[] PositiveXFace =
        {
            4, 5, 6,
            4, 6, 7
        };

        private static readonly int[] NegativeYFace =
        {
            7, 6, 1,
            7, 1, 0
        };

        private static readonly int[] PositiveYFace =
        {
            3, 2, 5,
            3, 5, 4
        };
        

        private static readonly int[] NegativeZFace =
        {
            7, 0, 3,
            7, 3, 4
        };
        
        private static readonly int[] PositiveZFace =
        {
            1, 6, 5,
            1, 5, 2
        };


        public static readonly Dictionary<Faces, int[]> For = new Dictionary<Faces, int[]>
        {
            {Faces.PositiveX, PositiveXFace},
            {Faces.NegativeX, NegativeXFace},
            {Faces.PositiveY, PositiveYFace},
            {Faces.NegativeY, NegativeYFace},
            {Faces.PositiveZ, PositiveZFace},
            {Faces.NegativeZ, NegativeZFace}
        };
    }
}