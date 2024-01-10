# Overview
This pet project contains types and helpers for parsing, compiling and evaluating Excel-like formulas/expressions.  It supports PEMDAS mathematical operations, functions, and custom data contexts and identifiers.

# Basic Example

```csharp
using ExpressionCompiler.Evaluation;
using System;
using System.Collections.Generic;

var evaluator = new ExpressionEvaluator();
var input = "1 + 2 * 3 ^ 2";

if (evaluator.TryEvaluate(input, out object result, out List<string> errors)) {
    Console.WriteLine(result); //19
}
else {
    foreach (var error in errors) {
        Console.WriteLine(error);
    }
}
```
