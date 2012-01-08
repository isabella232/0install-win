﻿/*
 * Copyright 2010-2012 Bastian Eicher
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using Common.Cli;
using Common.Utils;
using ZeroInstall.Model;
using ZeroInstall.Store.Feeds;

namespace ZeroInstall.Publish
{
    /// <summary>
    /// A wrapper around a <see cref="Feed"/> adding and XSL stylesheet and a digital signature.
    /// </summary>
    [Serializable]
    public class SignedFeed
    {
        #region Properties
        /// <summary>
        /// The wrapped <see cref="Feed"/>.
        /// </summary>
        public Feed Feed { get; private set; }

        /// <summary>
        /// The secret key used to sign the <see cref="Feed"/>; <see langword="null"/> for no signature.
        /// </summary>
        public OpenPgpSecretKey SecretKey { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new signed feed.
        /// </summary>
        /// <param name="feed">The wrapped <see cref="Feed"/>.</param>
        /// <param name="secretKey">The secret key used to sign the <see cref="Feed"/>; <see langword="null"/> for no signature.</param>
        public SignedFeed(Feed feed, OpenPgpSecretKey secretKey)
        {
            #region Sanity checks
            if (feed == null) throw new ArgumentNullException("feed");
            #endregion

            Feed = feed;
            SecretKey = secretKey;
        }
        #endregion

        //--------------------//

        #region Storage
        /// <summary>
        /// Loads a <see cref="Feed"/> from an XML file and identifies the signature (if any).
        /// </summary>
        /// <param name="path">The file to load from.</param>
        /// <returns>The loaded <see cref="SignedFeed"/>.</returns>
        /// <exception cref="IOException">Thrown if a problem occurs while reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if read access to the file is not permitted.</exception>
        /// <exception cref="InvalidDataException">Thrown if a problem occurs while deserializing the XML data.</exception>
        public static SignedFeed Load(string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            #endregion

            return new SignedFeed(Feed.Load(path), FeedUtils.GetKey(path, OpenPgpProvider.Default));
        }

        /// <summary>
        /// Saves <see cref="Feed"/> to an XML file, adds the default stylesheet and sign it it with <see cref="SecretKey"/> (if specified).
        /// </summary>
        /// <remarks>Writing and signing the feed file are performed as an atomic operation (i.e. if signing fails an existing file remains unchanged).</remarks>
        /// <param name="path">The file to save in.</param>
        /// <param name="passphrase">The passphrase to use to unlock the secret key; may be <see langword="null"/> if <see cref="SecretKey"/> is <see langword="null"/>.</param>
        /// <exception cref="IOException">Thrown if a problem occurs while writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if write access to the file is not permitted.</exception>
        /// <exception cref="WrongPassphraseException">Thrown if passphrase was incorrect.</exception>
        /// <exception cref="UnhandledErrorsException">Thrown if the OpenPGP implementation reported a problem.</exception>
        public void Save(string path, string passphrase)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            #endregion

            string tempPath = path + "." + Path.GetRandomFileName() + ".new";
            try
            {
                // Write to temporary file first
                Feed.Save(tempPath);

                FeedUtils.AddStylesheet(tempPath);
                if (SecretKey != null) FeedUtils.SignFeed(tempPath, SecretKey, passphrase, OpenPgpProvider.Default);

                FileUtils.Replace(tempPath, path);
            }
                #region Error handling
            catch (Exception)
            {
                // Clean up failed transactions
                if (File.Exists(tempPath)) File.Delete(tempPath);
                throw;
            }
            #endregion
        }
        #endregion
    }
}
