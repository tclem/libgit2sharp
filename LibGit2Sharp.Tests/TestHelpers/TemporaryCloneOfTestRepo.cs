using System.Collections.Generic;
using System.IO;
using SevenZip;

namespace LibGit2Sharp.Tests.TestHelpers
{
    public class TemporaryCloneOfTestRepo : SelfCleaningDirectory
    {
        public TemporaryCloneOfTestRepo(string sourceZipFile = null)
        {
            sourceZipFile = sourceZipFile ?? Constants.BareTestRepoName;

            var extractor = new SevenZipExtractor(sourceZipFile);
            extractor.ExtractArchive(DirectoryPath);

            RepositoryPath = (IsGitWorkingDir() ? Path.Combine(DirectoryPath, ".git") : DirectoryPath);
        }

        public string RepositoryPath { get; private set; }

        bool IsGitWorkingDir()
        {
            var di = new DirectoryInfo(Path.Combine(DirectoryPath, ".git"));
            return di.Exists;
        }

        static Dictionary<string, TemporaryCloneOfTestRepo> readOnlyRepoCopies;
        public static TemporaryCloneOfTestRepo ReadOnlyRepo(string sourceZipFile = null)
        {
            var key = sourceZipFile ?? "__NULL__";
            if (readOnlyRepoCopies.ContainsKey(key))
            {
                return readOnlyRepoCopies[key];
            }

            return (readOnlyRepoCopies[key] = new TemporaryCloneOfTestRepo(sourceZipFile));
        }
    }

}