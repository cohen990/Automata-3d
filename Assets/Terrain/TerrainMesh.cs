using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Terrain
{
    public class TerrainMesh : MonoBehaviour
    {
        private void Start()
        {
            var verticesBuffer = new OrderedSet<Vector3>();
            var trianglesBuffer = new List<int>();
            const int chunkBoundsX = 16;
            const int chunkBoundsY = 64;
            const int chunkBoundsZ = 16;
            for (var x = 0; x < chunkBoundsX; x++)
            for (var y = 0; y < chunkBoundsY; y++)
            for (var z = 0; z < chunkBoundsZ; z++)
            {
                var faceRenderFlags = Faces.None;
                if (x == 0) faceRenderFlags |= Faces.NegativeX;
                if (x == chunkBoundsX - 1) faceRenderFlags |= Faces.PositiveX;
                if (y == 0) faceRenderFlags |= Faces.NegativeY;
                if (y == chunkBoundsY - 1) faceRenderFlags |= Faces.PositiveY;
                if (z == 0) faceRenderFlags |= Faces.NegativeZ;
                if (z == chunkBoundsZ - 1) faceRenderFlags |= Faces.PositiveZ;

                MakeBlock(x, y, z, verticesBuffer, trianglesBuffer, faceRenderFlags);
            }

            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.Clear();
            mesh.vertices = verticesBuffer.ToArray();
            mesh.triangles = trianglesBuffer.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();
        }

        private static void MakeBlock(int x, int y, int z, OrderedSet<Vector3> verticesBuffer,
            List<int> trianglesBuffer,
            Faces facesToRender)
        {
            Vector3[] vertices =
            {
                new Vector3(x, y, z),
                new Vector3(x, y, z + 1),
                new Vector3(x, y + 1, z + 1),
                new Vector3(x, y + 1, z),
                new Vector3(x + 1, y + 1, z),
                new Vector3(x + 1, y + 1, z + 1),
                new Vector3(x + 1, y, z + 1),
                new Vector3(x + 1, y, z)
            };

            verticesBuffer.Add(vertices);

            void RenderIfShouldRender(Faces face)
            {
                if (facesToRender.HasFlag(face))
                    trianglesBuffer.AddRange(Corners.For[face]
                        .Select(corner => verticesBuffer.IndexOf(vertices[corner])));
            }

            RenderIfShouldRender(Faces.PositiveX);
            RenderIfShouldRender(Faces.NegativeX);
            RenderIfShouldRender(Faces.PositiveY);
            RenderIfShouldRender(Faces.NegativeY);
            RenderIfShouldRender(Faces.PositiveZ);
            RenderIfShouldRender(Faces.NegativeZ);
        }
    }
}