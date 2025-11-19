using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Tests.SchemaFormatters;

public class BooleanFieldTest : SchemaFormatterTestBase
{
    protected override string Expression =>
        "accepted is true";

    protected override JObject ExpectedJsonSchema => JObject.Parse(@"
    {
      ""$schema"": ""http://json-schema.org/draft-07/schema#"",
      ""type"": ""object"",
      ""properties"": {
        ""accepted"": { ""type"": ""boolean"" }
      }
    }");

    protected override JObject ExpectedFlatSchema => JObject.Parse(@"
    {
      ""accepted"": ""boolean""
    }");
}