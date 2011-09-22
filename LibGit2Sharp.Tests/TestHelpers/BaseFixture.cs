using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using SevenZip;

namespace LibGit2Sharp.Tests.TestHelpers
{
    public class BaseFixture
    {
        static BaseFixture()
        {
            // Do the set up in the static ctor so it only happens once
            SetUpTestEnvironment();
        }
        
        [TearDown]
        public void DestroyReadOnlyRepos()
        {
            TemporaryCloneOfTestRepo.DisposeReadOnlyRepos();
        }

        static bool sevenZipIsExtracted;
        static void SetUpTestEnvironment()
        {
            if (sevenZipIsExtracted)
            {
                return;
            }

            Initialize7Zip();
            sevenZipIsExtracted = true;
        }

        static void Initialize7Zip()
        {
            string sevenZipPath = GetPathTo7ZipNative();
            if (sevenZipPath == null)
            {
                throw new Exception("Can't find or extract 7Zip");
            }

            SevenZipBase.SetLibraryPath(sevenZipPath);
        }

        static string GetPathTo7ZipNative()
        {
            // Is it already here? Just return it
            var fi = new FileInfo(@".\7z.dll");
            if (fi.Exists)
            {
                return fi.FullName;
            }

            // Let's try to extract it to the same folder as the test DLL - if 
            // it doesn't work, we will fall back to a temp folder
            var outPath = Path.Combine(GetTestDllDirectory(), "7z.dll");

            string ret = null;
            if ((ret = AssemblyExtensions.ExtractResourceToFile(null, "LibGit2Sharp.Tests.data.7z.dll", outPath)) != null)
            {
                return ret;
            }
            
            outPath = Path.GetTempFileName();
            return AssemblyExtensions.ExtractResourceToFile(null, "LibGit2Sharp.Tests.data.7z.dll", outPath);
        }

        static string GetTestDllDirectory()
        {
            var di = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            return di.FullName;
        }
    }
}
