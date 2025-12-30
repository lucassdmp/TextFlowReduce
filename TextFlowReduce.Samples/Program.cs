using System;
using TextFlowReduce.Samples;

public class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine("=== TextFlowReduce - Análise de Respostas ===\n");
		QuestionAnalyzer.RunBulkAnalysisFromCsv();
	}
}
