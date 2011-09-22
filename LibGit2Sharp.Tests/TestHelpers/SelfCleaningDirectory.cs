using System;
using System.IO;

namespace LibGit2Sharp.Tests.TestHelpers
{
    public class SelfCleaningDirectory : IDisposable
    {
        public SelfCleaningDirectory() : this(BuildTempPath())
        {
        }

        public SelfCleaningDirectory(string path)
        {
            DirectoryPath = Path.GetFullPath(path);
        }

        public string DirectoryPath { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                throw new InvalidOperationException(String.Format("Directory '{0}' doesn't exist any longer.", DirectoryPath));
            }

            DirectoryHelper.DeleteDirectory(DirectoryPath);
        }

        #endregion

        protected static string BuildTempPath()
        {
            var di = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "LibGit2Sharp-" + Guid.NewGuid()));
            if (!di.Exists)
            {
                di.Create();
            }

            return di.FullName;
        }
    }
}
