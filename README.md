# prolexy.net
dotnet backend library for prolexy scripting language
# Prolexy.NET

**Prolexy.NET** is the .NET compiler and runtime for the **Prolexy scripting language**.

Prolexy allows applications to parse, analyze, compile, and execute dynamic expressions and rules against CLR objects and JSON documents.

It is designed not only for executing expressions, but also for providing the information required to build developer and user-facing tooling around the language — such as type analysis, schema inference, autocomplete, validation, and dynamic forms.

## Why Prolexy?

In many business applications, parts of the business logic need to be dynamic.

Instead of hard-coding every rule into the application, Prolexy allows those rules to be expressed as scripts:

```text
age >= 18 and accepted is true
```

The same language can then be used to:

* evaluate expressions
* execute rules and assignments
* inspect the return type of an expression
* infer the data schema required by an expression
* generate logical schemas from CLR types
* work with both CLR objects and JSON documents

This makes Prolexy useful as a foundation for **dynamic business rules, document templates, configuration-driven behavior, and language tooling**.

---

## Installation

```shell
dotnet install prolexy.net
```
---

## Features

### Expression Evaluation

Expressions can be compiled and evaluated against a CLR business object.

```csharp
var compiler = new Implementations.Compiler();

var evaluator = compiler
    .CompileExpression("age >= 18 and accepted is true")
    .AsClrEvaluator();

var context = EvaluatorContextBuilder.Default
    .AsClrEvaluatorBuilder()
    .WithBusinessObject(new
    {
        age = 20,
        accepted = true
    })
    .Build();

var result = evaluator.Evaluate(context)?.Value;
```

Prolexy supports expressions such as:

```text
name
age + 10
name + ' ' + family

age >= 18
name contains 'Yaser'
name starts with 'Ya'

birthDay before 2020/10/13

accepted is true
nothing is null
```

It also supports nested member access, method calls, object creation, conditional expressions, collection operations, and more.

For example:

```text
person.name
OrderDate.AddDays(1)

new Person('Yaser').Name

Grades.Sum(def x => x)
Grades.Exists(def x => x > 10)

Iff(age >= 18, 'adult', 'minor')
```

---

## Rules and Statements

Prolexy is not limited to single expressions. A script can contain statements and modify values in the evaluation context.

For example:

```text
set fullname with name + ' ' + family
```

Conditional rules are also supported:

```text
if(age >= 18) then
    set accepted with true
else
    set accepted with false
end
```

Rules can be compiled and executed against an evaluation context.

---

## CLR and JSON

Prolexy can operate on different kinds of data.

### CLR

A CLR object can be used as the business object:

```csharp
var context = EvaluatorContextBuilder.Default
    .AsClrEvaluatorBuilder()
    .WithBusinessObject(order)
    .Build();
```

CLR types can also be registered when expressions need to create or access them:

```csharp
var context = EvaluatorContextBuilder.Default
    .AsClrEvaluatorBuilder()
    .WithBusinessObject(order)
    .AddClrType<Person>()
    .Build();
```

### JSON

Expressions can also be evaluated against JSON data.

```csharp
var evaluator = compiler
    .CompileExpression(expression)
    .AsJsonEvaluator();
```

A JSON evaluation context can be created with a logical schema:

```csharp
var context = EvaluatorContextBuilder.Default
    .AsJsonEvaluatorBuilder()
    .WithSchema(schema)
    .WithBusinessObject(JObject.Parse(json))
    .Build();
```

This makes Prolexy suitable for scenarios where the data structure is dynamic and is not represented by a CLR class.

---

# Type Detection

One of the important capabilities of Prolexy is the ability to determine the return type of an expression **without executing it**.

For example:

```text
age + 10
```

can be analyzed as a `decimal` expression, while:

```text
name == 'Yaser'
```

is known to return `bool`.

A type detection context can be built from the CLR type of the business object:

```csharp
var evalContext = EvaluatorContextBuilder
    .Default
    .AsClrEvaluatorBuilder()
    .AsExpressionTypeDetectorContextBuilder()
    .WithBusinessObjectType(context.GetType())
    .Build();

var evaluator = compiler
    .CompileExpression("age + 10")
    .AsExpressionClrReturnTypeEvaluator();

var result = evaluator.Evaluate(evalContext);

Console.WriteLine(result.Result);
```

The important part is that this can be done using the **type of the business object**, without requiring an actual instance of the object.

This capability is particularly useful for language tooling such as:

* autocomplete
* semantic validation
* editor hints
* expression analysis
* determining available members and methods

---

# Schema Inference

Prolexy can also go one step further than return-type detection.

An expression can be analyzed to determine the **schema of the data required to evaluate it**.

This is especially useful when expressions operate on JSON documents whose structure is not known at compile time.

For example, imagine a document template containing expressions that reference data such as:

```text
customer.name
customer.address.city
order.items
```

Instead of manually defining the required input model, the expression can be analyzed and the required schema can be derived from it.

This enables an **expression-driven data model**:

```text
Expression
    │
    ▼
Schema Inference
    │
    ▼
Required JSON Schema
```

