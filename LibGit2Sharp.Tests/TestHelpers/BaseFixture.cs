using System.IO;

namespace LibGit2Sharp.Tests.TestHelpers
{
    public class BaseFixture
    {
        static BaseFixture()
        {
            // Do the set up in the static ctor so it only happens once
            SetUpTestEnvironment();
        }

        private static void SetUpTestEnvironment()
        {
            var source = new DirectoryInfo(Path.Combine(Constants.GetTestRootDirectory(), "Resources"));
            var target = new DirectoryInfo(@".\Resources");

            if (target.Exists)
            {
                target.Delete(true);
            }

            DirectoryHelper.CopyFilesRecursively(source, target);

            // The test repo under source control has its .git folder renamed to dot_git to avoid confusing git,
            // so we need to rename it back to .git in our copy under the target folder

            string tempDotGit = Path.Combine(Constants.StandardTestRepoWorkingDirPath, "dot_git");
            Directory.Move(tempDotGit, Constants.StandardTestRepoPath);

            // Hack! Those test files are part of the repository. When checked out on Windows with core.autocrlf config set to true, 
            // LF are replace with CRLF. As git_status_xxx() doesn't handle LF/CRLF yet, we regenerate those files with a LF line ending character.
            File.WriteAllText(Path.Combine(Constants.StandardTestRepoWorkingDirPath, "1/branch_file.txt"), "hi\n");
            File.WriteAllText(Path.Combine(Constants.StandardTestRepoWorkingDirPath, "branch_file.txt"), "hi\n");
            File.WriteAllText(Path.Combine(Constants.StandardTestRepoWorkingDirPath, "new.txt"), "my new file\n");
            File.WriteAllText(Path.Combine(Constants.StandardTestRepoWorkingDirPath, "new_tracked_file.txt"), "a new file\n");
            File.WriteAllText(Path.Combine(Constants.StandardTestRepoWorkingDirPath, "new_tracked_file.txt"), "a new file\n");
            File.WriteAllText(Path.Combine(Constants.StandardTestRepoWorkingDirPath, "modified_staged_file.txt"), "a change\nmore files!\n");
            File.WriteAllText(Path.Combine(Constants.StandardTestRepoWorkingDirPath, "README"), "hey there\n");
        }

        protected void CreateCorruptedDeadBeefHead(string repoPath)
        {
            const string deadbeef = "deadbeef";
            string headPath = string.Format("{0}refs/heads/{1}", repoPath, deadbeef);
            File.WriteAllText(headPath, string.Format("{0}{0}{0}{0}{0}\n", deadbeef));
        }
    }
}
