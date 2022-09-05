using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Terrain.Mesh
{
    public class ChunkMesh
    {
        private static readonly Vector2 DirtTextureLocation = new(0, 0);
        private static readonly Vector2 GrassTextureLocation = new(1, 0);
        private static readonly Vector2 StoneTextureLocation = new(0, 1);
        private static readonly Vector2 BedrockTextureLocation = new(1, 1);
        
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

            for (var blockIndex = 0; blockIndex < chunk.BlockCount; blockIndex++ )
            {
                var blockPosition = chunk.Vector3PositionOf(blockIndex);
                
                var faceRenderFlags = FaceRenderFlags(world, blockPosition);
                var textureLocation = TextureLocation(chunk, blockPosition);

                BlockMesh.Generate(
                    blockPosition.x, 
                    blockPosition.y, 
                    blockPosition.z, 
                    cornersBuffer, 
                    trianglesBuffer,
                    faceRenderFlags, 
                    textureLocation);
            }

            var decomposed = cornersBuffer.Decompose();
            var mesh = new UnityEngine.Mesh
            {
                indexFormat = IndexFormat.UInt32,
                vertices = decomposed.Vertices,
                triangles = trianglesBuffer.ToArray(),
                normals = decomposed.Normals,
                uv = decomposed.UV,
                uv2 = decomposed.UV2
            };
            mesh.Optimize();
            meshFilter.sharedMesh = mesh;
            var meshVertices = mesh.vertices;
            var meshTriangles = mesh.triangles;
            var meshNormals = mesh.normals;

            for (var i = 0; i < meshTriangles.Length; i += 3)
            {
                var flooredAverage =
                    Vector3Int.FloorToInt((meshVertices[meshTriangles[i]] + meshVertices[meshTriangles[i + 1]] + meshVertices[meshTriangles[i + 2]])/3);
                var normal = Vector3Int.FloorToInt(flooredAverage - meshNormals[meshTriangles[i]]);
                var coord = Vector3Int.Min(flooredAverage, normal);
                if (!blockMeshes.ContainsKey(coord))
                {
                    blockMeshes[coord] = new BlockMesh();
                }

                blockMeshes[coord].AddTriangle(i);
            }

            cornersBuffer.Clear();
            var meshUV = mesh.uv;
            var meshUV2 = mesh.uv2;
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                cornersBuffer.Add(new Corner(meshVertices[i], meshNormals[i], meshUV[i], meshUV2[i]));
            }
            
            return new ChunkMesh(blockMeshes, cornersBuffer, meshFilter, chunk, world);
        }
        
        public void UpdateBlock(Vector3Int blockPosition)
        {
            var mesh = _filter.mesh;
            var triangles = mesh.triangles;
            
            UpdateSingleBlockInternal(blockPosition, triangles);
            UpdateSingleBlockInternal(blockPosition + new Vector3Int(1, 0, 0), triangles);
            UpdateSingleBlockInternal(blockPosition + new Vector3Int(-1, 0, 0), triangles);
            UpdateSingleBlockInternal(blockPosition + new Vector3Int(0, 1, 0), triangles);
            UpdateSingleBlockInternal(blockPosition + new Vector3Int(0, -1, 0), triangles);
            UpdateSingleBlockInternal(blockPosition + new Vector3Int(0, 0, 1), triangles);
            UpdateSingleBlockInternal(blockPosition + new Vector3Int(0, 0, -1), triangles);
            
            mesh.Clear();
            var decomposed = _cornersBuffer.Decompose();
            mesh.vertices = decomposed.Vertices;
            mesh.triangles = triangles;
            mesh.normals = decomposed.Normals;
            mesh.uv = decomposed.UV;
            mesh.uv2 = decomposed.UV2;
        }

        public void UpdateSingleBlock(Vector3Int blockPosition)
        {
            if (!_blockMeshes.ContainsKey(blockPosition)) return;
            
            var mesh = _filter.mesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            var uv = mesh.uv;
            var uv2 = mesh.uv2;
            var normals = mesh.normals;
            
            mesh.Clear();
            mesh.vertices = vertices;
            
            UpdateSingleBlockInternal(blockPosition, triangles);
            
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.uv2 = uv2;
            
        }

        private void UpdateSingleBlockInternal(Vector3Int blockPosition, IList<int> triangles)
        {
            var blockMeshExists = _blockMeshes.TryGetValue(blockPosition, out var blockMesh);
            if (!blockMeshExists) return;
            
            var textureLocation = TextureLocation(_chunk, blockPosition);

            var blockTriangles = BlockMesh.CalculateTriangles(_cornersBuffer, FaceRenderFlags(_world, blockPosition),
                Corners.Calculate(blockPosition, textureLocation));
            
            for (var i = 0; i < blockTriangles.Length; i++)
            {
                triangles[blockMesh.Triangle(i)] = blockTriangles[i];
            }
        }

        private static Vector2 TextureLocation(Chunk chunk, Vector3Int blockPosition)
        {
            var blockId = chunk.BlockAt(blockPosition);
            var textureLocation = blockId switch
            {
                Block.STONE => StoneTextureLocation,
                Block.GRASS => GrassTextureLocation,
                Block.DIRT => DirtTextureLocation,
                Block.BEDROCK => BedrockTextureLocation,
                Block.AIR => Vector2.zero,
                _ => Vector2.negativeInfinity
            };

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

