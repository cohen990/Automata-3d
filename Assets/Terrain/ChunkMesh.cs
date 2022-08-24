using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Terrain
{
    public static class ChunkMesh
    {
        public static void Generate(MeshFilter meshFilter, Chunk chunk)
        {
            var verticesBuffer = new OrderedSet<Vector3>();
            var trianglesBuffer = new List<int>();
            var normalsBuffer = new List<Vector3>();

            foreach (var block in chunk)
            {
                if (block.Value == 0) continue;

                var faceRenderFlags = Faces.None;
                if (block.Key.x == chunk.Bounds.xMin || chunk.BlockAt(block.Key + new Vector3Int(-1, 0, 0)) == 0)
                    faceRenderFlags |= Faces.NegativeX;
                if (block.Key.x == chunk.Bounds.xMax - 1 || chunk.BlockAt(block.Key + new Vector3Int(1, 0, 0)) == 0)
                    faceRenderFlags |= Faces.PositiveX;
                if (block.Key.y == chunk.Bounds.yMin || chunk.BlockAt(block.Key + new Vector3Int(0, -1, 0)) == 0)
                    faceRenderFlags |= Faces.NegativeY;
                if (block.Key.y == chunk.Bounds.yMax - 1 || chunk.BlockAt(block.Key + new Vector3Int(0, 1, 0)) == 0)
                    faceRenderFlags |= Faces.PositiveY;
                if (block.Key.z == chunk.Bounds.zMin || chunk.BlockAt(block.Key + new Vector3Int(0, 0, -1)) == 0)
                    faceRenderFlags |= Faces.NegativeZ;
                if (block.Key.z == chunk.Bounds.zMax - 1 || chunk.BlockAt(block.Key + new Vector3Int(0, 0, 1)) == 0)
                    faceRenderFlags |= Faces.PositiveZ;

                MakeBlockMesh(block.Key.x, block.Key.y, block.Key.z, verticesBuffer, trianglesBuffer, normalsBuffer,
                    faceRenderFlags);
            }

            var mesh = meshFilter.mesh;
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.Clear();
            mesh.vertices = verticesBuffer.ToArray();
            mesh.triangles = trianglesBuffer.ToArray();
            mesh.normals = normalsBuffer.ToArray();

            var uvs = new Vector2[mesh.vertices.Length];

            for (var i = 0; i < uvs.Length; i++)
            {
                var thing = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
                uvs[i] = thing;
            }

            mesh.uv = uvs;
            mesh.Optimize();
            // mesh.RecalculateNormals();
        }

        private static void MakeBlockMesh(int x, int y, int z, OrderedSet<Vector3> verticesBuffer,
            List<int> trianglesBuffer, List<Vector3> normalsBuffer,
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
            normalsBuffer.AddRange(new List<Vector3>
            {
                new Vector3(-1, -1, -1),
                new Vector3(-1, -1, 1),
                new Vector3(-1, 1, 1),
                new Vector3(-1, 1, -1),
                new Vector3(1, 1, -1),
                new Vector3(1, 1, 1),
                new Vector3(1, -1, 1),
                new Vector3(1, -1, -1)
            });

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