using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibGit2Sharp.Tests.TestHelpers;
using NUnit.Framework;

namespace LibGit2Sharp.Tests
{
    [TestFixture]
    public class IndexFixture : BaseFixture
    {
        private readonly List<string> expectedEntries = new List<string>
                                                            {
                                                                "README",
                                                                "new.txt",
                                                                "branch_file.txt",
                                                                "1/branch_file.txt",
                                                                //"deleted_staged_file.txt",
                                                                "deleted_unstaged_file.txt",
                                                                "modified_staged_file.txt",
                                                                "modified_unstaged_file.txt",
                                                                "new_tracked_file.txt"
                                                            };

        [Test]
        public void CanCountEntriesInIndex()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                repo.Index.Count.ShouldEqual(expectedEntries.Count);
            }
        }

        [Test]
        public void CanEnumerateIndex()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                foreach (var entry in repo.Index)
                {
                    Assert.IsTrue(expectedEntries.Contains(entry.Path), string.Format("Could not find {0}", entry.Path));
                }
            }
        }

        [Test]
        [Ignore("Not implemented yet.")]
        public void CanEnumerateModifiedFiles()
        {
        }

        [Test]
        [Ignore("Not implemented yet.")]
        public void CanEnumerateUntrackedFiles()
        {
        }

        [Test]
        [Ignore("Not implemented yet.")]
        public void CanEnumeratorStagedFiles()
        {
        }

        [Test]
        public void CanFetchAnIndexEntryByItsName()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                var entry = repo.Index["README"];
                entry.Path.ShouldEqual("README");

                var entryWithPath = repo.Index["1/branch_file.txt"];
                entryWithPath.Path.ShouldEqual("1/branch_file.txt");
            }
        }

        [Test]
        public void FetchingAnUnknwonIndexEntryReturnsNull()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                var entry = repo.Index["I-do-not-exist.txt"];
                entry.ShouldBeNull();
            }
        }

        [Test]
        public void CanStageANewFile()
        {
            using (var path = new TemporaryCloneOfTestRepo(Constants.StandardTestRepoWorkingDirPath))
            using (var repo = new Repository(path.RepositoryPath))
            {
                var count = repo.Index.Count;
                const string filename = "unit_test.txt";
                repo.Index[filename].ShouldBeNull();
                File.WriteAllText(Path.Combine(repo.Info.WorkingDirectory, filename), "some contents");

                repo.Index.Stage(filename);

                repo.Index.Count.ShouldEqual(count + 1);
                repo.Index[filename].ShouldNotBeNull();
            }
        }

        [Test]
        public void StagingANewVersionOfAFileThenUnstagingRevertsTheBlobToTheVersionOfHead()
        {
            using (var scd = new SelfCleaningDirectory())
            {
                string dir = Repository.Init(scd.DirectoryPath);

                using (var repo = new Repository(dir))
                {
                    repo.Index.Count.ShouldEqual(0);

                    const string fileName = "myFile.txt";

                    var fullpath = Path.Combine(repo.Info.WorkingDirectory, fileName);

                    const string initialContent = "Hello?";
                    File.AppendAllText(fullpath, initialContent);

                    repo.Index.Stage(fileName);
                    var blobId = repo.Index[fileName].Id;

                    repo.Commit(Constants.Signature, Constants.Signature, "Initial commit");
                    repo.Index.Count.ShouldEqual(1);

                    File.AppendAllText(fullpath, "Is there there anybody out there?");
                    repo.Index.Stage(fileName);

                    repo.Index.Count.ShouldEqual(1);
                    repo.Index[fileName].Id.ShouldNotEqual((blobId));

                    repo.Index.Unstage(fileName);
                    repo.Index.Count.ShouldEqual(1);
                    repo.Index[fileName].Id.ShouldEqual((blobId));
                }
            }
        }

        [Test]
        public void CanStageANewFileInAPersistentManner()
        {
            using (var path = new TemporaryCloneOfTestRepo(Constants.StandardTestRepoPath))
            {
                using (var repo = new Repository(path.RepositoryPath))
                {
                    const string filename = "unit_test.txt";
                    repo.Index.RetrieveStatus(filename).ShouldEqual(GitStatus.GIT_STATUS_NOTFOUND);
                    repo.Index[filename].ShouldBeNull();

                    File.WriteAllText(Path.Combine(repo.Info.WorkingDirectory, filename), "some contents");
                    repo.Index.RetrieveStatus(filename).ShouldEqual(GitStatus.GIT_STATUS_WT_NEW);
                    repo.Index[filename].ShouldBeNull();

                    repo.Index.Stage(filename);
                    repo.Index[filename].ShouldNotBeNull();
                    repo.Index.RetrieveStatus(filename).ShouldEqual(GitStatus.GIT_STATUS_INDEX_NEW);
                    repo.Index[filename].State.ShouldEqual(GitStatus.GIT_STATUS_INDEX_NEW);
                }

                using (var repo = new Repository(path.RepositoryPath))
                {
                    const string filename = "unit_test.txt";
                    repo.Index[filename].ShouldNotBeNull();
                    repo.Index.RetrieveStatus(filename).ShouldEqual(GitStatus.GIT_STATUS_INDEX_NEW);
                    repo.Index[filename].State.ShouldEqual(GitStatus.GIT_STATUS_INDEX_NEW);
                }
            }
        }

        [Test]
        public void CanStageANewFileWithAFullPath()
        {
            using (var path = new TemporaryCloneOfTestRepo(Constants.StandardTestRepoWorkingDirPath))
            using (var repo = new Repository(path.RepositoryPath))
            {
                var count = repo.Index.Count;

                const string filename = "new_untracked_file.txt";
                string fullPath = Path.Combine(repo.Info.WorkingDirectory, filename);
                File.Exists(fullPath).ShouldBeTrue();

                repo.Index.Stage(fullPath);

                repo.Index.Count.ShouldEqual(count + 1);
                repo.Index[filename].ShouldNotBeNull();
            }
        }

        [Test]
        public void StagingANewFileWithAFullPathWhichEscapesOutOfTheWorkingDirThrows()
        {
            using (var scd = new SelfCleaningDirectory())
            using (var path = new TemporaryCloneOfTestRepo(Constants.StandardTestRepoWorkingDirPath))
            using (var repo = new Repository(path.RepositoryPath))
            {
                var di = Directory.CreateDirectory(scd.DirectoryPath);

                const string filename = "unit_test.txt";
                string fullPath = Path.Combine(di.FullName, filename);
                File.WriteAllText(fullPath, "some contents");

                Assert.Throws<ArgumentException>(() => repo.Index.Stage(fullPath));
            }
        }

        [Test]
        public void CanRenameAFile()
        {
            using (var scd = new SelfCleaningDirectory())
            {
                string dir = Repository.Init(scd.DirectoryPath);

                using (var repo = new Repository(dir))
                {
                    repo.Index.Count.ShouldEqual(0);

                    const string oldName = "polite.txt";
                    string oldPath = Path.Combine(repo.Info.WorkingDirectory, oldName);

                    //TODO: Retrieving status in an empty repository currently fails in libgit2
                    repo.Index.RetrieveStatus(oldName).ShouldEqual(GitStatus.GIT_STATUS_NOTFOUND);

                    File.WriteAllText(oldPath, "hello test file\n", Encoding.ASCII);
                    //TODO: Retrieving status in an empty repository currently fails in libgit2
                    repo.Index.RetrieveStatus(oldName).ShouldEqual(GitStatus.GIT_STATUS_WT_NEW);

                    repo.Index.Stage(oldName);
                    //TODO: Retrieving status in an empty repository currently fails in libgit2
                    repo.Index.RetrieveStatus(oldName).ShouldEqual(GitStatus.GIT_STATUS_INDEX_NEW);

                    // Generated through
                    // $ echo "hello test file" | git hash-object --stdin
                    const string expectedHash = "88df547706c30fa19f02f43cb2396e8129acfd9b";
                    repo.Index[oldName].Id.Sha.ShouldEqual((expectedHash));

                    repo.Index.Count.ShouldEqual(1);

                    Signature who = Constants.Signature;
                    repo.Commit(who, who, "Initial commit");

                    repo.Index.RetrieveStatus(oldName).ShouldEqual(GitStatus.GIT_STATUS_CURRENT);

                    const string newName = "being.frakking.polite.txt";

                    repo.Index.Move(oldName, newName);
                    repo.Index.RetrieveStatus(oldName).ShouldEqual(GitStatus.GIT_STATUS_INDEX_DELETED);
                    repo.Index.RetrieveStatus(newName).ShouldEqual(GitStatus.GIT_STATUS_INDEX_NEW);

                    repo.Index.Count.ShouldEqual(1);
                    repo.Index[newName].Id.Sha.ShouldEqual((expectedHash));

                    who = who.TimeShift(TimeSpan.FromMinutes(5));
                    Commit commit = repo.Commit(who, who, "Fix file name");

                    repo.Index.RetrieveStatus(oldName).ShouldEqual(GitStatus.GIT_STATUS_NOTFOUND);
                    repo.Index.RetrieveStatus(newName).ShouldEqual(GitStatus.GIT_STATUS_CURRENT);

                    commit.Tree[newName].Target.Id.Sha.ShouldEqual(expectedHash);
                }
            }
        }

        [Test]
        public void CanUnstageANewFile()
        {
            using (var path = new TemporaryCloneOfTestRepo(Constants.StandardTestRepoWorkingDirPath))
            using (var repo = new Repository(path.RepositoryPath))
            {
                var count = repo.Index.Count;

                const string filename = "new_untracked_file.txt";
                string fullPath = Path.Combine(repo.Info.WorkingDirectory, filename);
                File.Exists(fullPath).ShouldBeTrue();

                repo.Index.RetrieveStatus(filename).ShouldEqual(GitStatus.GIT_STATUS_WT_NEW);

                repo.Index.Stage(filename);
                repo.Index.Count.ShouldEqual(count + 1);

                repo.Index.RetrieveStatus(filename).ShouldEqual(GitStatus.GIT_STATUS_INDEX_NEW);

                repo.Index.Unstage(filename);
                repo.Index.Count.ShouldEqual(count);

                repo.Index.RetrieveStatus(filename).ShouldEqual(GitStatus.GIT_STATUS_WT_NEW);

            }
        }

        [Test]
        public void CanRetrieveTheStatusOfAFile()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                GitStatus status = repo.Index.RetrieveStatus("new_tracked_file.txt");
                status.ShouldEqual(GitStatus.GIT_STATUS_INDEX_NEW);
            }
        }

        [Test]
        public void CanRetrieveTheStatusOfTheWholeWorkingDirectory()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                RepositoryStatus status = repo.Index.RetrieveStatus();
                status.ShouldNotBeNull();
            }
        }

        [Test]
        public void UnstagingANonStagedFileThrows()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                Assert.Throws<ApplicationException>(() => repo.Index.Unstage("shadowcopy_of_an_unseen_ghost.txt"));
            }
        }

        [Test]
        public void ReadIndexWithBadParamsFails()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                Assert.Throws<ArgumentNullException>(() => { var entry = repo.Index[null]; });
                Assert.Throws<ArgumentException>(() => { var entry = repo.Index[string.Empty]; });
            }
        }

        [Test]
        public void StageFileWithBadParamsThrows()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                Assert.Throws<ArgumentException>(() => repo.Index.Stage(string.Empty));
                Assert.Throws<ArgumentNullException>(() => repo.Index.Stage(null));
            }
        }

        [Test]
        public void UnstagingFileWithBadParamsThrows()
        {
            using (var repo = new Repository(Constants.StandardTestRepoPath))
            {
                Assert.Throws<ArgumentException>(() => repo.Index.Stage(string.Empty));
                Assert.Throws<ArgumentNullException>(() => repo.Index.Stage(null));
            }
        }
    }
}