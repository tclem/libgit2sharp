using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   A Commit
    /// </summary>
    public class Commit : GitObject
    {
        readonly Lazy<IEnumerable<Commit>> parents;
        readonly Repository repo;
        readonly Lazy<Tree> tree;

        string messageShort;

        internal Commit(ObjectId id, ObjectId treeId, Repository repo)
            : base(id)
        {
            tree = new Lazy<Tree>(() => repo.Lookup<Tree>(treeId));
            parents = new Lazy<IEnumerable<Commit>>(() => RetrieveParentsOfCommit(id.Oid));
            this.repo = repo;
        }

        #region Public Properties

        /// <summary>
        ///   Gets the author of this commit.
        /// </summary>
        public Signature Author { get; private set; }

        /// <summary>
        ///   Gets the committer.
        /// </summary>
        public Signature Committer { get; private set; }

        public string Encoding { get; private set; }

        /// <summary>
        ///   Gets the commit message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        ///   Gets the encoding of the message.
        /// </summary>
        public string MessageShort
        {
            get
            {
                if (messageShort == null)
                {
                    messageShort = string.IsNullOrEmpty(Message) ? string.Empty : Message.Split('\n')[0];
                }
                return messageShort;
            }
        }

        /// <summary>
        ///   Gets the parents of this commit. This property is lazy loaded and can throw an exception if the commit no longer exists in the repo.
        /// </summary>
        public IEnumerable<Commit> Parents
        {
            get { return parents.Value; }
        }

        /// <summary>
        ///   Gets the Tree associated to this commit.
        /// </summary>
        public Tree Tree
        {
            get { return tree.Value; }
        }

        #endregion

        internal static Commit BuildFromPtr(IntPtr obj, ObjectId id, Repository repo)
        {
            var treeId =
                new ObjectId((GitOid)Marshal.PtrToStructure(NativeMethods.git_commit_tree_oid(obj), typeof(GitOid)));

            return new Commit(id, treeId, repo)
            {
                Message = NativeMethods.git_commit_message(obj).MarshallAsString(),
                Encoding = RetrieveEncodingOf(obj),
                Author = new Signature(NativeMethods.git_commit_author(obj)),
                Committer = new Signature(NativeMethods.git_commit_committer(obj)),
            };
        }

        static string RetrieveEncodingOf(IntPtr obj)
        {
            string encoding = NativeMethods.git_commit_message_encoding(obj).MarshallAsString();

            return encoding ?? "UTF-8";
        }

        IEnumerable<Commit> RetrieveParentsOfCommit(GitOid oid) //TODO: Convert to a ParentEnumerator
        {
            IntPtr obj;
            int res = NativeMethods.git_object_lookup(out obj, repo.Handle, ref oid, GitObjectType.Commit);
            Ensure.Success(res);

            var parentsOfCommits = new List<Commit>();

            try
            {
                uint parentsCount = NativeMethods.git_commit_parentcount(obj);

                for (uint i = 0; i < parentsCount; i++)
                {
                    IntPtr parentCommit;
                    res = NativeMethods.git_commit_parent(out parentCommit, obj, i);
                    Ensure.Success(res);
                    parentsOfCommits.Add((Commit)CreateFromPtr(parentCommit, ObjectIdOf(parentCommit), repo));
                }
            }
            finally
            {
                NativeMethods.git_object_close(obj);
            }

            return parentsOfCommits;
        }
    }
}
