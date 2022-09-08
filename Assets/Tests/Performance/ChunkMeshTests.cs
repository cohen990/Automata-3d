using System.Collections.Generic;
using NUnit.Framework;
using Terrain;
using Terrain.Mesh;
using UnityEngine;

namespace Tests.Performance
{
    [SetUpFixture]
    public class ChunkMeshTestsSetup
    {
        public static PerformanceTests PerformanceTesting;
        
        [OneTimeSetUp]
        public void SetUp()
        {
            PerformanceTesting = new PerformanceTests(System.DateTime.Now.ToString("o"));
        }
    }
    
    public class ChunkMeshTests
    {
        [Test]
        public void Performance_test_of_1x1_mesh_generation()
        {
            var gameObject = new GameObject();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            const int size = 1;
            var bounds = new BoundsInt(0, 0, 0, size, size, size);
            var chunk = new Chunk(bounds);
            for(var x = bounds.xMin; x < bounds.xMax; x++)
            for(var y = bounds.yMin; y < bounds.yMax; y++)
            for(var z = bounds.zMin; z < bounds.zMax; z++)
                chunk.SetBlock(new Vector3Int(x, y, z), Block.DIRT);
            
            var world = new World(1);
            world.SetChunk(new Vector2Int(0, 0), chunk);
            
            ChunkMeshTestsSetup.PerformanceTesting.Measure(() => ChunkMesh.Generate(meshFilter, chunk, world), 5);
        }
        
        [Test]
        public void Performance_test_of_10x10_mesh_generation()
        {
            var gameObject = new GameObject();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            const int size = 10;
            var bounds = new BoundsInt(0, 0, 0, size, size, size);
            var chunk = new Chunk(bounds);
            for(var x = bounds.xMin; x < bounds.xMax; x++)
            for(var y = bounds.yMin; y < bounds.yMax; y++)
            for(var z = bounds.zMin; z < bounds.zMax; z++)
                chunk.SetBlock(new Vector3Int(x, y, z), Block.DIRT);
            
            var world = new World(1);
            world.SetChunk(new Vector2Int(0, 0), chunk);
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() => ChunkMesh.Generate(meshFilter, chunk, world), 2);
        }
        
        [Test]
        public void Performance_test_of_16x16_mesh_generation()
        {
            var gameObject = new GameObject();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            const int size = 16;
            var bounds = new BoundsInt(0, 0, 0, size, size, size);
            var chunk = new Chunk(bounds);
            for(var x = bounds.xMin; x < bounds.xMax; x++)
            for(var y = bounds.yMin; y < bounds.yMax; y++)
            for(var z = bounds.zMin; z < bounds.zMax; z++)
                chunk.SetBlock(new Vector3Int(x, y, z), Block.DIRT);
            
            var world = new World(1);
            world.SetChunk(new Vector2Int(0, 0), chunk);
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() => ChunkMesh.Generate(meshFilter, chunk, world), 2);
        }
        
        [Test]
        public void Performance_test_of_16x16x17_mesh_generation()
        {
            var gameObject = new GameObject();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            const int size = 16;
            var bounds = new BoundsInt(0, 0, 0, size, size, size + 1);
            var chunk = new Chunk(bounds);
            for(var x = bounds.xMin; x < bounds.xMax; x++)
            for(var y = bounds.yMin; y < bounds.yMax; y++)
            for(var z = bounds.zMin; z < bounds.zMax; z++)
                chunk.SetBlock(new Vector3Int(x, y, z), Block.DIRT);
            
            var world = new World(1);
            world.SetChunk(new Vector2Int(0, 0), chunk);
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() => ChunkMesh.Generate(meshFilter, chunk, world), 2);
        }
        
        [Test]
        public void Performance_test_of_16x64x16_mesh_generation()
        {
            var gameObject = new GameObject();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            const int size = 16;
            const int height = 64;
            var bounds = new BoundsInt(0, 0, 0, size, height, size);
            var chunk = new Chunk(bounds);
            for(var x = bounds.xMin; x < bounds.xMax; x++)
            for(var y = bounds.yMin; y < bounds.yMax; y++)
            for(var z = bounds.zMin; z < bounds.zMax; z++)
                chunk.SetBlock(new Vector3Int(x, y, z), Block.DIRT);
            
            var world = new World(1);
            world.SetChunk(new Vector2Int(0, 0), chunk);
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() => ChunkMesh.Generate(meshFilter, chunk, world), 2);
        }

        [Test]
        public void Floor_to_int()
        {
            var vector3 = Vector3.down;
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() => Vector3Int.FloorToInt(vector3), 9);
        }
        
        [Test]
        public void Vector3Int_min()
        {
            var first = Vector3Int.down;
            var second = Vector3Int.up;
            
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() => Vector3Int.Min(first, second), 9);
        }
        
        [Test]
        public void Dictionary_has_key()
        {
            var dict = new Dictionary<Vector3Int, BlockMesh>();
            for (var i = 0; i < 9000; i++)
            {
                dict[new Vector3Int(i, 0, 0)] = new BlockMesh();
            }

            var key = new Vector3Int(8000, 0, 0);
            
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() => dict.ContainsKey(key), 9);
        }
        
        [Test]
        public void Dictionary_get_value_and_update()
        {
            var dict = new Dictionary<Vector3Int, BlockMesh>();
            for (var i = 0; i < 9000; i++)
            {
                dict[new Vector3Int(i, 0, 0)] = new BlockMesh();
            }

            var key = new Vector3Int(8000, 0, 0);
            
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() => dict[key].AddTriangle(9999), 9);
        }

        [Test]
        public void Initialize_block_mesh()
        {
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() =>
            {
                var thing = new BlockMesh();
                thing.AddTriangle(1);
            }, 9);
        }

        [Test]
        public void Vector_multiplication()
        {
            var first = Vector3.down;
            var second = Vector3.up;
            var third = Vector3.left;
            ChunkMeshTestsSetup.PerformanceTesting.MeasureTenfoldIterations(() =>
            {
                var thing = (first + second + third) / 3;
            }, 9);
        }
    }
}
