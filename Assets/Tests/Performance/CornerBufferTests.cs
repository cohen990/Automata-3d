using NUnit.Framework;
using Terrain.Mesh;
using UnityEngine;

namespace Tests.Performance
{
    [SetUpFixture]
    public class CornerBufferTestsSetup
    {
        public static PerformanceTests PerformanceTesting;
        
        [OneTimeSetUp]
        public void SetUp()
        {
            PerformanceTesting = new PerformanceTests(System.DateTime.Now.ToString("o"));
        }
    }
    
    [TestFixture]
    public class CornerBufferTests
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        [TestCase(10000000)]
        public void Corner_buffer_clearing(int bufferSize)
        {
            var buffer = new CornerBuffer();
            for (var i = 0; i < bufferSize; i++)
                buffer.Add(new Corner(Vector3.zero, Vector3.back, Vector2.zero, Vector2.zero));
            
            CornerBufferTestsSetup.PerformanceTesting.Measure(() => buffer.Clear());
        }
        
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        [TestCase(10000000)]
        public void Corner_buffer_decomposing(int bufferSize)
        {
            var buffer = new CornerBuffer();
            for (var i = 0; i < bufferSize; i++)
                buffer.Add(new Corner(Vector3.zero, Vector3.back, Vector2.zero, Vector2.zero));
            
            CornerBufferTestsSetup.PerformanceTesting.Measure(() => buffer.Decompose());
        }
    }
}
