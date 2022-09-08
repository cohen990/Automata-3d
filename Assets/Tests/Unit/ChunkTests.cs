using NUnit.Framework;
using Terrain;
using UnityEngine;

namespace Tests.Unit
{
    [TestFixture]
    public class ChunkTests
    {
        [Test]
        public void Should_initialise_a_chunk_with_all_empty_blocks()
        {
            var bounds = new BoundsInt(0, 0, 0, 2, 2, 2);
            var chunk = new Chunk(bounds);

            foreach(var block in chunk)
            {
                Assert.That(block, Is.EqualTo(Block.NULL));
            }
        }
    }
}
