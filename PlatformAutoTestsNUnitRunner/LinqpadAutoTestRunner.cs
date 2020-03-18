using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Tests
{

    [TestFixture]
    public class LinqpadAutoTestRunner
    {
        [Test, TestCaseSource(typeof(LinqPadTestCaseSource), "TestCases")]
        public void LinqPadTest(string filePath)
        {
            var sbOutput = new StringBuilder();
            var pathToLinqPad = @"C:\Program Files\LINQPad6\LPRun6.exe";

            if (!File.Exists(pathToLinqPad))
                throw new Exception($"Expected to find Linqpad6 installed @ {pathToLinqPad}");

            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = pathToLinqPad,
                Arguments = $@"""{filePath}""",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                var line = proc.StandardOutput.ReadLine();
                sbOutput.AppendLine(line);
                TestContext.Out.WriteLine(line);
            }

            proc.WaitForExit();
            
            var output = sbOutput.ToString();

            if (string.IsNullOrEmpty(output))
                Assert.Fail($"Problem running linqpad - no output captured, Exit code was {proc.ExitCode}");

            if (output.Contains("==Test FAILED=="))
                Assert.Fail("Output from lprun indicated that the Test failed");

        }
    }
}