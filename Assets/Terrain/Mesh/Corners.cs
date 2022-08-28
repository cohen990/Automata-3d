using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Mesh
{
    public static class Corners
    {
        public static readonly int NumberPerTriangle = 6;
        
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

        private static readonly int[] DontRender =
        {
            0, 0, 0,
            0, 0, 0
        };
        
        public static readonly Dictionary<Faces, int[]> For = new Dictionary<Faces, int[]>
        {
            {Faces.PositiveX, PositiveXFace},
            {Faces.NegativeX, NegativeXFace},
            {Faces.PositiveY, PositiveYFace},
            {Faces.NegativeY, NegativeYFace},
            {Faces.PositiveZ, PositiveZFace},
            {Faces.NegativeZ, NegativeZFace},
            {Faces.None, DontRender}
        };
        
        public static Corner[] Calculate(int x, int y, int z, Vector2 uv2)
        {
            Corner[] corners =
            {
                new Corner(new Vector3(x, y, z), Normals.NegativeZ, UVs.BottomLeft, uv2),
                new Corner(new Vector3(x, y, z), Normals.NegativeY, UVs.BottomLeft, uv2),
                new Corner(new Vector3(x, y, z), Normals.NegativeX, UVs.BottomLeft, uv2),
                new Corner(new Vector3(x, y, z + 1), Normals.PositiveZ, UVs.TopRight, uv2),
                new Corner(new Vector3(x, y, z + 1), Normals.NegativeY, UVs.BottomRight, uv2),
                new Corner(new Vector3(x, y, z + 1), Normals.NegativeX, UVs.BottomRight, uv2),
                new Corner(new Vector3(x, y + 1, z + 1), Normals.PositiveZ, UVs.TopLeft, uv2),
                new Corner(new Vector3(x, y + 1, z + 1), Normals.PositiveY, UVs.BottomLeft, uv2),
                new Corner(new Vector3(x, y + 1, z + 1), Normals.NegativeX, UVs.TopRight, uv2),
                new Corner(new Vector3(x, y + 1, z), Normals.NegativeZ, UVs.BottomRight, uv2),
                new Corner(new Vector3(x, y + 1, z), Normals.PositiveY, UVs.BottomRight, uv2),
                new Corner(new Vector3(x, y + 1, z), Normals.NegativeX, UVs.TopLeft, uv2),
                new Corner(new Vector3(x + 1, y + 1, z), Normals.NegativeZ, UVs.TopRight, uv2),
                new Corner(new Vector3(x + 1, y + 1, z), Normals.PositiveY, UVs.TopRight, uv2),
                new Corner(new Vector3(x + 1, y + 1, z), Normals.PositiveX, UVs.BottomLeft, uv2),
                new Corner(new Vector3(x + 1, y + 1, z + 1), Normals.PositiveZ, UVs.BottomLeft, uv2),
                new Corner(new Vector3(x + 1, y + 1, z + 1), Normals.PositiveY, UVs.TopLeft, uv2),
                new Corner(new Vector3(x + 1, y + 1, z + 1), Normals.PositiveX, UVs.BottomRight, uv2),
                new Corner(new Vector3(x + 1, y, z + 1), Normals.PositiveZ, UVs.BottomRight, uv2),
                new Corner(new Vector3(x + 1, y, z + 1), Normals.NegativeY, UVs.TopRight, uv2),
                new Corner(new Vector3(x + 1, y, z + 1), Normals.PositiveX, UVs.TopRight, uv2),
                new Corner(new Vector3(x + 1, y, z), Normals.NegativeZ, UVs.TopLeft, uv2),
                new Corner(new Vector3(x + 1, y, z), Normals.NegativeY, UVs.TopLeft, uv2),
                new Corner(new Vector3(x + 1, y, z), Normals.PositiveX, UVs.TopLeft, uv2)
            };
            return corners;
        }

        public static Corner[] Calculate(Vector3Int blockPosition, Vector2 uv2)
        {
            return Calculate(blockPosition.x, blockPosition.y, blockPosition.z, uv2);
        }
    }
}