The schema can then be stored together with the template.

---

# From Templates to Dynamic Forms

One practical use case for schema inference is dynamic document generation.

For example, a business application can allow a user to design a document template containing Prolexy expressions.

When the template is saved:

```text
Document Template
        │
        ▼
   Prolexy Analysis
        │
        ▼
      Schema
        │
        └───────────────┐
                        │
                 Store with Template
```

Later, when the user wants to execute the template, the stored schema can be used to generate a dynamic form:

```text
Stored Template
      +
Stored Schema
      │
      ▼
Dynamic Form
      │
      ▼
User Input
      │
      ▼
JSON Document
      │
      ▼
Prolexy Execution
      │
      ▼
Generated Document
```

This means the same language infrastructure can be used to drive both **execution** and **user interaction**.

The schema is not merely metadata. It becomes the contract between the template and the UI that collects the data required by the template.

---

# Schema Generation from CLR Types

Prolexy also provides schema generation from CLR types.

For example:

```csharp
var context = EvaluatorContextBuilder
    .Default
    .AsClrEvaluatorBuilder()
    .AsSchemaGeneratorContextBuilder<SimpleType>()
    .Build();

var schema = new SchemaGenerator().Generate(context);
```

The generated logical schema can describe:

* primitive properties
* complex types
* collections
* methods
* constructors
* references between complex types

For example, a CLR type containing:

```csharp
string Name
decimal Age
DateTime RegistrationDate
bool Accepted
```

can be represented in Prolexy's logical schema model.

This allows CLR models to become part of the same schema-driven ecosystem used by JSON and dynamic tooling.

---

# Collections and Higher-Order Expressions

Prolexy supports collection operations and lambda-like expressions.

Examples include:

```text
Grades.Sum(def x => x)

Grades.Aggregate(0, def x, y => x + y)

Grades.First()

Grades.Single(def x => x > 10)

Grades.Exists(def x => x > 10)

Grades.Count()
```

Expressions can also compose these operations:

```text
Iff(
    Grades.Sum(def x => x) > 15,
    'approved',
    'rejected'
)
```

---

# Expression Language

The language provides a syntax designed for business-oriented expressions.

Examples include:

```text
is null
is not null

is
is not

contains
not contains

starts with
not starts with

ends with
not ends with

before
before or equal to

after
after or equal to
```

This makes rules readable without requiring every business rule to be expressed directly in C#.

---

# Architecture

At its core, Prolexy is built around a compiler and AST-based architecture.

The general pipeline is:

```text
Prolexy Source
      │
      ▼
    Parser
      │
      ▼
     AST
      │
      ├───────────────┐
      │               │
      ▼               ▼
  Evaluation     Type Analysis
      │               │
      ▼               ▼
 CLR / JSON       Return Type
   Runtime
      │
      ▼
Schema Analysis
```

AST visitors provide the foundation for different kinds of analysis and execution.

This architecture allows new capabilities to operate on the same language representation rather than implementing a separate parser for every feature.

---

# Tooling

Prolexy.NET is also the foundation for the Prolexy editor ecosystem.

A client can use the language analysis capabilities to provide features such as:

* syntax diagnostics
* semantic diagnostics
* autocomplete
* type information
* schema-aware editing

This is particularly useful when users are authoring expressions or templates rather than writing C# code.

---

# Example: A Dynamic Business Rule

A rule can combine member access, collection operations, conditions, and assignments.

For example:

```text
if(LineItems.Exists(def x =>
    x.Product is 'special product'))
then
    set DiscountPercentage with 10
end
```

More complex rules can be composed from multiple statements and conditions.

The application does not need to compile a new C# assembly for every rule. The rule remains data that can be stored, analyzed, compiled, and executed by Prolexy.

---

# Project Structure

The main compiler project is organized around several concepts:

```text
Prolexy.Compiler
├── Ast
├── Implementations
├── Models
├── SchemaGenerators
├── Visitors
│   └── TypeDetectorVisitors
├── ExtensionMethods
└── ...
```

The test suite covers areas including:

```text
Evaluator
ExpressionTypeDetectors
LogicalSchemaGenerators
ParserTests
SchemaFormatters
```

This also serves as a collection of examples of the language syntax and runtime behavior.

---

# Design Goals

Prolexy is intended to provide a common language infrastructure for applications that need dynamic behavior.

Instead of building separate mechanisms for:

* parsing expressions
* executing rules
* discovering types
* validating expressions
* describing JSON data
* generating dynamic forms

these capabilities can be built around the same language and schema model.

The goal is to make a Prolexy expression useful not only at **runtime**, but throughout the lifecycle of a dynamic feature:

```text
Author
  │
  ▼
Expression / Template
  │
  ├── Parse
  ├── Validate
  ├── Infer Types
  ├── Infer Schema
  │
  ▼
Store
  │
  ▼
Collect Data
  │
  ▼
Execute
  │
  ▼
Result
```

---

# Status

Prolexy is an evolving project and its language, compiler APIs, and tooling are still under development.

The repository currently contains the core compiler/runtime and its test suite, while the editor/tooling layer is being developed separately.
