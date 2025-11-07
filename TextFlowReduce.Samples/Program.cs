using System;
using TextFlowReduce.Core.Analyzers;
using TextFlowReduce.Core.Models;

public class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine("=== Debug Test 1 ===");
		var criteria1 = new AnswerCriteria
		{
			RequiredKeywords = new() { "classe", "objeto" },
			RequiredPhrases = new(),
			OptionalKeywords = new(),
			RequiredKeywordsWeight = 1.0,
			RequiredPhrasesWeight = 0.0,
			OptionalKeywordsWeight = 0.0
		};
		string answer1 = "Uma classe é um modelo para criar objetos.";
		var result1 = AnswerAnalyzer.AnalyzeAnswer(answer1, criteria1);
		Console.WriteLine($"Answer: {answer1}");
		Console.WriteLine($"Final Score: {result1.FinalScore}");
		Console.WriteLine($"Found: {string.Join(", ", result1.FoundRequiredKeywords)}");
		Console.WriteLine($"Missing: {string.Join(", ", result1.MissingRequiredKeywords)}");

		Console.WriteLine("\n=== Analisador de Respostas ===\n");

		Console.WriteLine("--- Exemplo 1: Fotossíntese ---\n");

		var criteriaFotossintese = new AnswerCriteria
		{
			RequiredKeywords = new() { "luz", "clorofila", "oxigênio", "glicose" },
			RequiredPhrases = new() { "energia luminosa", "dióxido de carbono" },
			OptionalKeywords = new() { "cloroplasto", "fotossistema", "ATP", "NADPH" },
			RequiredKeywordsWeight = 0.4,
			RequiredPhrasesWeight = 0.4,
			OptionalKeywordsWeight = 0.2
		};

		string respostaFotossintese = @"A fotossíntese é o processo pelo qual as plantas convertem 
		energia luminosa em energia química. Durante este processo, a clorofila nas folhas absorbe luz 
		e utiliza dióxido de carbono do ar e água do solo para produzir glicose e liberar oxigênio. 
		Este processo ocorre nos cloroplastos e envolve a produção de ATP e NADPH.";

		var resultadoFotossintese = AnswerAnalyzer.AnalyzeAnswer(respostaFotossintese, criteriaFotossintese);
		Console.WriteLine(resultadoFotossintese.GetDetailedReport());

		Console.WriteLine("\n\n--- Exemplo 2: Programação Orientada a Objetos ---\n");

		var criteriaPOO = new AnswerCriteria
		{
			RequiredKeywords = new() { "classe", "objeto", "herança", "polimorfismo" },
			RequiredPhrases = new() { "encapsulamento de dados" },
			OptionalKeywords = new() { "abstração", "interface", "método", "propriedade" },
			RequiredKeywordsWeight = 0.5,
			RequiredPhrasesWeight = 0.3,
			OptionalKeywordsWeight = 0.2
		};

		string respostaPOO = @"Programação Orientada a Objetos é um paradigma que organiza o código 
     em classes e objetos. Uma classe é um modelo que define propriedades e métodos, enquanto 
um objeto é uma instância dessa classe. Os pilares incluem herança, que permite criar 
      classes derivadas, e polimorfismo, que permite diferentes implementações. A abstração 
         ajuda a simplificar sistemas complexos através de interfaces.";

		var resultadoPOO = AnswerAnalyzer.AnalyzeAnswer(respostaPOO, criteriaPOO);
		Console.WriteLine(resultadoPOO.GetDetailedReport());

		Console.WriteLine("\n\n--- Exemplo 3: Resposta Incompleta ---\n");

		string respostaIncompleta = @"POO usa classes e objetos. Herança é importante.";

		var resultadoIncompleto = AnswerAnalyzer.AnalyzeAnswer(respostaIncompleta, criteriaPOO);
		Console.WriteLine(resultadoIncompleto.GetDetailedReport());

		Console.WriteLine("\n\nPressione qualquer tecla para sair...");
		Console.ReadKey();
	}
}