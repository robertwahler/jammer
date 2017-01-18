using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Jammer.Test.NewtonSoft {

  [TestFixture]
  [Category("Serializers")]
  public class JsonSerializerObjectTest : UnitTest {

    public class TestContainer {

      public string Id { get; set; }

      public object Data { get; set; }

      public override string ToString() {
        return string.Format("Id={0}, Data={1}", Id, Data);
      }
    }

    [SetUp]
    public void Before() {
    }

    [TearDown]
    public void After() {
    }

    [Test]
    public void JsonSerializerHandlesObjectData() {

      var container = new TestContainer();
      container.Id = "1a";
      container.Data = @"{ ""id"":  ""birds"" }";
      //Debug.Log(container);

      string json = JsonConvert.SerializeObject(value: container, formatting: Formatting.None);
      //Debug.Log(json);

      var deserializedContainer = JsonConvert.DeserializeObject<TestContainer>(json);
      //Debug.Log(deserializedContainer.Data.GetType());
      Assert.True(deserializedContainer.Data.GetType() == typeof(System.String));
      Assert.True(deserializedContainer.Id == "1a");
      Assert.True((string) deserializedContainer.Data == @"{ ""id"":  ""birds"" }");

      var birds = JsonConvert.DeserializeObject<TestContainer>((string) deserializedContainer.Data);
      //Debug.Log(birds);
      Assert.True(birds.Id == "birds");
    }

    [Test]
    public void JsonSerializerHandlesComplexObjectData() {

      string json = @"{ ""id"":  ""dogs"", ""data"": {""a"": true, ""b"": false, ""c"": [1, 2, 3]}}";
      TestContainer dogs = JsonConvert.DeserializeObject<TestContainer>(json);
      //Debug.Log(dogs);
      //Debug.Log(dogs.Data.GetType());
      //Debug.Log(dogs.Data);
      Assert.True(dogs.Id == "dogs");
      Assert.True(dogs.Data.GetType() == typeof(Newtonsoft.Json.Linq.JObject));
    }

  }
}
