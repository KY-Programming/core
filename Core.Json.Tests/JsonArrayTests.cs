using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KY.Core.Tests;

[TestClass]
public class JsonArrayTests
{
    [TestMethod]
    public void Add_a_String_to_an_existing_Array()
    {
        JObject jObject = JsonConvert.DeserializeObject<JObject>("""
                                                                 {
                                                                     "name": "John Doe",
                                                                     "age": 42,
                                                                     "children": [
                                                                         "John Jr.",
                                                                         "Jane"
                                                                     ]
                                                                 }
                                                                 """)!;
        Json.Array.Add(jObject, "children", "Jack");
        Assert.AreEqual(3, jObject["children"]?.Count());
        Assert.AreEqual("John Jr.", jObject["children"]?[0]);
        Assert.AreEqual("Jane", jObject["children"]?[1]);
        Assert.AreEqual("Jack", jObject["children"]?[2]);
    }

    [TestMethod]
    public void Add_a_Object_to_an_existing_Array()
    {
        JObject jObject = JsonConvert.DeserializeObject<JObject>("""
                                                                  {
                                                                      "name": "John Doe",
                                                                      "age": 42,
                                                                      "children": [
                                                                          {
                                                                              "name": "John Jr.",
                                                                              "age": 13
                                                                          },
                                                                          {
                                                                              "name": "Jane",
                                                                              "age": 9
                                                                          }
                                                                      ]
                                                                  }
                                                                  """)!;
        Json.Array.Add(jObject, "children", JsonConvert.DeserializeObject<JObject>("""
                                                                                    {
                                                                                       "name": "Jack",
                                                                                       "age": 2
                                                                                   }
                                                                                   """));
        Assert.AreEqual(3, jObject["children"]?.Count());
        Assert.AreEqual("Jack", jObject["children"]?[2]?["name"]);
    }
    
    [TestMethod]
    public void Remove_a_String_from_an_existing_Array()
    {
        JObject jObject = JsonConvert.DeserializeObject<JObject>("""
                                                                 {
                                                                     "name": "John Doe",
                                                                     "age": 42,
                                                                     "children": [
                                                                         "John Jr.",
                                                                         "Jane"
                                                                     ]
                                                                 }
                                                                 """)!;
        Json.Array.Remove(jObject, "children", "Jane");
        Assert.AreEqual(1, jObject["children"]?.Count());
        Assert.AreEqual("John Jr.", jObject["children"]?[0]);
    }
    
    [TestMethod]
    public void Remove_a_String_from_an_existing_Deep_Array()
    {
        JObject jObject = JsonConvert.DeserializeObject<JObject>("""
                                                                 {
                                                                    "jd": {
                                                                         "name": "John Doe",
                                                                         "age": 42,
                                                                         "children": [
                                                                             "John Jr.",
                                                                             "Jane"
                                                                         ]
                                                                      }
                                                                 }
                                                                 """)!;
        Json.Array.Remove(jObject, "jd.children", "Jane");
        Assert.AreEqual(1, jObject["jd"]?["children"]?.Count());
        Assert.AreEqual("John Jr.", jObject["jd"]?["children"]?[0]);
    }
}
