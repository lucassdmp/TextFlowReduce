# TextFlowReduce

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A powerful and flexible .NET library for analyzing and scoring text-based answers against defined criteria. Perfect for automated grading systems, quiz applications, content evaluation, and educational platforms.

## 🎯 Features

- **Keyword Analysis**: Detect required and optional keywords with word boundary detection
- **Phrase Detection**: Identify complete phrases within answers
- **Weighted Scoring**: Customize the importance of different criteria components
- **Accent Normalization**: Handle accented characters seamlessly
- **Case-Insensitive**: Automatic text normalization
- **Detailed Reporting**: Get comprehensive analysis results with missing elements
- **Text Statistics**: Word and sentence counting
- **Type-Safe**: Fully typed with C# 8.0+ nullable reference types

## 📦 Installation

### NuGet Package (Coming Soon)
```bash
dotnet add package TextFlowReduce
```

### Manual Installation
Clone the repository and reference the project:
```bash
git clone https://github.com/lucassdmp/TextFlowReduce.git
```

## 🚀 Quick Start

Here's a simple example to get you started:

```csharp
using TextFlowReduce.Core.Analyzers;
using TextFlowReduce.Core.Models;

// Define your criteria
var criteria = new AnswerCriteria
{
    RequiredKeywords = new() { "class", "object" },
    RequiredPhrases = new() { "object-oriented programming" },
    OptionalKeywords = new() { "inheritance", "polymorphism" },
    RequiredKeywordsWeight = 0.5,
    RequiredPhrasesWeight = 0.3,
    OptionalKeywordsWeight = 0.2
};

// Analyze an answer
string answer = "Object-oriented programming uses class and object concepts. " +
                "Inheritance and polymorphism are key features.";

var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

Console.WriteLine($"Score: {result.FinalScore}/100");
Console.WriteLine($"Found Keywords: {result.FoundRequiredKeywords.Count}");
Console.WriteLine($"Missing Keywords: {result.MissingRequiredKeywords.Count}");
```

## 📖 Usage Guide

### 1. Creating Answer Criteria

The `AnswerCriteria` class defines what you're looking for in an answer:

```csharp
var criteria = new AnswerCriteria
{
    // Keywords that MUST appear (case-insensitive, accent-insensitive)
    RequiredKeywords = new List<string> 
    { 
        "photosynthesis", 
        "chlorophyll", 
        "oxygen" 
    },
    
    // Complete phrases that MUST appear
    RequiredPhrases = new List<string> 
    { 
        "light energy", 
        "carbon dioxide" 
    },
    
    // Keywords that provide bonus points if present
    OptionalKeywords = new List<string> 
    { 
        "chloroplast", 
        "ATP", 
        "NADPH" 
    },
    
    // Weights must sum to 1.0
    RequiredKeywordsWeight = 0.4,  // 40% of final score
    RequiredPhrasesWeight = 0.4,   // 40% of final score
    OptionalKeywordsWeight = 0.2   // 20% of final score
};
```

### 2. Analyzing Answers

```csharp
string studentAnswer = @"Photosynthesis is the process by which plants 
    convert light energy into chemical energy. Chlorophyll absorbs light 
    and uses carbon dioxide from the air to produce glucose and release 
    oxygen. This occurs in chloroplasts and involves ATP production.";

var result = AnswerAnalyzer.AnalyzeAnswer(studentAnswer, criteria);
```

### 3. Understanding Results

The `AnswerAnalysisResult` provides comprehensive information:

```csharp
// Overall score (0-100)
Console.WriteLine($"Final Score: {result.FinalScore}");

// Individual component scores
Console.WriteLine($"Required Keywords Score: {result.RequiredKeywordsScore}");
Console.WriteLine($"Required Phrases Score: {result.RequiredPhrasesScore}");
Console.WriteLine($"Optional Keywords Score: {result.OptionalKeywordsScore}");

// What was found
Console.WriteLine($"Found Keywords: {string.Join(", ", result.FoundRequiredKeywords)}");
Console.WriteLine($"Found Phrases: {string.Join(", ", result.FoundRequiredPhrases)}");
Console.WriteLine($"Found Optional: {string.Join(", ", result.FoundOptionalKeywords)}");

// What was missing
Console.WriteLine($"Missing Keywords: {string.Join(", ", result.MissingRequiredKeywords)}");
Console.WriteLine($"Missing Phrases: {string.Join(", ", result.MissingRequiredPhrases)}");

// Text statistics
Console.WriteLine($"Total Words: {result.TotalWords}");
Console.WriteLine($"Total Sentences: {result.TotalSentences}");
```

