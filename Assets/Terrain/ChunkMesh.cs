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
            var cornersBuffer = new OrderedSet<Corner>();
            var trianglesBuffer = new List<int>();

            var dirtUV1 = new Vector2(0, 0);
            var grassUV1 = new Vector2(1, 0);

            foreach (var block in chunk)
            {
                if (block.Value == 0) continue;
                
                var faceRenderFlags = Faces.None;
                if (chunk.BlockAt(block.Key + new Vector3Int(-1, 0, 0)) <= 0)
                    faceRenderFlags |= Faces.NegativeX;
                if (chunk.BlockAt(block.Key + new Vector3Int(1, 0, 0)) <= 0)
                    faceRenderFlags |= Faces.PositiveX;
                if (chunk.BlockAt(block.Key + new Vector3Int(0, -1, 0)) <= 0)
                    faceRenderFlags |= Faces.NegativeY;
                if (chunk.BlockAt(block.Key + new Vector3Int(0, 1, 0)) <= 0)
                    faceRenderFlags |= Faces.PositiveY;
                if (chunk.BlockAt(block.Key + new Vector3Int(0, 0, -1)) <= 0)
                    faceRenderFlags |= Faces.NegativeZ;
                if (chunk.BlockAt(block.Key + new Vector3Int(0, 0, 1)) <= 0)
                    faceRenderFlags |= Faces.PositiveZ;

                var uv1 = chunk.BlockAt(block.Key + new Vector3Int(0, 1, 0)) <= 0 ? grassUV1: dirtUV1;
                MakeBlockMesh(block.Key.x, block.Key.y, block.Key.z, cornersBuffer, trianglesBuffer,
                    faceRenderFlags, uv1);
            }

            var mesh = meshFilter.mesh;
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.Clear();
            mesh.vertices = cornersBuffer.Select(x => x.Vertex).ToArray();
            mesh.triangles = trianglesBuffer.ToArray();
            mesh.normals = cornersBuffer.Select(x => x.Normal).ToArray();
            mesh.uv = cornersBuffer.Select(x => x.uv0).ToArray();
            mesh.uv2 = cornersBuffer.Select(x => x.uv2).ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();
        }

        private static void MakeBlockMesh(int x, int y, int z, OrderedSet<Corner> cornersBuffer,
            List<int> trianglesBuffer,
            Faces facesToRender, Vector2 uv1)
        {
            Corner[] corners =
            {
                new Corner(new Vector3(x, y, z), Normals.NegativeZ, UVs.BottomLeft, uv1),
                new Corner(new Vector3(x, y, z), Normals.NegativeY, UVs.BottomLeft, uv1),
                new Corner(new Vector3(x, y, z), Normals.NegativeX, UVs.BottomLeft, uv1),
                new Corner(new Vector3(x, y, z + 1), Normals.PositiveZ, UVs.TopRight, uv1),
                new Corner(new Vector3(x, y, z + 1), Normals.NegativeY, UVs.BottomRight, uv1),
                new Corner(new Vector3(x, y, z + 1), Normals.NegativeX, UVs.BottomRight, uv1),
                new Corner(new Vector3(x, y + 1, z + 1), Normals.PositiveZ, UVs.TopLeft, uv1),
                new Corner(new Vector3(x, y + 1, z + 1), Normals.PositiveY, UVs.BottomLeft, uv1),
                new Corner(new Vector3(x, y + 1, z + 1), Normals.NegativeX, UVs.TopRight, uv1),
                new Corner(new Vector3(x, y + 1, z), Normals.NegativeZ, UVs.BottomRight, uv1),
                new Corner(new Vector3(x, y + 1, z), Normals.PositiveY, UVs.BottomRight, uv1),
                new Corner(new Vector3(x, y + 1, z), Normals.NegativeX, UVs.TopLeft, uv1),
                new Corner(new Vector3(x + 1, y + 1, z), Normals.NegativeZ, UVs.TopRight, uv1),
                new Corner(new Vector3(x + 1, y + 1, z), Normals.PositiveY, UVs.TopRight, uv1),
                new Corner(new Vector3(x + 1, y + 1, z), Normals.PositiveX, UVs.BottomLeft, uv1),
                new Corner(new Vector3(x + 1, y + 1, z + 1), Normals.PositiveZ, UVs.BottomLeft, uv1),
                new Corner(new Vector3(x + 1, y + 1, z + 1), Normals.PositiveY, UVs.TopLeft, uv1),
                new Corner(new Vector3(x + 1, y + 1, z + 1), Normals.PositiveX, UVs.BottomRight, uv1),
                new Corner(new Vector3(x + 1, y, z + 1), Normals.PositiveZ, UVs.BottomRight, uv1),
                new Corner(new Vector3(x + 1, y, z + 1), Normals.NegativeY, UVs.TopRight, uv1),
                new Corner(new Vector3(x + 1, y, z + 1), Normals.PositiveX, UVs.TopRight, uv1),
                new Corner(new Vector3(x + 1, y, z), Normals.NegativeZ, UVs.TopLeft, uv1),
                new Corner(new Vector3(x + 1, y, z), Normals.NegativeY, UVs.TopLeft, uv1),
                new Corner(new Vector3(x + 1, y, z), Normals.PositiveX, UVs.TopLeft, uv1)
            };

            cornersBuffer.Add(corners);

            void RenderIfShouldRender(Faces face)
            {
                if (facesToRender.HasFlag(face))
                    trianglesBuffer.AddRange(Corners.For[face]
                        .Select(corner => cornersBuffer.IndexOf(corners[corner])));
            }

            RenderIfShouldRender(Faces.PositiveX);
            RenderIfShouldRender(Faces.NegativeX);
            RenderIfShouldRender(Faces.PositiveY);
            RenderIfShouldRender(Faces.NegativeY);
            RenderIfShouldRender(Faces.PositiveZ);
            RenderIfShouldRender(Faces.NegativeZ);
        }
    }

    internal class Corner
    {
        public Corner(Vector3 vertex, Vector3 normal, Vector2 uv0, Vector2 uv2)
        {
            Vertex = vertex;
            Normal = normal;
            this.uv0 = uv0;
            this.uv2 = uv2;
        }

        public Vector3 Normal { get; }
        public Vector3 Vertex { get; }
        // ReSharper disable once InconsistentNaming
        public Vector2 uv0 { get; }
        // ReSharper disable once InconsistentNaming
        public Vector2 uv2 { get; }

        public override int GetHashCode()
        {
            return Normal.GetHashCode() ^ (Vertex.GetHashCode() << 2) ^ (uv0.GetHashCode() << 4);
        }
    }

    public static class Normals
    {
        public static Vector3 NegativeZ = new Vector3(0, 0, -1);
        public static Vector3 NegativeY = new Vector3(0, -1, 0);
        public static Vector3 NegativeX = new Vector3(-1, 0, 0);
        public static Vector3 PositiveZ = new Vector3(0, 0, 1);
        public static Vector3 PositiveY = new Vector3(0, 1, 0);
        public static Vector3 PositiveX = new Vector3(1, 0, 0);
    }

    public static class UVs
    {
        public static readonly Vector2 BottomLeft = new Vector2(0, 0);
        public static readonly Vector2 BottomRight = new Vector2(1, 0);
        public static readonly Vector2 TopLeft = new Vector2(0, 1);
        public static readonly Vector2 TopRight = new Vector2(1, 1);
    }
}