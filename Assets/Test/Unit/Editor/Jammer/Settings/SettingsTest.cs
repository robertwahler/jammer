using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Jammer.Test {

  [TestFixture]
  [Category("Settings")]
  public class SettingsTest : UnitTest {

    [SetUp]
    public void Before() {
    }

    [TearDown]
    public void After() {
      Settings.Clear();
    }

    [Test]
    public void GetStringReturnsEmptyStringWhenNoDataFileExists() {
      string key = "test";

      Assert.IsFalse(Settings.ContainsKey(key));
      Assert.IsTrue(Settings.GetString(key) == "");
    }

    [Test]
    public void GetStringReturnsDefaultWhenNoDataFileExists() {
      string key = "test";

      Assert.IsFalse(Settings.ContainsKey(key));
      Assert.IsTrue(Settings.GetString(key, "default") == "default");
    }

    [Test]
    public void GetStringReturnsFileContents() {
      string key = "test";
      string filename = System.IO.Path.Combine(TestPath, "test.txt");
      //Debug.Log(filename);

      System.IO.Directory.CreateDirectory(TestPath);
      System.IO.File.WriteAllText(filename, "file contents here");

      Assert.IsTrue(Settings.GetString(key) == "file contents here");
    }

    [Test]
    public void GetStringReturnsFileContentsFromSubfolders() {
      string key = "test.level1";
      string filename = Settings.FilenameFromKey(key);
      string folder = System.IO.Path.GetDirectoryName(filename);

      System.IO.Directory.CreateDirectory(folder);
      System.IO.File.WriteAllText(filename, "file contents here 2");

      Assert.IsTrue(Settings.GetString(key) == "file contents here 2");
    }

    [Test]
    public void SetStringSetsDirty() {
      string key = "test";

      Assert.IsFalse(Settings.ContainsKey(key));
      Settings.SetString(key, "value");
      Assert.IsTrue(Settings.ContainsKey(key));
      Assert.IsTrue(Settings.settings[key].dirty);
    }

    [Test]
    public void SetStringSetsData() {
      string key = "test";

      Assert.IsFalse(Settings.ContainsKey(key));
      Settings.SetString(key, "value");
      Assert.IsTrue(Settings.ContainsKey(key));
      Assert.IsTrue(Settings.settings[key].data == "value");
    }

    [Test]
    public void SaveWritesToTextFile() {
      string key = "test";
      string filename = System.IO.Path.Combine(TestPath, "test.txt");

      Assert.IsFalse(Settings.ContainsKey(key));
      Assert.IsFalse(System.IO.File.Exists(filename));
      Settings.SetString(key, "value");
      Settings.Save();
      Assert.IsTrue(System.IO.File.Exists(filename));
    }

    [Test]
    public void SaveClearsDirtyFlag() {
      string key = "test";

      Assert.IsFalse(Settings.ContainsKey(key));
      Settings.SetString(key, "value");
      Assert.IsTrue(Settings.settings[key].dirty);
      Settings.Save();
      Assert.IsFalse(Settings.settings[key].dirty);
    }

    [Test]
    public void SaveDoesNotWriteUnlessDirty() {
      string key = "test";
      string filename = System.IO.Path.Combine(TestPath, "test.txt");

      Assert.IsFalse(Settings.ContainsKey(key));
      Assert.IsFalse(System.IO.File.Exists(filename));
      Settings.SetString(key, "value");
      Settings.settings[key].dirty = false;
      Settings.Save();
      Assert.IsFalse(System.IO.File.Exists(filename));
    }

    [Test]
    public void SaveConvertsSingleDotInKeyToFolderSeparator() {
      string key = "leaderboards.level1";
      string filename = System.IO.Path.Combine(TestPath, "leaderboards" + System.IO.Path.DirectorySeparatorChar + "level1.txt");

      Assert.IsFalse(Settings.ContainsKey(key));
      Assert.IsFalse(System.IO.File.Exists(filename));
      Settings.SetString(key, "value");
      Settings.Save();
      Assert.IsTrue(System.IO.File.Exists(filename));
    }

    [Test]
    public void SaveConvertsMultipleDotsInKeyToFolderSeparators() {
      string key = "leaderboards.folder1.level1";
      string filename = System.IO.Path.Combine(TestPath, "leaderboards" + System.IO.Path.DirectorySeparatorChar + "folder1" + System.IO.Path.DirectorySeparatorChar + "level1.txt");

      Assert.IsFalse(Settings.ContainsKey(key));
      Assert.IsFalse(System.IO.File.Exists(filename));
      Settings.SetString(key, "value");
      Settings.Save();
      Assert.IsTrue(System.IO.File.Exists(filename));
    }
  }
}
