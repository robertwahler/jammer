using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using NUnit.Framework;

namespace Jammer.Test {

  [TestFixture]
  [Category("Application")]
  public class AudioManagerTest : UnitTest {

    private GameObject TestContainer { get; set; }
    private AudioManager audioManager;

    [SetUp]
    public void Before() {
      TestContainer = new GameObject();
      audioManager = TestContainer.AddComponent<AudioManager>();
      audioManager.name = "audioManager";
      // Testing MonoBehaviours requires taking manual control. Call a
      // protected method for setup.
      audioManager.Call("Awake");
    }

    [TearDown]
    public void After() {
      // Testing MonoBehaviours requires taking manual control. Call a
      // protected method for cleanup.
      audioManager.Call("OnDisable");
      if (TestContainer != null) {
        GameObject.DestroyImmediate(TestContainer);
      }
      TestContainer = null;
      audioManager = null;
    }

    [Test]
    public void MuteAudioDefaultsToFalse() {
      Assert.IsTrue(audioManager.MuteAudio == false);
    }

    [Test]
    public void SaveWritesAudioSettingsToJson() {
      var paths = new string[] {TestPath, "audio.txt"};
      string filename = paths.Aggregate(System.IO.Path.Combine);
      Assert.False(System.IO.File.Exists(filename));

      audioManager.Save();
      Assert.True(System.IO.File.Exists(filename));

      string contents = System.IO.File.ReadAllText(filename);
      Assert.True(Regex.IsMatch(contents, @".MuteAudio.: false"));
    }
  }
}
