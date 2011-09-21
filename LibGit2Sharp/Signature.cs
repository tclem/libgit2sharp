using System;
using System.Runtime.InteropServices;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   A signature
    /// </summary>
    public class Signature
    {
        readonly GitSignature handle = new GitSignature();
        DateTimeOffset? when;

        internal Signature(IntPtr signaturePtr)
        {
            Marshal.PtrToStructure(signaturePtr, handle);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Signature" /> class.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <param name = "email">The email.</param>
        /// <param name = "when">The when.</param>
        public Signature(string name, string email, DateTimeOffset when)
        {
            IntPtr ptr;
            Ensure.Success(NativeMethods.git_signature_new(out ptr, name, email, when.ToSecondsSinceEpoch(), (int)when.Offset.TotalMinutes));
            Marshal.PtrToStructure(ptr, handle);
            NativeMethods.git_signature_free(ptr);
        }

        #region Public Properties

        /// <summary>
        ///   Gets the email.
        /// </summary>
        public string Email
        {
            get { return handle.Email; }
        }

        internal GitSignature Handle
        {
            get { return handle; }
        }

        /// <summary>
        ///   Gets the name.
        /// </summary>
        public string Name
        {
            get { return handle.Name; }
        }

        /// <summary>
        ///   Gets the date when this signature happened.
        /// </summary>
        public DateTimeOffset When
        {
            get
            {
                if (when == null)
                {
                    when = Epoch.ToDateTimeOffset(handle.When.Time, handle.When.Offset);
                }
                return when.Value;
            }
        }

        #endregion
    }
}
