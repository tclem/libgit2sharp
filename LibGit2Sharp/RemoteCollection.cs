﻿using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    public class RemoteCollection
    {
        private readonly Repository repository;

        internal RemoteCollection(Repository repository)
        {
            this.repository = repository;
        }

        public Remote this[string name]
        {
            get { return RemoteForName(name); }
        }

        private Remote RemoteForName(string name)
        {
            var remote = new Remote();
            RemoteSafeHandle handle;
            Ensure.Success(NativeMethods.git_remote_get(out handle, repository.Config.LocalHandle, name));
            using (handle)
            {
                var ptr = NativeMethods.git_remote_name(handle);
                remote.Name = ptr.MarshallAsString();

                ptr = NativeMethods.git_remote_url(handle);
                remote.Url = ptr.MarshallAsString();
            }
            return remote;
        }
    }
}