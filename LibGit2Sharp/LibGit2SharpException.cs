﻿using System;
using System.Globalization;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   The exception that is thrown when an error occurs during application execution.
    /// </summary>
    [Obsolete("This type will be removed in the next release. Please use LibGit2SharpException instead.")]
    public class LibGit2Exception : LibGit2SharpException
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "LibGit2Exception" /> class.
        /// </summary>
        public LibGit2Exception()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LibGit2Exception" /> class with a specified error message.
        /// </summary>
        /// <param name = "message">A message that describes the error. </param>
        public LibGit2Exception(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LibGit2Exception" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name = "message">The error message that explains the reason for the exception. </param>
        /// <param name = "innerException">The exception that is the cause of the current exception. If the <paramref name = "innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public LibGit2Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    ///   The exception that is thrown when an error occurs during application execution.
    /// </summary>
    public class LibGit2SharpException : Exception
    {
        readonly GitErrorCode code;
        readonly GitErrorCategory category;
        readonly bool isLibraryError;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LibGit2SharpException" /> class.
        /// </summary>
        public LibGit2SharpException()
        {
        }

        internal LibGit2SharpException(GitErrorCode code, GitErrorCategory category, string message) : base(message)
        {
            this.code = code;
            this.category = category;

            Data.Add("libgit2.code", (int)code);
            Data.Add("libgit2.class", (int)category);
            isLibraryError = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LibGit2SharpException" /> class with a specified error message.
        /// </summary>
        /// <param name = "message">A message that describes the error. </param>
        public LibGit2SharpException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LibGit2SharpException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name = "message">The error message that explains the reason for the exception. </param>
        /// <param name = "innerException">The exception that is the cause of the current exception. If the <paramref name = "innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public LibGit2SharpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override string ToString()
        {
            return isLibraryError
                ? String.Format(CultureInfo.InvariantCulture, "An error was raised by libgit2. Class = {0} ({1}).{2}{3}",
                    category,
                    code,
                    Environment.NewLine,
                    Message)
                : base.ToString();
        }
    }
}
