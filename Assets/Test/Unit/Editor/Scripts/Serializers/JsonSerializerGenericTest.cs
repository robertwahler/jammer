using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Jammer.Test.NewtonSoft {

  [TestFixture]
  [Category("Serializers")]
  public class JsonSerializerGenericTest : UnitTest {

    public enum EnumA {
      None = 0,
      North = 10,
      South = 20,
      East = 30,
      West = 40,
      NorthWest = 50,
    }

    public class TestDefaults {

      public int Number { get; set; }

      public EnumA NoDefault { get; set; }

      public EnumA NoDefault1 { get; set; }

      [System.ComponentModel.DefaultValue(EnumA.North)]
      public EnumA DirectionDefault { get; set; }

      [System.ComponentModel.DefaultValue(null)]
      public string Owner { get; set; }

      public string NoDefaultAttributeString { get; set; }

      [System.ComponentModel.DefaultValue(0f)]
      public float Duration { get; set; }

      public override string ToString() {
        return string.Format("Number={0}", Number);
      }
    }

    public class TestContents {

      public int Number { get; set; }

      public override string ToString() {
        return string.Format("Number={0}", Number);
      }
    }

    public class TestContainer {

      public string Id { get; set; }

      public List<TestContents> Contents { get; set; }

      public TestContainer() {
        Id = "C1";
        Contents = new List<TestContents>();
      }

      public override string ToString() {
        return string.Format("Id={0}, Contents.Count={1}", Id, Contents.Count);
      }
    }

    public class GenericContainer<T> {
      public T Value { get; set; }
    }

    [SetUp]
    public void Before() {
    }

    [TearDown]
    public void After() {
    }

    [Test]
    public void JsonSerializerHandlesGenerics() {

      var container = new GenericContainer<int>() { Value = 2 };
      string json = JsonConvert.SerializeObject(value: container, formatting: Formatting.None);
      var deserializedContainer = JsonConvert.DeserializeObject<GenericContainer<int>>(json);
      Assert.True(deserializedContainer.Value == 2);

      var container2 = new GenericContainer<float>() { Value = 2.91f };
      json = JsonConvert.SerializeObject(value: container2, formatting: Formatting.None);
      var deserializedContainer2 = JsonConvert.DeserializeObject<GenericContainer<float>>(json);
      Assert.True(deserializedContainer2.Value == 2.91f);
    }

    // TODO: add generic tests with binder class that saves type or just store
    // ints in ./int and floats in ./float. Nah, save $type, easier that
    // screwing around with separate folders


    [Test]
    public void JsonSerializerDoesNotEmitMetadata() {

      var container = new TestContainer() { Id = "C7" };
      string json = JsonConvert.SerializeObject(value: container, formatting: Formatting.None);

      Assert.True(Regex.IsMatch(json, @"C7"));
      Assert.False(Regex.IsMatch(json, @"\$type"));
    }

    [Ignore] // works properly with Json.Net from asset store
    [Test]
    public void JsonSerializerIgnoresDefaults() {

      var testDefaults = new TestDefaults() { Number=2, DirectionDefault=EnumA.North, NoDefault1=EnumA.NorthWest };
      string json = JsonConvert.SerializeObject(value: testDefaults, formatting: Formatting.None);
      Debug.Log(json);

      Assert.True(Regex.IsMatch(json, @"number"));
      Assert.False(Regex.IsMatch(json, @"directionDefault"));
    }

    [Test]
    public void JsonSerializerDeserializeFunctionsWithoutMetadata() {

      var json = @"{ ""Id"" : ""C6"", ""Contents"" : [ { ""Number"" : 1}, { ""Number"" : 2} ] }";

      var container = JsonConvert.DeserializeObject<TestContainer>(json);
      Assert.True(container.Id == "C6");
      Assert.True(container.Contents.Count == 2);
      Assert.True(container.Contents[0].Number == 1);
      Assert.True(container.Contents[1].Number == 2);
    }

    [Test]
    public void JsonSerializerDeserializeIgnoresMissingFields() {

      var json = @"{  ""Contents"" : [ { ""Number"" : 1}, { ""Number"" : 2} ] }";

      var container = JsonConvert.DeserializeObject<TestContainer>(json);
      Assert.True(container.Id == "C1");
      Assert.True(container.Contents.Count == 2);
      Assert.True(container.Contents[0].Number == 1);
      Assert.True(container.Contents[1].Number == 2);
    }

    [Test]
    public void JsonSerializerDeserializeHandlesBogusFields() {

      var json = @"{ ""Id"" : ""C6"", ""Bogus"" : ""B"", ""Contents"" : [ { ""Number"" : 1}, { ""Number"" : 2} ] }";

      var container = JsonConvert.DeserializeObject<TestContainer>(json);
      Assert.True(container.Id == "C6");
      Assert.True(container.Contents.Count == 2);
      Assert.True(container.Contents[0].Number == 1);
      Assert.True(container.Contents[1].Number == 2);
    }

  }
}
