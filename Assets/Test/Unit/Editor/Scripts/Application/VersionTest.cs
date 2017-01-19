using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using NUnit.Framework;
using SDD;

namespace Jammer.Test {

  [TestFixture]
  [Category("Application")]
  public class VersionTest : UnitTest {

    private Version version;

    [SetUp]
    public void Before() {
      version = new Version();

      version.version = "2.0.1";
      version.code = 10;
      version.build = "1EF22";
      version.label = "";
    }

    [TearDown]
    public void After() {
      version = null;
    }

    [Test]
    public void ToStringReturnsVersionCodeBuild() {
      //Debug.Log(version);
      Assert.True(Regex.IsMatch(version.ToString(), @"^2.0.1\+10.1EF22"));
    }

    [Test]
    public void ToStringReturnsVersionLabelCodeBuild() {
      version.label = "beta";
      //Debug.Log(version);
      Assert.True(Regex.IsMatch(version.ToString(), @"^2.0.1\-beta\+10.1EF22"));
    }

  }
}

