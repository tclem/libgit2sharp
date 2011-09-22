using System;
using System.Diagnostics;
using System.IO;

namespace LibGit2Sharp.Tests.TestHelpers
{
    public static class Constants
    {
        public const string BareTestRepoName = "testrepo.git.7z";
        public const string StandardTestRepoWorkingDirName = "testrepo_wd.7z";

        public static readonly string TestRepoPath = Path.Combine(GetTestRootDirectory(), "Resources");

        public const string UnknownSha = "deadbeefdeadbeefdeadbeefdeadbeefdeadbeef";
        public static readonly Signature Signature = new Signature("A. U. Thor", "thor@valhalla.asgard.com", new DateTimeOffset(2011, 06, 16, 10, 58, 27, TimeSpan.FromHours(2)));

        public static string GetTestRootDirectory()
        {
            // XXX: This is an evil hack, but it's okay for a unit test
            // We can't use Assembly.Location because unit test runners love
            // to move stuff to temp directories
            var st = new StackFrame(true);
            var di = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(st.GetFileName()), ".."));

            return di.FullName;
        }

    }
}
