using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Tests.SchemaFormatters;

public class ArrayQuantityTest : SchemaFormatterTestBase
{
    protected override string Expression =>
        "LineItems.Exists(def x => x.Quantity > 0)";

    protected override JObject ExpectedJsonSchema => JObject.Parse(@"
    {
      ""$schema"": ""http://json-schema.org/draft-07/schema#"",
      ""type"": ""object"",
      ""properties"": {
        ""LineItems"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""properties"": {
              ""Quantity"": { ""type"": ""number"" }
            }
          }
        }
      }
    }");

    protected override JObject ExpectedFlatSchema => JObject.Parse(@"
    {
      ""LineItems"": [
        { ""Quantity"": ""number"" }
      ]
    }");
}