### 4. Detailed Reporting

Get a formatted report with all analysis details:

```csharp
string report = result.GetDetailedReport();
Console.WriteLine(report);
```

Output example:
```
Score Final: 85.00/100
- Palavras-chave obrigatórias: 100.00/100 (3 de 3 encontradas)
- Frases obrigatórias: 100.00/100 (2 de 2 encontradas)
- Palavras-chave opcionais: 66.67/100 (2 encontradas)
- Total de palavras: 45
- Total de sentenças: 3

Palavras-chave opcionais encontradas:
  - chloroplast
  - ATP
```

## 🎓 Complete Examples

### Example 1: Quiz Application

```csharp
var criteria = new AnswerCriteria
{
    RequiredKeywords = new() { "water", "cycle", "evaporation", "precipitation" },
    RequiredPhrases = new() { "state change" },
    OptionalKeywords = new() { "condensation", "infiltration", "runoff" },
    RequiredKeywordsWeight = 0.5,
    RequiredPhrasesWeight = 0.3,
    OptionalKeywordsWeight = 0.2
};

string answer1 = "The water cycle involves evaporation and precipitation.";
string answer2 = @"The water cycle is a continuous process where water undergoes 
    state change through evaporation, condensation, and precipitation. 
    Water evaporates from surfaces, forms clouds, and returns as rain, 
    with some infiltration into the ground and runoff into rivers.";

var result1 = AnswerAnalyzer.AnalyzeAnswer(answer1, criteria);
var result2 = AnswerAnalyzer.AnalyzeAnswer(answer2, criteria);

Console.WriteLine($"Student 1 Score: {result1.FinalScore}/100");
Console.WriteLine($"Student 2 Score: {result2.FinalScore}/100");
```

### Example 2: Programming Assessment

```csharp
var criteria = new AnswerCriteria
{
    RequiredKeywords = new() { "variable", "function", "loop", "conditional" },
    RequiredPhrases = new() { "control flow" },
    OptionalKeywords = new() { "algorithm", "syntax", "debugging" },
    RequiredKeywordsWeight = 0.6,
    RequiredPhrasesWeight = 0.2,
    OptionalKeywordsWeight = 0.2
};

string answer = @"Programming basics include variable declaration, 
    function definition, and control flow through loop and conditional 
    statements. Understanding syntax and debugging are essential for 
    developing efficient algorithms.";

var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

if (result.FinalScore >= 80)
{
    Console.WriteLine("Excellent understanding!");
}
else if (result.FinalScore >= 60)
{
    Console.WriteLine("Good, but review: " + 
        string.Join(", ", result.MissingRequiredKeywords));
}
else
{
    Console.WriteLine("Needs improvement");
}
```

### Example 3: Content Validation

```csharp
var criteria = new AnswerCriteria
{
    RequiredKeywords = new() { "security", "encryption", "authentication" },
    RequiredPhrases = new() { "data protection" },
    OptionalKeywords = new() { "SSL", "HTTPS", "firewall", "VPN" },
    RequiredKeywordsWeight = 0.5,
    RequiredPhrasesWeight = 0.3,
    OptionalKeywordsWeight = 0.2
};

string blogPost = @"Ensuring data protection is crucial in modern applications. 
    Implement security through encryption and strong authentication mechanisms. 
    Use HTTPS with SSL certificates and consider VPN for remote access.";

var result = AnswerAnalyzer.AnalyzeAnswer(blogPost, criteria);

if (result.FinalScore >= 75)
{
    Console.WriteLine("Content meets quality standards");
}
else
{
    Console.WriteLine("Content needs these improvements:");
    foreach (var missing in result.MissingRequiredKeywords)
    {
        Console.WriteLine($"  - Add information about: {missing}");
    }
}
```

## ⚙️ Advanced Features

### Weight Validation

Weights are automatically validated to ensure they sum to 1.0:

```csharp
var criteria = new AnswerCriteria
{
    RequiredKeywordsWeight = 0.5,
    RequiredPhrasesWeight = 0.3,
    OptionalKeywordsWeight = 0.3  // Invalid! Sum = 1.1
};

try
{
    criteria.ValidateWeights();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message); // "A soma dos pesos deve ser 1.0..."
}
```

