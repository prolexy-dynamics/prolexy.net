using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Tests.SchemaFormatters;

public class NestedDateFieldTest : SchemaFormatterTestBase
{
    protected override string Expression =>
        "brother.birthDay before 2020/10/12";

    protected override JObject ExpectedJsonSchema => JObject.Parse(@"
    {
      ""$schema"": ""http://json-schema.org/draft-07/schema#"",
      ""type"": ""object"",
      ""properties"": {
        ""brother"": {
          ""type"": ""object"",
          ""properties"": {
            ""birthDay"": { ""type"": ""string"", ""format"": ""date-time"" }
          }
        }
      }
    }");

    protected override JObject ExpectedFlatSchema => JObject.Parse(@"
    {
      ""brother"": {
        ""birthDay"": ""date-time""
      }
    }");
}