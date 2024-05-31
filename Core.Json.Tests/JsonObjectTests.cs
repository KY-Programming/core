using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KY.Core.Tests;

[TestClass]
public class JsonObjectTests
{
    [TestMethod]
    public void Set_a_property_of_an_existing_Object()
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
        Json.Object.Set(jObject, "name", "Jack");
        Assert.AreEqual("Jack", jObject["name"]);
    }
    
    [TestMethod]
    public void Set_a_property_of_an_existing_Depp_Object()
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
        Json.Object.Set(jObject, "children.[1].name", "Jude");
        Assert.AreEqual("Jude", jObject["children"]?[1]?["name"]);
    }
    
    [TestMethod]
    public void Remove_a_property_of_an_existing_Object()
    { 
        JObject jObject = JsonConvert.DeserializeObject<JObject>("""
                                                                 {
                                                                     "name": "John Doe",
                                                                     "age": 42
                                                                 }
                                                                 """)!;
        Json.Object.Remove(jObject, "age");
        Assert.AreEqual("{\"name\":\"John Doe\"}", jObject.ToString(Formatting.None));
    }
}
