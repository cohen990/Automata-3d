using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Tests
{
    public class PerformanceTests
    {
        private readonly string _fileName;

        public PerformanceTests(string fileName)
        {
            _fileName = fileName;
        }
        
        public void Measure(Action action, int iterations = 1)
        {
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                action.Invoke();
            }
            stopwatch.Stop();

            var testResult = $"[{TestContext.CurrentContext.Test.Name}] {iterations:N0} iteration(s) completed in {stopwatch.Elapsed}\n";
            TestContext.WriteLine(testResult);
            File.AppendAllText($"./PerformanceTestResults/{_fileName}.txt", testResult);
        }

        public void MeasureTenfoldIterations(Action func, int timesToTenfold)
        {
            for (var i = 0; i < timesToTenfold; i++)
            {
                Measure(func, (int)Math.Pow(10, i));
            }
        }
    }
}
