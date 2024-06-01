# prolexy.net
dotnet backend library for prolexy scripting language

## Installation

```shell
dotnet install prolexy.net
```

## Usage

- define prolexy context as a type

```csharp
public class BusinessObject
{
    public string Name { get; set; }
    public DateTime BirthDay { get; set; }
    public bool Marrried { get; set; }
}
```
- Create evaluator builder:

```csharp
var evaluatorBuilder = EvaluatorContextBuilder.Default
    .AsClrEvaluatorBuilder();
```
- 
