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
using System.Text.RegularExpressions;
using Common.Storage;
using Common.Streams;
using Common.Tasks;
using NUnit.Framework;
using Moq;
using ZeroInstall.Injector;
using ZeroInstall.Model;

namespace ZeroInstall.Store.Implementation
{
    /// <summary>
    /// Contains test methods for <see cref="Manifest"/>.
    /// </summary>
    [TestFixture]
    public class ManifestTest
    {
        #region Helpers
        /// <summary>
        /// Creates a <see cref="Manifest"/> from a temporary directory.
        /// </summary>
        private static Manifest CreateTestManifest()
        {
            // Create a test directory to create a manifest for
            string tempDir = DirectoryStoreTest.CreateArtificialPackage();

            try
            {
                // Generate manifest, write it to a file and read the file again
                return Manifest.Generate(tempDir, ManifestFormat.Sha1New, new SilentHandler(), null);
            }
            finally
            { // Clean up
                Directory.Delete(tempDir, true);
            }
        }
        #endregion

        [Test(Description = "Ensures that Manifest is correctly generated, serialized and deserialized.")]
        public void TestSaveLoad()
        {
            Manifest manifest1, manifest2;
            using (var tempFile = new TemporaryFile("0install-unit-tests"))
            {
                // Generate manifest, write it to a file and read the file again
                manifest1 = CreateTestManifest();
                manifest1.Save(tempFile.Path);
                manifest2 = Manifest.Load(tempFile.Path, ManifestFormat.Sha1New);
            }

            // Ensure data stayed the same
            Assert.AreEqual(manifest1, manifest2);
        }

