using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LibGit2Sharp.Core
{
    internal static unsafe class Libgit2UnsafeHelper
    {
        public static IList<string> ListAllReferenceNames(RepositorySafeHandle repo, GitReferenceType types)
        {
            UnSafeNativeMethods.git_strarray strArray;
            var res = UnSafeNativeMethods.git_reference_listall(out strArray, repo, types);
            Ensure.Success(res);

            return BuildListOf(strArray);
        }

        public static IList<string> ListAllTagNames(RepositorySafeHandle repo)
        {
            UnSafeNativeMethods.git_strarray strArray;
            var res = UnSafeNativeMethods.git_tag_list(out strArray, repo);
            Ensure.Success(res);

            return BuildListOf(strArray);
        }

        private static IList<string> BuildListOf(UnSafeNativeMethods.git_strarray strArray)
        {
            var list = new List<string>();

            try
            {
                UnSafeNativeMethods.git_strarray* gitStrArray = &strArray;

                int numberOfEntries = gitStrArray->size.ToInt32();
                for (uint i = 0; i < numberOfEntries; i++)
                {
                    var name = new string(gitStrArray->strings[i]);
                    list.Add(name);
                }
            }
            finally
            {
                UnSafeNativeMethods.git_strarray_free(ref strArray);
            }

            return list;
        }
    }

    internal class UTF8Marshaler : ICustomMarshaler
    {
        static UTF8Marshaler static_instance;

        public unsafe IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj == null)
                return IntPtr.Zero;
            if (!(managedObj is string))
                throw new MarshalDirectiveException(
                       "UTF8Marshaler must be used on a string.");

            // not null terminated
            byte[] strbuf = Encoding.UTF8.GetBytes((string)managedObj);
            IntPtr buffer = Marshal.AllocHGlobal(strbuf.Length + 1);
            Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

            // write the terminating null
            byte* pBuf = (byte*) buffer;
            pBuf[strbuf.Length] = 0;

            return buffer;
        }

        public unsafe object MarshalNativeToManaged(IntPtr pNativeData)
        {
            byte* walk = (byte*)pNativeData;

            // find the end of the string
            while (*walk != 0)
            {
                walk++;
            }
            int length = (int)(walk - (byte*)pNativeData);

            // should not be null terminated
            byte[] strbuf = new byte[length - 1];
            // skip the trailing null
            Marshal.Copy((IntPtr)pNativeData, strbuf, 0, length - 1);
            string data = Encoding.UTF8.GetString(strbuf);
            return data;
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public void CleanUpManagedData(object managedObj)
        {
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (static_instance == null)
            {
                return static_instance = new UTF8Marshaler();
            }
            return static_instance;
        }
    }
}