### Word Boundary Detection

The analyzer uses word boundaries to prevent false positives:

```csharp
var criteria = new AnswerCriteria
{
    RequiredKeywords = new() { "sun" },
    RequiredKeywordsWeight = 1.0,
    RequiredPhrasesWeight = 0.0,
    OptionalKeywordsWeight = 0.0
};

// "sun" appears in "sunshine" but won't match (word boundary)
string answer = "The sunshine is bright today.";
var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

Console.WriteLine(result.RequiredKeywordsScore); // 0.0
```

### Accent Normalization

Automatically handles accented characters:

```csharp
var criteria = new AnswerCriteria
{
    RequiredKeywords = new() { "agua", "acido" },  // Without accents
    RequiredKeywordsWeight = 1.0,
    RequiredPhrasesWeight = 0.0,
    OptionalKeywordsWeight = 0.0
};

string answer = "A água pode ser ácida.";  // With accents
var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

Console.WriteLine(result.RequiredKeywordsScore); // 100.0
```

## 🧪 Testing

The library includes comprehensive unit and integration tests:

```bash
cd TextFlowReduce.Tests
dotnet test
```

### Test Coverage
- Unit tests for all core functionality
- Integration tests for real-world scenarios
- Edge case handling
- Performance testing with long texts

## 📊 Scoring System

The final score is calculated using the formula:

```
FinalScore = (RequiredKeywordsScore × RequiredKeywordsWeight) +
             (RequiredPhrasesScore × RequiredPhrasesWeight) +
             (OptionalKeywordsScore × OptionalKeywordsWeight)
```

Where:
- Each component score ranges from 0 to 100
- Weights must sum to exactly 1.0
- Final score ranges from 0 to 100

### Score Components

1. **Required Keywords Score**: Percentage of required keywords found
2. **Required Phrases Score**: Percentage of required phrases found
3. **Optional Keywords Score**: Percentage of optional keywords found (bonus points)

## 🛠️ Requirements

- .NET 8.0 or later
- C# 8.0+ (nullable reference types support)

## 📝 API Reference

### AnswerCriteria Class

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| RequiredKeywords | List&lt;string&gt; | Keywords that must appear | Empty list |
| RequiredPhrases | List&lt;string&gt; | Phrases that must appear | Empty list |
| OptionalKeywords | List&lt;string&gt; | Keywords that provide bonus | Empty list |
| RequiredKeywordsWeight | double | Weight for required keywords (0-1) | 0.4 |
| RequiredPhrasesWeight | double | Weight for required phrases (0-1) | 0.4 |
| OptionalKeywordsWeight | double | Weight for optional keywords (0-1) | 0.2 |

### AnswerAnalysisResult Class

| Property | Type | Description |
|----------|------|-------------|
| FinalScore | double | Overall score (0-100) |
| RequiredKeywordsScore | double | Score for required keywords (0-100) |
| RequiredPhrasesScore | double | Score for required phrases (0-100) |
| OptionalKeywordsScore | double | Score for optional keywords (0-100) |
| FoundRequiredKeywords | List&lt;string&gt; | List of found required keywords |
| MissingRequiredKeywords | List&lt;string&gt; | List of missing required keywords |
| FoundRequiredPhrases | List&lt;string&gt; | List of found required phrases |
| MissingRequiredPhrases | List&lt;string&gt; | List of missing required phrases |
| FoundOptionalKeywords | List&lt;string&gt; | List of found optional keywords |
| TotalWords | int | Total word count in answer |
| TotalSentences | int | Total sentence count in answer |

### AnswerAnalyzer Class

#### AnalyzeAnswer Method

```csharp
public static AnswerAnalysisResult AnalyzeAnswer(string answer, AnswerCriteria criteria)
```

**Parameters:**
- `answer`: The text answer to analyze
- `criteria`: The criteria to evaluate against

**Returns:** `AnswerAnalysisResult` with detailed analysis

**Exceptions:**
- `ArgumentException`: If answer is null or empty
- `ArgumentNullException`: If criteria is null
- `InvalidOperationException`: If criteria weights don't sum to 1.0

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👤 Author

**João Lucas** - [@lucassdmp](https://github.com/lucassdmp)
