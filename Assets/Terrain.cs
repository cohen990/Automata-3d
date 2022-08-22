using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    private void Start()
    {
        var verticesBuffer = new OrderedSet<Vector3>();
        var trianglesBuffer = new List<int>();
        const int gridBounds = 2;
        for (var x = 0; x < gridBounds; x++)
        for (var y = 0; y < gridBounds; y++)
        for (var z = 0; z < gridBounds; z++)
        {
            var faceRenderFlags = Faces.None;
            if (x == 0) faceRenderFlags |= Faces.NegativeX;
            if (x == gridBounds - 1) faceRenderFlags |= Faces.PositiveX;
            if (y == 0) faceRenderFlags |= Faces.NegativeY;
            if (y == gridBounds - 1) faceRenderFlags |= Faces.PositiveY;
            if (z == 0) faceRenderFlags |= Faces.NegativeZ;
            if (z == gridBounds - 1) faceRenderFlags |= Faces.PositiveZ;

            MakeBlock(x, y, z, verticesBuffer, trianglesBuffer, faceRenderFlags);
        }

        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = verticesBuffer.ToArray();
        mesh.triangles = trianglesBuffer.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    private void MakeBlock(int x, int y, int z, OrderedSet<Vector3> verticesBuffer, List<int> trianglesBuffer,
        Faces facesToRender)
    {
        Vector3[] vertices =
        {
            new Vector3(x, y, z),
            new Vector3(x + 1, y, z),
            new Vector3(x + 1, y + 1, z),
            new Vector3(x, y + 1, z),
            new Vector3(x, y + 1, z + 1),
            new Vector3(x + 1, y + 1, z + 1),
            new Vector3(x + 1, y, z + 1),
            new Vector3(x, y, z + 1)
        };


        verticesBuffer.Add(vertices);


        void RenderIfShouldRender(Faces face)
        {
            if (facesToRender.HasFlag(face))
                trianglesBuffer.AddRange(Corners.For[face].Select(corner => verticesBuffer.IndexOf(vertices[corner])));
        }

        RenderIfShouldRender(Faces.PositiveX);
        RenderIfShouldRender(Faces.NegativeX);
        RenderIfShouldRender(Faces.PositiveY);
        RenderIfShouldRender(Faces.NegativeY);
        RenderIfShouldRender(Faces.PositiveZ);
        RenderIfShouldRender(Faces.NegativeZ);
    }

    [Flags]
    private enum Faces
    {
        None = 0,
        NegativeX = 1,
        PositiveX = 2,
        NegativeY = 4,
        PositiveY = 8,
        NegativeZ = 16,
        PositiveZ = 32
    }

    private static class Corners
    {
        private static readonly int[] NegativeXFace =
        {
            0, 2, 1,
            0, 3, 2
        };

        private static readonly int[] PositiveYFace =
        {
            2, 3, 4,
            2, 4, 5
        };

        private static readonly int[] PositiveZFace =
        {
            1, 2, 5,
            1, 5, 6
        };

        private static readonly int[] NegativeZFace =
        {
            0, 7, 4,
            0, 4, 3
        };

        private static readonly int[] PositiveXFace =
        {
            5, 4, 7,
            5, 7, 6
        };

        private static readonly int[] NegativeYFace =
        {
            0, 6, 7,
            0, 1, 6
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