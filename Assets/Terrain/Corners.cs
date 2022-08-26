using System.Collections.Generic;

namespace Terrain
{
    public static class Corners
    {
        private static readonly int[] NegativeXFace =
        {
            2, 5, 8,
            2, 8, 11
        };

        private static readonly int[] PositiveXFace =
        {
            14, 17, 20,
            14, 20, 23
        };

        private static readonly int[] NegativeYFace =
        {
            22, 19, 4,
            22, 4, 1
        };

        private static readonly int[] PositiveYFace =
        {
            10, 7, 16,
            10, 16, 13
        };

        private static readonly int[] NegativeZFace =
        {
            21, 0, 9,
            21, 9, 12
        };

        private static readonly int[] PositiveZFace =
        {
            3, 18, 15,
            3, 15, 6
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