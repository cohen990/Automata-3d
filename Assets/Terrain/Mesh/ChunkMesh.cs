using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GameEngine;
using UnityEngine;
using UnityEngine.Rendering;

namespace Terrain.Mesh
{
    public class ChunkMesh
    {
        private readonly MeshFilter _filter;
        private readonly Chunk _chunk;
        private readonly World _world;
        public readonly Dictionary<Vector3Int, BlockMesh> BlockMeshes;
        private readonly CornerBuffer _cornersBuffer;
        private static ChunkMesh _latest;

        private ChunkMesh(Dictionary<Vector3Int, BlockMesh> blockMeshes, CornerBuffer cornersBuffer,
            MeshFilter filter, Chunk chunk, World world)
        {
            BlockMeshes = blockMeshes;
            _cornersBuffer = cornersBuffer;
            _filter = filter;
            _chunk = chunk;
            _world = world;
        }

        public static ChunkMesh Latest => _latest;

        public static IEnumerator Generate(MeshFilter meshFilter, Chunk chunk, World world)
        {
            var cornersBuffer = new CornerBuffer();
            var trianglesBuffer = new List<int>();
            var blockMeshes = new Dictionary<Vector3Int, BlockMesh>();
            var timer = Stopwatch.StartNew();

            for (var blockIndex = 0; blockIndex < chunk.BlockCount; blockIndex++)
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

                if (timer.ElapsedMilliseconds <= FrameTimer.MAXIMUM_MILLISECONDS_PER_COMPUTATION_BATCH) continue;
                timer.Restart();
                yield return null;
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
                    Vector3Int.FloorToInt((meshVertices[meshTriangles[i]] + meshVertices[meshTriangles[i + 1]] +
                                           meshVertices[meshTriangles[i + 2]]) / 3);
                var normal = Vector3Int.FloorToInt(flooredAverage - meshNormals[meshTriangles[i]]);
                var coord = Vector3Int.Min(flooredAverage, normal);
                
                blockMeshes.TryAdd(coord, new  BlockMesh());
                blockMeshes[coord].AddTriangle(i);
                
                if (timer.ElapsedMilliseconds <= FrameTimer.MAXIMUM_MILLISECONDS_PER_COMPUTATION_BATCH) continue;
                timer.Restart();
                yield return null;
            }

            cornersBuffer.Clear();
            var meshUV = mesh.uv;
            var meshUV2 = mesh.uv2;
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                cornersBuffer.Add(new Corner(meshVertices[i], meshNormals[i], meshUV[i], meshUV2[i]));
                if (timer.ElapsedMilliseconds <= FrameTimer.MAXIMUM_MILLISECONDS_PER_COMPUTATION_BATCH) continue;
                timer.Restart();
                yield return null;
            }

            _latest = new ChunkMesh(blockMeshes, cornersBuffer, meshFilter, chunk, world);
        }


        public void UpdateBlock(Vector3Int blockPosition)
        {
            var mesh = _filter.sharedMesh;
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
            if (!BlockMeshes.ContainsKey(blockPosition)) return;
            
            var mesh = _filter.mesh;
            var triangles = mesh.triangles;
            
            mesh.Clear();
            
            UpdateSingleBlockInternal(blockPosition, triangles);
            
            var decomposed = _cornersBuffer.Decompose();
            mesh.vertices = decomposed.Vertices;
            mesh.triangles = triangles;
            mesh.normals = decomposed.Normals;
            mesh.uv = decomposed.UV;
            mesh.uv2 = decomposed.UV2;
        }

        private void UpdateSingleBlockInternal(Vector3Int blockPosition, IList<int> triangles)
        {
            if (_chunk.IsOutOfBounds(blockPosition)) return;
            
            BlockMeshes.TryAdd(blockPosition, new BlockMesh());
            var blockMesh = BlockMeshes[blockPosition];
            
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
                Block.STONE => TextureLocations.Stone,
                Block.GRASS => TextureLocations.Grass,
                Block.DIRT => TextureLocations.Dirt,
                Block.BEDROCK => TextureLocations.Bedrock,
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

