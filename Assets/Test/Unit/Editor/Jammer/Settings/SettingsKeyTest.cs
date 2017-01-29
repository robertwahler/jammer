using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Jammer.Test {

  [TestFixture]
  [Category("Settings")]
  public class SettingsKeyTest : UnitTest {

    [Test]
    public void ToStringReturnsString() {
      Assert.True(SettingsKey.None.ToString() == "none");
      Assert.True(SettingsKey.Version.ToString() == "version");
    }

    [Test]
    public void ValueReturnsInt() {
      Assert.True(SettingsKey.None.Value == 0);
      Assert.True(SettingsKey.Version.Value == 800);
    }

  }
}
