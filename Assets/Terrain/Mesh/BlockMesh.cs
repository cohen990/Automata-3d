using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Terrain.Mesh
{
    public class BlockMesh
    {
        public readonly int TrianglesStart;
        public readonly int TrianglesCount;

        private BlockMesh(int trianglesStart, int trianglesCount)
        {
            TrianglesStart = trianglesStart;
            TrianglesCount = trianglesCount;
        }

        public static BlockMesh Generate(int x, int y, int z, CornerBuffer cornersBuffer,
            List<int> trianglesBuffer,
            Faces facesToRender, Vector2 uv2)
        {
            var trianglesStart = trianglesBuffer.Count;
            
            var corners = Corners.Calculate(x, y, z, uv2);
            cornersBuffer.Add(corners);

            var triangles = CalculateTriangles(cornersBuffer, facesToRender, corners);
            trianglesBuffer.AddRange(triangles);

            return new BlockMesh(trianglesStart, triangles.Length);
        }

        public static int[] CalculateTriangles(CornerBuffer cornersBuffer, Faces facesToRender, Corner[] corners)
        {
            var triangles = new int[Corners.NumberPerTriangle * 6];

            var faceNumber = 0;
            void RenderIfShouldRender(Faces face)
            {
                var trianglesLocalVertex = facesToRender.HasFlag(face) ? Corners.For[face] : Corners.For[Faces.None];

                var triangleGlobalVertices = TrianglesGlobalVertices(trianglesLocalVertex, cornersBuffer, corners).ToArray();
                var triangleStartIndex = faceNumber * Corners.NumberPerTriangle;
                var triangleEndIndex = triangleGlobalVertices.Length + triangleStartIndex;
                for(var i = triangleStartIndex ; i < triangleEndIndex; i++)
                {
                    triangles[i] = triangleGlobalVertices[i - triangleStartIndex];
                }

                faceNumber++;
            }

            RenderIfShouldRender(Faces.PositiveX);
            RenderIfShouldRender(Faces.NegativeX);
            RenderIfShouldRender(Faces.PositiveY);
            RenderIfShouldRender(Faces.NegativeY);
            RenderIfShouldRender(Faces.PositiveZ);
            RenderIfShouldRender(Faces.NegativeZ);
            return triangles;
        }

        private static IEnumerable<int> TrianglesGlobalVertices(IEnumerable<int> trianglesLocalVertex, CornerBuffer cornersBuffer, IReadOnlyList<Corner> corners)
        {
            var trianglesGlobalVertices = trianglesLocalVertex
                .Select(localVertex =>
                {
                    cornersBuffer.TryGetIndexOf(corners[localVertex], out var index);
                    return index;
                });
            return trianglesGlobalVertices;
        }
    }
}