        [Test(Description = "Ensures damaged manifest lines are correctly identified.")]
        public void TestLoadException()
        {
            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("test"), ManifestFormat.Sha1));
            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("test"), ManifestFormat.Sha1New));
            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("test"), ManifestFormat.Sha256));
            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("test"), ManifestFormat.Sha256New));

            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("D /test"), ManifestFormat.Sha1));
            Assert.DoesNotThrow(() => Manifest.Load(StreamUtils.CreateFromString("D /test"), ManifestFormat.Sha1New));
            Assert.DoesNotThrow(() => Manifest.Load(StreamUtils.CreateFromString("D /test"), ManifestFormat.Sha256));
            Assert.DoesNotThrow(() => Manifest.Load(StreamUtils.CreateFromString("D /test"), ManifestFormat.Sha256New));

            Assert.DoesNotThrow(() => Manifest.Load(StreamUtils.CreateFromString("F abc123 1200000000 128 test"), ManifestFormat.Sha1));
            Assert.DoesNotThrow(() => Manifest.Load(StreamUtils.CreateFromString("F abc123 1200000000 128 test"), ManifestFormat.Sha1New));
            Assert.DoesNotThrow(() => Manifest.Load(StreamUtils.CreateFromString("F abc123 1200000000 128 test"), ManifestFormat.Sha256));
            Assert.DoesNotThrow(() => Manifest.Load(StreamUtils.CreateFromString("F abc123 1200000000 128 test"), ManifestFormat.Sha256New));

            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("F abc123 128 test"), ManifestFormat.Sha1));
            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("F abc123 128 test"), ManifestFormat.Sha1New));
            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("F abc123 128 test"), ManifestFormat.Sha256));
            Assert.Throws<FormatException>(() => Manifest.Load(StreamUtils.CreateFromString("F abc123 128 test"), ManifestFormat.Sha256New));
        }

        [Test]
        public void TestCalculateDigest()
        {
            string packageDir = DirectoryStoreTest.CreateArtificialPackage();
            try
            {
                Assert.AreEqual(
                    Manifest.CreateDotFile(packageDir, ManifestFormat.Sha1, new SilentHandler()),
                    Manifest.Generate(packageDir, ManifestFormat.Sha1, new SilentHandler(), null).CalculateDigest(),
                    "sha1 dot file and digest should match");
                Assert.AreEqual(
                    Manifest.CreateDotFile(packageDir, ManifestFormat.Sha1New, new SilentHandler()),
                    Manifest.Generate(packageDir, ManifestFormat.Sha1New, new SilentHandler(), null).CalculateDigest(),
                    "sha1new dot file and digest should match");
                Assert.AreEqual(
                    Manifest.CreateDotFile(packageDir, ManifestFormat.Sha256, new SilentHandler()),
                    Manifest.Generate(packageDir, ManifestFormat.Sha256, new SilentHandler(), null).CalculateDigest(),
                    "sha256 dot file and digest should match");
                Assert.AreEqual(
                    Manifest.CreateDotFile(packageDir, ManifestFormat.Sha256New, new SilentHandler()),
                    Manifest.Generate(packageDir, ManifestFormat.Sha256New, new SilentHandler(), null).CalculateDigest(),
                    "sha256new dot file and digest should match");
            }
            finally
            {
                Directory.Delete(packageDir, true);
            }
        }

        [Test]
        public void TestCreateDigest()
        {
            string packageDir = DirectoryStoreTest.CreateArtificialPackage();
            try
            {
                ManifestDigest digest1 = Manifest.CreateDigest(packageDir, new SilentHandler());
                Assert.IsNullOrEmpty(digest1.Sha1); // sha1 is deprecated
                Assert.IsNotNullOrEmpty(digest1.Sha1New);
                Assert.IsNotNullOrEmpty(digest1.Sha256);
                Assert.IsNotNullOrEmpty(digest1.Sha256New);

                ManifestDigest digest2 = Manifest.CreateDigest(packageDir, new SilentHandler());
                Assert.AreEqual(digest1, digest2);
            }
            finally
            {
                Directory.Delete(packageDir, true);
            }
        }

        [Test(Description = "Ensures that ToString() correctly outputs a serialized form of the manifest.")]
        public void TestToString()
        {
            string packageDir = DirectoryStoreTest.CreateArtificialPackage();
            try
            {
                var manifest = Manifest.Generate(packageDir, ManifestFormat.Sha1New, new SilentHandler(), null);
                Assert.AreEqual("D /subdir\nF 606ec6e9bd8a8ff2ad14e5fade3f264471e82251 946684800 3 file.txt\n", manifest.ToString().Replace(Environment.NewLine, "\n"));
            }
            finally
            {
                Directory.Delete(packageDir, true);
            }
        }

        // ReSharper disable AssignNullToNotNullAttribute
        [Test]
        public void ShouldListNormalWindowsExeWithFlagF()
        {
            using (var package = new TemporaryDirectory("0install-unit-tests"))
            {
                string exePath = Path.Combine(package.Path, "test.exe");
                string manifestPath = Path.Combine(package.Path, ".manifest");

                File.WriteAllText(exePath, "");
                Manifest.CreateDotFile(package.Path, ManifestFormat.Sha256, new SilentHandler());

                using (var manifest = File.OpenText(manifestPath))
                {
                    string firstLine = manifest.ReadLine();
                    Assert.True(Regex.IsMatch(firstLine, @"^F \w+ \d+ \d+ test.exe$"), "Manifest didn't match expected format");
                }
            }
        }

        [Test]
        public void ShouldListFilesInXbitWithFlagX()
        {
            using (var package = new TemporaryDirectory("0install-unit-tests"))
            {
                string exePath = Path.Combine(package.Path, "test.exe");
                string xbitPath = Path.Combine(package.Path, ".xbit");
                string manifestPath = Path.Combine(package.Path, ".manifest");

                File.WriteAllText(exePath, "");
                File.WriteAllText(xbitPath, @"/test.exe");
                Manifest.CreateDotFile(package.Path, ManifestFormat.Sha256, new SilentHandler());

                using (var manifest = File.OpenText(manifestPath))
                {
                    string firstLine = manifest.ReadLine();
                    Assert.True(Regex.IsMatch(firstLine, @"^X \w+ \d+ \d+ test.exe$"), "Manifest didn't match expected format");
                }
            }
        }

        [Test]
        public void ShouldListFilesInSymlinkWithFlagS()
        {
            using (var package = new TemporaryDirectory("0install-unit-tests"))
            {
                string exePath = Path.Combine(package.Path, "test");
                string xbitPath = Path.Combine(package.Path, ".symlink");
                string manifestPath = Path.Combine(package.Path, ".manifest");

                File.WriteAllText(exePath, "");
                File.WriteAllText(xbitPath, @"/test");
                Manifest.CreateDotFile(package.Path, ManifestFormat.Sha256, new SilentHandler());

                using (var manifest = File.OpenText(manifestPath))
                {
                    string firstLine = manifest.ReadLine();
                    Assert.True(Regex.IsMatch(firstLine, @"^S \w+ \d+ test$"), "Manifest didn't match expected format");
                }
            }
        }

        [Test]
        public void ShouldListNothingForEmptyPackage()
        {
            using (var package = new TemporaryDirectory("0install-unit-tests"))
            {
                Manifest.CreateDotFile(package.Path, ManifestFormat.Sha256, new SilentHandler());
                using (var manifestFile = File.OpenRead(Path.Combine(package.Path, ".manifest")))
                    Assert.AreEqual(0, manifestFile.Length, "Empty package directory should make an empty manifest");
            }
        }

        [Test]
        public void ShouldHandleSubdirectoriesWithExecutables()
        {
            using (var package = new TemporaryDirectory("0install-unit-tests"))
            {
                string innerPath = Path.Combine(package.Path, "inner");
                Directory.CreateDirectory(innerPath);

                string innerExePath = Path.Combine(innerPath, "inner.exe");
                string xbitPath = Path.Combine(package.Path, ".xbit");
                string manifestPath = Path.Combine(package.Path, ".manifest");
                File.WriteAllText(innerExePath, @"xxxxxxx");
                File.WriteAllText(xbitPath, @"/inner/inner.exe");
                Manifest.CreateDotFile(package.Path, ManifestFormat.Sha256, new SilentHandler());
                using (var manifestFile = File.OpenText(manifestPath))
                {
                    string currentLine = manifestFile.ReadLine();
                    Assert.True(Regex.IsMatch(currentLine, @"^D /inner$"), "Manifest didn't match expected format:\n" + currentLine);
                    currentLine = manifestFile.ReadLine();
                    Assert.True(Regex.IsMatch(currentLine, @"^X \w+ \w+ \d+ inner.exe$"), "Manifest didn't match expected format:\n" + currentLine);
                }
            }
        }

        [Test]
        public void ShouldHandleSha1()
        {
            using (var package = new TemporaryDirectory("0install-unit-tests"))
            {
                string innerPath = Path.Combine(package.Path, "inner");
                Directory.CreateDirectory(innerPath);

                string innerExePath = Path.Combine(innerPath, "inner.exe");
                string xbitPath = Path.Combine(package.Path, ".xbit");
                string manifestPath = Path.Combine(package.Path, ".manifest");
                File.WriteAllText(innerExePath, @"xxxxxxx");
                File.WriteAllText(xbitPath, @"/inner/inner.exe");
                Manifest.CreateDotFile(package.Path, ManifestFormat.Sha1, new SilentHandler());
                using (var manifestFile = File.OpenText(manifestPath))
                {
                    string currentLine = manifestFile.ReadLine();
                    Assert.True(Regex.IsMatch(currentLine, @"^D \w+ /inner$"), "Manifest didn't match expected format:\n" + currentLine);
                    currentLine = manifestFile.ReadLine();
                    Assert.True(Regex.IsMatch(currentLine, @"^X \w+ \w+ \d+ inner.exe$"), "Manifest didn't match expected format:\n" + currentLine);
                }
            }
        }

        // ReSharper restore AssignNullToNotNullAttribute

        [Test]
        public void ShouldCallProgressCallback()
        {
            string packageDir = DirectoryStoreTest.CreateArtificialPackage();
            try
            {
                var handlerMock = new Mock<ITaskHandler>(MockBehavior.Strict);
                handlerMock.Setup(x => x.RunTask(It.IsAny<ITask>(), It.IsAny<string>())).Verifiable();
                Manifest.Generate(packageDir, ManifestFormat.Sha256, handlerMock.Object, null);
                handlerMock.Verify();
            }
            finally
            {
                Directory.Delete(packageDir, true);
            }
        }
    }
}
