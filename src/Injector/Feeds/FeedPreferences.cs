﻿/*
 * Copyright 2010-2011 Bastian Eicher
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;
using Common;
using Common.Collections;
using Common.Storage;
using Common.Utils;
using ZeroInstall.Model;

namespace ZeroInstall.Injector.Feeds
{
    /// <summary>
    /// Stores user-specific preferences for a <see cref="Model.Feed"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "C5 collections don't need to be disposed.")]
    [XmlRoot("feed-preferences", Namespace = Feed.XmlNamespace)]
    [XmlType("feed-preferences", Namespace = Feed.XmlNamespace)]
    public sealed class FeedPreferences : XmlUnknown, ICloneable
    {
        #region Properties
        /// <summary>
        /// The point in time this feed was last checked for updates.
        /// </summary>
        [Description("The point in time this feed was last checked for updates.")]
        [XmlIgnore]
        public DateTime LastChecked { get; set; }

        /// <summary>Used for XML serialization.</summary>
        /// <seealso cref="LastChecked"/>
        [XmlAttribute("last-checked"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long LastCheckedUnix
        {
            get { return FileUtils.ToUnixTime(LastChecked); }
            set { LastChecked = FileUtils.FromUnixTime(value); }
        }

        // Preserve order
        private readonly C5.ArrayList<ImplementationPreferences> _implementations = new C5.ArrayList<ImplementationPreferences>();
        /// <summary>
        /// A list of implementation-specific user-overrides.
        /// </summary>
        [Description("A list of implementation-specific user-overrides.")]
        [XmlElement("implementation")]
        // Note: Can not use ICollection<T> interface with XML Serialization
        public C5.ArrayList<ImplementationPreferences> Implementations { get { return _implementations; } }
        #endregion

        //--------------------//

        #region Storage
        /// <summary>
        /// Loads <see cref="FeedPreferences"/> from an XML file.
        /// </summary>
        /// <param name="path">The file to load from.</param>
        /// <returns>The loaded <see cref="FeedPreferences"/>.</returns>
        /// <exception cref="IOException">Thrown if a problem occurs while reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if read access to the file is not permitted.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a problem occurs while deserializing the XML data.</exception>
        public static FeedPreferences Load(string path)
        {
            return XmlStorage.Load<FeedPreferences>(path);
        }

        /// <summary>
        /// Loads <see cref="FeedPreferences"/> for a specific feed. Automatically falls back to defaults on errors.
        /// </summary>
        /// <param name="feedID">The feed to load the preferences for.</param>
        /// <returns>The loaded <see cref="FeedPreferences"/>.</returns>
        public static FeedPreferences LoadFor(string feedID)
        {
            var path = EnumerableUtils.GetFirst(Locations.GetLoadConfigPaths("0install.net", FileUtils.PathCombine("injector", "feeds", ModelUtils.PrettyEscape(feedID)), false));
            if (string.IsNullOrEmpty(path)) return new FeedPreferences();

            try { return XmlStorage.Load<FeedPreferences>(path); }
            #region Error handling
            catch (Exception ex)
            {
                Log.Warn("Failed to load preferences for feed '" + feedID + "'\n" + ex.Message);
                Log.Warn("Reverting to default values");
                return new FeedPreferences();
            }
            #endregion
        }

        /// <summary>
        /// Saves these <see cref="FeedPreferences"/> to an XML file.
        /// </summary>
        /// <param name="path">The file to save in.</param>
        /// <exception cref="IOException">Thrown if a problem occurs while writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if write access to the file is not permitted.</exception>
        public void Save(string path)
        {
            XmlStorage.Save(path, this);
        }

        /// <summary>
        /// Saves these <see cref="FeedPreferences"/> for a specific feed.
        /// </summary>
        /// <param name="feedID">The feed to save the preferences for.</param>
        /// <exception cref="IOException">Thrown if a problem occurs while writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if write access to the file is not permitted.</exception>
        public void SaveFor(string feedID)
        {
            var path = Locations.GetSaveConfigPath("0install.net", FileUtils.PathCombine("injector", "feeds", ModelUtils.PrettyEscape(feedID)), false);
            XmlStorage.Save(path, this);
        }
        #endregion

        //--------------------//

        #region Clone
        /// <summary>
        /// Creates a deep copy of this <see cref="FeedPreferences"/> instance.
        /// </summary>
        /// <returns>The new copy of the <see cref="FeedPreferences"/>.</returns>
        public FeedPreferences CloneFeedPreferences()
        {
            var feedPreferences = new FeedPreferences {LastChecked = LastChecked};
            foreach (var implementation in Implementations) feedPreferences.Implementations.Add(implementation.CloneImplementationPreferences());

            return feedPreferences;
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="FeedPreferences"/> instance.
        /// </summary>
        /// <returns>The new copy of the <see cref="FeedPreferences"/>.</returns>
        public object Clone()
        {
            return CloneFeedPreferences();
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Returns the preferences in the form "FeedPreferences: LastChecked". Not safe for parsing!
        /// </summary>
        public override string ToString()
        {
            return string.Format("FeedPreferences: {0}", LastChecked);
        }
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(FeedPreferences other)
        {
            if (other == null) return false;

            if (LastChecked != other.LastChecked) return false;
            if (!Implementations.SequencedEquals(other.Implementations)) return false;
            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(FeedPreferences) && Equals((FeedPreferences)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = LastChecked.GetHashCode();
                result = (result * 397) ^ Implementations.GetUnsequencedHashCode();
                return result;
            }
        }
        #endregion
    }
}
