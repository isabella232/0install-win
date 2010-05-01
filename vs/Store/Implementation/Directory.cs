﻿using System;
using System.Globalization;
using ZeroInstall.Store.Properties;

namespace ZeroInstall.Store.Implementation
{
    /// <summary>
    /// An immutable directory-entry in a <see cref="Manifest"/>.
    /// </summary>
    public sealed class Directory : ManifestNode, IEquatable<Directory>
    {
        #region Properties
        /// <summary>
        /// The time this directory was last modified in the number of seconds since the epoch.
        /// </summary>
        /// <remarks>Outdated, retained for compatibility with old manifest format.</remarks>
        public long ModifiedTime { get; private set; }

        private string _fullPath;
        /// <summary>
        /// The name of the symlink without the containing directory.
        /// </summary>
        public string FullPath
        {
            get { return _fullPath; }
            private set
            {
                if (value.Contains("\n")) throw new ArgumentException(Resources.NewlineInName, "value");
                _fullPath = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new directory-entry.
        /// </summary>
        /// <param name="modifiedTime">The time this directory was last modified in the number of seconds since the epoch.</param>
        /// <param name="fullPath">The complete path of this directory relative to the tree root as a Unix-Path beginning with a slash.</param>
        internal Directory(long modifiedTime, string fullPath)
        {
            ModifiedTime = modifiedTime;
            FullPath = fullPath;
        }
        #endregion

        //--------------------//

        #region Conversion
        /// <summary>
        /// Returns the string representation of this node for the new manifest format.
        /// </summary>
        /// <returns><code>"D", space, full path name, newline</code></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "D {0}", FullPath);
        }

        /// <summary>
        /// Returns the string representation of this node for the old manifest format.
        /// </summary>
        /// <returns><code>"D", space, mtime, space, full path name, newline</code></returns>
        public override string ToStringOld()
        {
            return string.Format(CultureInfo.InvariantCulture, "D {0} {1}", ModifiedTime, FullPath);
        }

        /// <summary>
        /// Creates a new node from a string representation as created by <see cref="ToString"/>.
        /// </summary>
        /// <param name="line">The string representation to parse.</param>
        /// <returns>The newly created node.</returns>
        /// <exception cref="ArgumentException">Thrown if the number of space-separated parts in the <paramref name="line"/> are incorrect.</exception>
        internal static Directory FromString(string line)
        {
            const int numberOfParts = 2;
            string[] parts = line.Split(new[] { ' ' }, numberOfParts);
            if (parts.Length != numberOfParts) throw new ArgumentException(Resources.InvalidNumberOfLineParts, "line");
            return new Directory(0, parts[1]);
        }
        #endregion

        #region Equality
        public bool Equals(Directory other)
        {
            if (ReferenceEquals(null, other)) return false;

            // Directory ModifiedTime is ignored in the new manifest format
            return /*other.ModifiedTime == ModifiedTime &&*/ Equals(other.FullPath, FullPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(Directory) && Equals((Directory)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ModifiedTime.GetHashCode() * 397) ^ (FullPath != null ? FullPath.GetHashCode() : 0);
            }
        }
        #endregion
    }
}
