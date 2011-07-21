using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    public class RepositoryStatus : IEnumerable<StatusEntry>
    {
        private readonly List<StatusEntry> statusEntries = new List<StatusEntry>();
        private NativeMethods.status_callback callback;

        internal RepositoryStatus(RepositorySafeHandle handle)
        {
            callback = new NativeMethods.status_callback(StateChanged);
            Process(handle);
        }

        void Process(RepositorySafeHandle handle)
        {
            StringBuilder buffer = new StringBuilder(4096);
            int res = NativeMethods.git_status_foreach(handle, buffer, buffer.Capacity, callback, IntPtr.Zero);
            Ensure.Success(res);
        }

        private int StateChanged(string pathPtr, uint state, IntPtr payload)
        {
            statusEntries.Add(new StatusEntry(pathPtr, (GitStatus)state));
            return 0;
        }

        public IEnumerator<StatusEntry> GetEnumerator()
        {
            return statusEntries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}