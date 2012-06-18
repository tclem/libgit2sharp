using System;
using System.Runtime.InteropServices;

namespace LibGit2Sharp.Core
{
    internal class GitErrorMarshaler : ICustomMarshaler
    {
        bool shouldFree = true;

        public void CleanUpManagedData(object managedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            if(shouldFree)
            {
                Marshal.FreeHGlobal(pNativeData);
            }
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            throw new NotImplementedException();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return NativeToGitError(pNativeData);
        }

        protected GitError NativeToGitError(IntPtr pNativeData)
        {
            var res = (GitError)Marshal.PtrToStructure(pNativeData, typeof(GitError));

            if(res == null)
            {
                shouldFree = false;
            }

            return res;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new GitErrorMarshaler();
        }
    }
}
