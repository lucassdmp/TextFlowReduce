using TextFlowReduce.Core;
using TextFlowReduce.Core.Models.Entity.Interfaces;
using TextFlowReduce.Core.Reducers;

var analyzer = new TextAnalyzer( new List<ITextAnalysisFunction>
    {
     new SentenceComplexity(),
     new WordLength()
    });

var text = "Primeiro parágrafo.\nSegunda frase aqui.\n\nSegundo parágrafo.";
var results = await analyzer.AnalyzeAsync(text);

foreach (var result in results)
{
    Console.WriteLine($"{result.Key}: {result.Value}");
}