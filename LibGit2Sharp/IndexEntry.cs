using System;
using System.Runtime.InteropServices;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    public class IndexEntry
    {
        Func<FileStatus> state;

        #region Public Properties

        public ObjectId Id { get; private set; }
        public string Path { get; private set; }

        public FileStatus State
        {
            get { return state(); }
        }

        #endregion

        internal static IndexEntry CreateFromPtr(Repository repo, IntPtr ptr)
        {
            var entry = (GitIndexEntry)Marshal.PtrToStructure(ptr, typeof(GitIndexEntry));
            return new IndexEntry
            {
                Path = entry.Path,
                Id = new ObjectId(entry.oid),
                state = () => repo.Index.RetrieveStatus(entry.Path)
            };
        }
    }
}
