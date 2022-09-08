using NUnit.Framework;
using Terrain;
using Terrain.Mesh;
using UnityEngine;

namespace Tests.Behaviour
{
    [TestFixture]
    public class When_placing_a_stone_block
    {
        [Test]
        public void Should_not_have_any_dirt_uvs()
        {
            var chunkBounds = new BoundsInt(0, 0, 0, 1, 2, 1);
            var chunk = new Chunk(chunkBounds);
            chunk.SetBlock(new Vector3Int(0, 0, 0), Block.BEDROCK);
            chunk.SetBlock(new Vector3Int(1, 0, 0), Block.BEDROCK);
            chunk.SetBlock(new Vector3Int(0, 0, 1), Block.BEDROCK);
            chunk.SetBlock(new Vector3Int(1, 0, 1), Block.BEDROCK);
            var world = new World(chunkBounds.xMax);
            world.SetChunk(new Vector2Int(0, 0), chunk);

            var gameObject = new GameObject();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            
            var coroutine = ChunkMesh.Generate(meshFilter, chunk, world);
            while (coroutine.MoveNext())
            {
            }

            var chunkMesh = ChunkMesh.Latest;

            var blockPosition = new Vector3Int(0, 1, 0);
            chunk.SetBlock(blockPosition, Block.STONE);
            chunkMesh.UpdateBlock(blockPosition);
            var blockMesh = chunkMesh.BlockMeshes[blockPosition];
            var otherBlockMesh = chunkMesh.BlockMeshes[blockPosition - new Vector3Int(0, 1, 0)];

            var mesh = meshFilter.sharedMesh;
            var uv2S = mesh.uv2;
            var triangles = mesh.triangles;
            for (var i = 0; i < 36; i++)
            {
                Assert.That(uv2S[triangles[otherBlockMesh.Triangle(i)]], Is.EqualTo(TextureLocations.Bedrock));
            }
            for (var i = 0; i < 36; i++)
            {
                Assert.That(uv2S[triangles[blockMesh.Triangle(i)]], Is.EqualTo(TextureLocations.Stone));
            }
        }
    }
}
