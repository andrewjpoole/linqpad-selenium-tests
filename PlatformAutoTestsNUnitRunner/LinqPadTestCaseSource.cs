using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    public class LinqPadTestCaseSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                var currentDir = new DirectoryInfo(TestContext.CurrentContext.WorkDirectory);
                var platformAutoTestsDir = currentDir?.Parent?.Parent?.Parent?.Parent;
                var testDirectory = new DirectoryInfo(Path.Combine(platformAutoTestsDir.FullName, @"PlatformAutoTests\Tests\"));

                if (!testDirectory.Exists)
                    throw new DirectoryNotFoundException($"Cant find linqpad test directory, expected to find it @ {testDirectory.FullName}");

                var tests = GetTestsFromLinqpadFiles(testDirectory.FullName);
                foreach (var test in tests)
                    yield return test;
            }
        }

        public static List<TestCaseData> GetTestsFromLinqpadFiles(string directoryPath)
        {
            var testCases = new List<TestCaseData>();
            var dir = new DirectoryInfo(directoryPath);
            var subDirs = dir.EnumerateDirectories();
            foreach (var subDir in subDirs)
            {
                var linqpadFiles = subDir.EnumerateFiles("*.linq");
                foreach (var linqpadFile in linqpadFiles)
                {
                    var contents = File.ReadAllText(linqpadFile.FullName);
                    if (contents.Contains("//[NUNIT IGNORE]"))
                        continue;

                    TestContext.Out.WriteLine($"found test {linqpadFile.FullName}");

                    var testCase = new TestCaseData(linqpadFile.FullName)
                        .SetName($"{subDir.Name} - {linqpadFile.Name.Replace(".linq", "")}")
                        .SetCategory(subDir.Name);
                    testCases.Add(testCase);
                }
            }
            return testCases;
        }
    }
}