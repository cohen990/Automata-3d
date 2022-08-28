using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering;

namespace Terrain.Mesh
{
    public class ChunkMesh
    {
        private static readonly Vector2 DirtTextureLocation = new Vector2(0, 0);
        private static readonly Vector2 GrassTextureLocation = new Vector2(1, 0);
        private static readonly Vector2 StoneTextureLocation = new Vector2(0, 1);
        private static readonly Vector2 BedrockTextureLocation = new Vector2(1, 1);
        
        private readonly MeshFilter _filter;
        private readonly Chunk _chunk;
        private readonly World _world;
        private readonly Dictionary<Vector3Int, BlockMesh> _blockMeshes;
        private readonly CornerBuffer _cornersBuffer;

        private ChunkMesh(Dictionary<Vector3Int, BlockMesh> blockMeshes, CornerBuffer cornersBuffer,
            MeshFilter filter, Chunk chunk, World world)
        {
            _blockMeshes = blockMeshes;
            _cornersBuffer = cornersBuffer;
            _filter = filter;
            _chunk = chunk;
            _world = world;
        }

        public static ChunkMesh Generate(MeshFilter meshFilter, Chunk chunk, World world)
        {
            var cornersBuffer = new CornerBuffer();
            var trianglesBuffer = new List<int>();
            var blockMeshes = new Dictionary<Vector3Int, BlockMesh>();

            foreach (var block in chunk)
            {
                var blockPosition = block.Key;
                
                var faceRenderFlags = FaceRenderFlags(world, blockPosition);
                var textureLocation = TextureLocation(chunk, blockPosition);

                var generatedBlock = BlockMesh.Generate(blockPosition.x, blockPosition.y, blockPosition.z, cornersBuffer, trianglesBuffer,
                    faceRenderFlags, textureLocation);
                blockMeshes[blockPosition] = generatedBlock;
            }

            var mesh = meshFilter.mesh;
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.Clear();
            var decomposed = cornersBuffer.Decompose();
            mesh.vertices = decomposed.Vertices;
            mesh.triangles = trianglesBuffer.ToArray();
            mesh.normals = decomposed.Normals;
            mesh.uv = decomposed.UV;
            mesh.uv2 = decomposed.UV2;

            return new ChunkMesh(blockMeshes, cornersBuffer, meshFilter, chunk, world);
        }
        
        public void UpdateBlock(Vector3Int blockPosition)
        {
            var mesh = _filter.mesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            var uv = mesh.uv;
            var uv2 = mesh.uv2;
            var normals = mesh.normals;
            
            mesh.Clear();
            mesh.vertices = vertices;
            
            RemoveSingleBlock(blockPosition, triangles);
            UpdateSingleBlock(blockPosition + new Vector3Int(1, 0, 0), triangles);
            UpdateSingleBlock(blockPosition + new Vector3Int(-1, 0, 0), triangles);
            UpdateSingleBlock(blockPosition + new Vector3Int(0, 1, 0), triangles);
            UpdateSingleBlock(blockPosition + new Vector3Int(0, -1, 0), triangles);
            UpdateSingleBlock(blockPosition + new Vector3Int(0, 0, 1), triangles);
            UpdateSingleBlock(blockPosition + new Vector3Int(0, 0, -1), triangles);
            
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.uv2 = uv2;
        }

        public void UpdateSingleBlock(Vector3Int blockPosition)
        {
            var mesh = _filter.mesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            var uv = mesh.uv;
            var uv2 = mesh.uv2;
            var normals = mesh.normals;
            
            mesh.Clear();
            mesh.vertices = vertices;
            
            UpdateSingleBlock(blockPosition, triangles);
            
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.uv2 = uv2;
            
        }

        private void UpdateSingleBlock(Vector3Int blockPosition, IList<int> triangles)
        {
            var blockMeshExists = _blockMeshes.TryGetValue(blockPosition, out var blockMesh);
            if (!blockMeshExists) return;
            
            var uv2 = TextureLocation(_chunk, blockPosition);

            var blockTriangles = BlockMesh.CalculateTriangles(_cornersBuffer, FaceRenderFlags(_world, blockPosition),
                Corners.Calculate(blockPosition, uv2));
            for (var i = 0; i < blockTriangles.Length; i++)
            {
                triangles[i + blockMesh.TrianglesStart] = blockTriangles[i];
            }
        }
        
        private void RemoveSingleBlock(Vector3Int blockPosition, IList<int> triangles)
        {
            var blockMesh = _blockMeshes[blockPosition];
            for (var i = blockMesh.TrianglesStart; i < blockMesh.TrianglesStart + blockMesh.TrianglesCount; i++)
            {
                triangles[i] = 0;
            }
        }

        private static Vector2 TextureLocation(Chunk chunk, Vector3Int blockPosition)
        {
            var blockId = chunk.BlockAt(blockPosition);
            var textureLocation = Vector2.negativeInfinity;
            switch (blockId)
            {
                case Block.STONE:
                    textureLocation = StoneTextureLocation;
                    break;
                case Block.GRASS:
                    textureLocation = GrassTextureLocation;
                    break;
                case Block.DIRT:
                    textureLocation = DirtTextureLocation;
                    break;
                case Block.BEDROCK:
                    textureLocation = BedrockTextureLocation;
                    break;
            }

            return textureLocation;
        }

        private static Faces FaceRenderFlags(World world, Vector3Int blockPosition)
        {
            var faceRenderFlags = Faces.None;
            if (Block.IsEmpty(world.BlockAt(blockPosition)))
                return faceRenderFlags;
            
            if (world.BlockAt(blockPosition + new Vector3Int(-1, 0, 0)) <= 0)
                faceRenderFlags |= Faces.NegativeX;
            if (world.BlockAt(blockPosition + new Vector3Int(1, 0, 0)) <= 0)
                faceRenderFlags |= Faces.PositiveX;
            if (world.BlockAt(blockPosition + new Vector3Int(0, -1, 0)) <= 0)
                faceRenderFlags |= Faces.NegativeY;
            if (world.BlockAt(blockPosition + new Vector3Int(0, 1, 0)) <= 0)
                faceRenderFlags |= Faces.PositiveY;
            if (world.BlockAt(blockPosition + new Vector3Int(0, 0, -1)) <= 0)
                faceRenderFlags |= Faces.NegativeZ;
            if (world.BlockAt(blockPosition + new Vector3Int(0, 0, 1)) <= 0)
                faceRenderFlags |= Faces.PositiveZ;
            return faceRenderFlags;
        }
    }
}

