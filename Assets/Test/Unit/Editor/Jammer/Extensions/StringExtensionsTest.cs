using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using Jammer.Extensions;

namespace Jammer.Test {

  [TestFixture]
  [Category("Extensions")]
  public class StringExtensionTest : UnitTest  {

    [SetUp]
    public void Before() {
    }

    [TearDown]
    public void After() {
    }

    [Test]
    public void CapitalizeUppercasesTheFirstLetter() {
      string text = "my old dog";
      Assert.True(text.Capitalize() == "My old dog");

      text = "My cat";
      Assert.True(text.Capitalize() == "My cat");
    }

    [Test]
    public void StripBlankLinesRemovesAllBlankLines() {
      string text = "\n{ some text\n\n\nhere}\n\n";
      string strippedText = text.StripBlankLines();
      Assert.True(strippedText ==  "{ some text\nhere}");
    }
  }
}

