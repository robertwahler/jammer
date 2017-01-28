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
  public class ApplicationHelperTest : UnitTest {

    [SetUp]
    public void Before() {
    }

    [TearDown]
    public void After() {
    }

    [Test]
    public void VersionReturnsVersionNumber() {
      //Debug.Log(ApplicationHelper.Version);
      Assert.True(Regex.IsMatch(ApplicationHelper.Version.ToString(), @"^[\d]\.[\d]\.[\d]\+"));
    }


  }
}

