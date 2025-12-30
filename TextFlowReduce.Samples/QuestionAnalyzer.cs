using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextFlowReduce.Core.Analyzers;
using TextFlowReduce.Core.Models;

namespace TextFlowReduce.Samples
{
	/// <summary>
	/// Analisador de questões baseado em critérios CSV
	/// </summary>
	public class QuestionAnalyzer
	{
		public static void RunBulkAnalysisFromCsv()
		{
			Console.Clear();
			Console.WriteLine("=== Análise em Lote de Arquivo CSV ===\n");

			var questions = GetQuestions();

			// Opção 1: Criar arquivo de exemplo
			Console.WriteLine("Opções:");
			Console.WriteLine("1 - Criar arquivo CSV de exemplo");
			Console.WriteLine("2 - Analisar arquivo CSV existente");
			Console.Write("\nEscolha uma opção: ");

			var option = Console.ReadLine();

			if (option == "1")
			{
				CreateSampleCsvFile(questions);
				return;
			}

			// Solicitar caminho do arquivo
			Console.Write("\nDigite o caminho completo do arquivo CSV: ");
			var filePath = Console.ReadLine()?.Trim().Trim('"');

			if (string.IsNullOrEmpty(filePath))
			{
				Console.WriteLine("\nCaminho inválido!");
				Console.WriteLine("Pressione qualquer tecla para voltar...");
				Console.ReadKey();
				return;
			}

			try
			{
				// Ler respostas do CSV
				Console.WriteLine("\nLendo arquivo CSV...");
				var studentAnswerSets = CsvQuestionReader.ReadStudentAnswersFromCsv(filePath);

				if (studentAnswerSets.Count == 0)
				{
					Console.WriteLine("\nNenhuma resposta encontrada no arquivo!");
					Console.WriteLine("Pressione qualquer tecla para voltar...");
					Console.ReadKey();
					return;
				}

				Console.WriteLine($"\n✓ {studentAnswerSets.Count} estudante(s) encontrado(s)!");
				Console.WriteLine("\nIniciando análise...\n");

				// Processar análise em lote
				var allResults = AnalyzeAllStudents(studentAnswerSets, questions);

				// Exibir resultados
				DisplayBulkAnalysisResults(allResults, questions);

				// Opção de exportar resultados
				Console.Write("\n\nDeseja exportar os resultados para um arquivo? (S/N): ");
				if (Console.ReadLine()?.ToUpper() == "S")
				{
					ExportResultsToFile(allResults, questions);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\n❌ Erro ao processar arquivo: {ex.Message}");
			}

			Console.WriteLine("\n\nPressione qualquer tecla para voltar ao menu...");
			Console.ReadKey();
		}

		private static void CreateSampleCsvFile(List<QuestionData> questions)
		{
			Console.Write("\nDigite o caminho onde deseja salvar o arquivo de exemplo\n(ex: C:\\Users\\..\\respostas_exemplo.csv)\nOu pressione Enter para salvar na Área de Trabalho: ");
			var filePath = Console.ReadLine()?.Trim().Trim('"');

			if (string.IsNullOrEmpty(filePath))
			{
				filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "respostas_exemplo.csv");
				Console.WriteLine($"\nUsando caminho padrão: {filePath}");
			}

			try
			{
				CsvQuestionReader.CreateSampleCsvFile(filePath, questions);
				Console.WriteLine($"\n✓ Arquivo criado com sucesso em: {filePath}");
				Console.WriteLine("\nO arquivo contém:");
				Console.WriteLine("- Linha 1: Cabeçalhos (Nome do Estudante | IDs das questões)");
				Console.WriteLine("- Linhas seguintes: Dados de 3 estudantes de exemplo");
				Console.WriteLine("\nEdite este arquivo e depois use a opção 2 para analisar!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\n❌ Erro ao criar arquivo: {ex.Message}");
			}

			Console.WriteLine("\nPressione qualquer tecla para voltar...");
			Console.ReadKey();
		}

		private static List<StudentAnalysisResult> AnalyzeAllStudents(
			List<StudentAnswerSet> studentAnswerSets, 
			List<QuestionData> questions)
		{
			var results = new List<StudentAnalysisResult>();

			foreach (var studentSet in studentAnswerSets)
			{
				var studentResult = new StudentAnalysisResult
				{
					StudentName = studentSet.StudentName,
					QuestionResults = new Dictionary<string, AnswerAnalysisResult>()
				};

				foreach (var question in questions)
				{
					if (studentSet.Answers.TryGetValue(question.Id, out var answer))
					{
						var criteria = CreateCriteriaFromQuestion(question);
						var analysisResult = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);
						studentResult.QuestionResults[question.Id] = analysisResult;
					}
				}

				results.Add(studentResult);
			}

			return results;
		}

		private static void DisplayBulkAnalysisResults(
			List<StudentAnalysisResult> results, 
			List<QuestionData> questions)
		{
			Console.WriteLine($"\n{new string('=', 100)}");
			Console.WriteLine("RESULTADOS DA ANÁLISE EM LOTE");
			Console.WriteLine($"{new string('=', 100)}\n");

			// Resumo por estudante
			Console.WriteLine("📊 RESUMO POR ESTUDANTE:\n");
			Console.WriteLine($"{"Estudante",-25} {"Questões Respondidas",-25} {"Média Geral",-15} {"Status"}");
			Console.WriteLine(new string('-', 100));

			foreach (var result in results.OrderByDescending(r => r.AverageScore))
			{
				var status = result.AverageScore >= 70 ? "✓ Aprovado" : "✗ Necessita Revisão";
				Console.WriteLine($"{result.StudentName,-25} {result.QuestionResults.Count,-25} {result.AverageScore:F2}/100{"",-7} {status}");
			}

			// Análise detalhada por estudante
			Console.WriteLine($"\n\n{new string('=', 100)}");
			Console.WriteLine("ANÁLISE DETALHADA POR ESTUDANTE");
			Console.WriteLine($"{new string('=', 100)}");

			foreach (var studentResult in results)
			{
				Console.WriteLine($"\n\n👤 ESTUDANTE: {studentResult.StudentName}");
				Console.WriteLine($"   Média Geral: {studentResult.AverageScore:F2}/100");
				Console.WriteLine($"   Total de questões: {studentResult.QuestionResults.Count}");
				Console.WriteLine($"\n   {"ID",-5} {"Pergunta",-50} {"Score",-12} {"Status"}");
				Console.WriteLine($"   {new string('-', 90)}");

				foreach (var question in questions)
				{
					if (studentResult.QuestionResults.TryGetValue(question.Id, out var result))
					{
						var status = result.FinalScore >= 70 ? "✓" : "✗";
						var truncatedQuestion = question.Question.Length > 47 
							? question.Question.Substring(0, 47) + "..." 
							: question.Question;
						Console.WriteLine($"   {question.Id,-5} {truncatedQuestion,-50} {result.FinalScore:F2}/100{"",-2} {status}");
					}
					else
					{
						var truncatedQuestion = question.Question.Length > 47 
							? question.Question.Substring(0, 47) + "..." 
							: question.Question;
						Console.WriteLine($"   {question.Id,-5} {truncatedQuestion,-50} {"N/A",-12} -");
					}
				}

				// Pontos fracos
				var weakPoints = studentResult.QuestionResults
					.Where(r => r.Value.FinalScore < 70)
					.OrderBy(r => r.Value.FinalScore)
					.Take(3)
					.ToList();

				if (weakPoints.Count > 0)
				{
					Console.WriteLine($"\n   ⚠ Pontos de atenção:");
					foreach (var weak in weakPoints)
					{
						var question = questions.First(q => q.Id == weak.Key);
						Console.WriteLine($"      - Q{weak.Key}: {question.Question} ({weak.Value.FinalScore:F2}/100)");
						
						if (weak.Value.MissingRequiredKeywords.Count > 0)
						{
							Console.WriteLine($"        Faltam palavras-chave: {string.Join(", ", weak.Value.MissingRequiredKeywords)}");
						}
						if (weak.Value.MissingRequiredPhrases.Count > 0)
						{
							Console.WriteLine($"        Faltam frases: {string.Join(", ", weak.Value.MissingRequiredPhrases)}");
						}
					}
				}
			}

			// Estatísticas gerais
			Console.WriteLine($"\n\n{new string('=', 100)}");
			Console.WriteLine("📈 ESTATÍSTICAS GERAIS");
			Console.WriteLine($"{new string('=', 100)}\n");

			var overallAverage = results.Average(r => r.AverageScore);
			var bestStudent = results.OrderByDescending(r => r.AverageScore).FirstOrDefault();
			var approvalRate = (results.Count(r => r.AverageScore >= 70) * 100.0) / results.Count;

			Console.WriteLine($"Média geral da turma: {overallAverage:F2}/100");
			Console.WriteLine($"Melhor desempenho: {bestStudent?.StudentName} ({bestStudent?.AverageScore:F2}/100)");
			Console.WriteLine($"Taxa de aprovação (≥70): {approvalRate:F1}%");
			Console.WriteLine($"Total de estudantes: {results.Count}");

			// Análise por questão
			Console.WriteLine($"\n\n📋 DESEMPENHO POR QUESTÃO:\n");
			Console.WriteLine($"{"ID",-5} {"Área",-12} {"Média",-12} {"Aprovados",-15} {"Taxa"}");
			Console.WriteLine(new string('-', 60));

			foreach (var question in questions)
			{
				var questionResults = results
					.Where(r => r.QuestionResults.ContainsKey(question.Id))
					.Select(r => r.QuestionResults[question.Id])
					.ToList();

				if (questionResults.Count > 0)
				{
					var avg = questionResults.Average(r => r.FinalScore);
					var approved = questionResults.Count(r => r.FinalScore >= 70);
					var rate = (approved * 100.0) / questionResults.Count;

					Console.WriteLine($"{question.Id,-5} {question.Area,-12} {avg:F2}/100{"",-2} {approved}/{questionResults.Count,-12} {rate:F1}%");
				}
			}
		}

		private static void ExportResultsToFile(
			List<StudentAnalysisResult> results, 
			List<QuestionData> questions)
		{
			var fileName = $"analise_resultado_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
			var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

			try
			{
				using (var writer = new StreamWriter(filePath))
				{
					writer.WriteLine("=== RELATÓRIO DE ANÁLISE DE RESPOSTAS ===");
					writer.WriteLine($"Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
					writer.WriteLine($"Total de estudantes: {results.Count}");
					writer.WriteLine($"Total de questões: {questions.Count}\n");

					foreach (var studentResult in results)
					{
						writer.WriteLine($"\nEstudante: {studentResult.StudentName}");
						writer.WriteLine($"Média: {studentResult.AverageScore:F2}/100");
						writer.WriteLine("Questões:");

						foreach (var question in questions)
						{
							if (studentResult.QuestionResults.TryGetValue(question.Id, out var result))
							{
								writer.WriteLine($"  Q{question.Id}: {result.FinalScore:F2}/100");
							}
						}
						writer.WriteLine(new string('-', 50));
					}
				}

				Console.WriteLine($"\n✓ Resultados exportados para: {filePath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\n❌ Erro ao exportar: {ex.Message}");
			}
		}

		public static AnswerCriteria CreateCriteriaFromQuestion(QuestionData question)
		{
			var requiredKeywords = question.RequiredKeywords
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(k => k.Trim())
				.ToList();

			var requiredPhrases = question.RequiredPhrases
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim())
				.ToList();

			var optionalKeywords = question.OptionalKeywords
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(k => k.Trim())
				.ToList();

			return new AnswerCriteria
			{
				RequiredKeywords = requiredKeywords,
				RequiredPhrases = requiredPhrases,
				OptionalKeywords = optionalKeywords,
				RequiredKeywordsWeight = 0.4,
				RequiredPhrasesWeight = 0.4,
				OptionalKeywordsWeight = 0.2
			};
		}

		public static List<QuestionData> GetQuestions()
		{
			return new List<QuestionData>
			{
				new QuestionData
				{
					Id = "01",
					Area = "Progr.",
					Question = "O que é uma classe em POO?",
					RequiredKeywords = "modelo, objetos, atributos",
					RequiredPhrases = "define atributos e métodos",
					OptionalKeywords = "blueprint, instância",
					StandardAnswer = "Uma classe é um modelo que define atributos e métodos para criar objetos."
				},
				new QuestionData
				{
					Id = "02",
					Area = "Progr.",
					Question = "Explique o conceito de Herança.",
					RequiredKeywords = "derivada, herdar, base",
					RequiredPhrases = "herda comportamentos, classe base",
					OptionalKeywords = "superclasse, subclasse",
					StandardAnswer = "Herança permite que uma classe derivada possa herdar comportamentos de uma classe base."
				},
				new QuestionData
				{
					Id = "03",
					Area = "Eng. Soft.",
					Question = "O que caracteriza o Acoplamento?",
					RequiredKeywords = "dependência, módulos, software",
					RequiredPhrases = "nível de dependência, componentes de software",
					OptionalKeywords = "interdependência, baixo acoplamento",
					StandardAnswer = "O acoplamento mede o nível de dependência entre diferentes módulos ou componentes de software."
				},
				new QuestionData
				{
					Id = "04",
					Area = "Redes",
					Question = "Qual a função do protocolo HTTP?",
					RequiredKeywords = "protocolo, cliente, servidor",
					RequiredPhrases = "transferência de dados, navegador e servidor",
					OptionalKeywords = "hipertexto, stateless",
					StandardAnswer = "É um protocolo que permite a transferência de dados entre um cliente (navegador) e um servidor."
				},
				new QuestionData
				{
					Id = "05",
					Area = "B. Dados",
					Question = "O que é uma Chave Primária?",
					RequiredKeywords = "identificador, registro, tabela",
					RequiredPhrases = "identificador único, registro em uma tabela",
					OptionalKeywords = "PK, chave estrangeira",
					StandardAnswer = "É um identificador único que garante que cada registro em uma tabela seja exclusivo."
				},
				new QuestionData
				{
					Id = "06",
					Area = "Hardware",
					Question = "Qual o papel da Memória RAM?",
					RequiredKeywords = "volátil, processador, dados",
					RequiredPhrases = "acesso rápido, execução imediata",
					OptionalKeywords = "randômico, latência",
					StandardAnswer = "É uma memória volátil de acesso rápido para dados de execução imediata pelo processador."
				},
				new QuestionData
				{
					Id = "07",
					Area = "Eng. Soft.",
					Question = "Defina Coesão em um código.",
					RequiredKeywords = "classe, método, propósito",
					RequiredPhrases = "única responsabilidade, intimamente relacionadas",
					OptionalKeywords = "modularidade, SOLID",
					StandardAnswer = "Coesão indica se as funções de uma classe estão intimamente relacionadas a um único propósito."
				},
				new QuestionData
				{
					Id = "08",
					Area = "Progr.",
					Question = "O que é Polimorfismo?",
					RequiredKeywords = "objeto, formas, métodos",
					RequiredPhrases = "múltiplas implementações, mesmo método",
					OptionalKeywords = "sobrescrita, sobrecarga",
					StandardAnswer = "Capacidade de um objeto assumir várias formas, permitindo múltiplas implementações de um mesmo método."
				},
				new QuestionData
				{
					Id = "09",
					Area = "Progr.",
					Question = "O que é o Encapsulamento?",
					RequiredKeywords = "dados, acesso, métodos",
					RequiredPhrases = "modificadores de acesso, esconder detalhes",
					OptionalKeywords = "private, getters, setters",
					StandardAnswer = "Técnica de proteger dados usando modificadores de acesso, para esconder detalhes internos do objeto."
				},
				new QuestionData
				{
					Id = "10",
					Area = "Progr.",
					Question = "Explique a Recursividade.",
					RequiredKeywords = "chamada, função, parada",
					RequiredPhrases = "chama a si mesmo, condição de parada",
					OptionalKeywords = "caso base, stack",
					StandardAnswer = "Ocorre quando uma função chama a si mesmo, exigindo uma condição de parada para terminar."
				}
			};
		}
	}

	/// <summary>
	/// Representa os dados de uma questão
	/// </summary>
	public class QuestionData
	{
		public string Id { get; set; } = string.Empty;
		public string Area { get; set; } = string.Empty;
		public string Question { get; set; } = string.Empty;
		public string RequiredKeywords { get; set; } = string.Empty;
		public string RequiredPhrases { get; set; } = string.Empty;
		public string OptionalKeywords { get; set; } = string.Empty;
		public string StandardAnswer { get; set; } = string.Empty;
	}

	/// <summary>
	/// Resultado da análise de um estudante
	/// </summary>
	public class StudentAnalysisResult
	{
		public string StudentName { get; set; } = string.Empty;
		public Dictionary<string, AnswerAnalysisResult> QuestionResults { get; set; } = new Dictionary<string, AnswerAnalysisResult>();
		
		public double AverageScore => QuestionResults.Count > 0 
			? QuestionResults.Values.Average(r => r.FinalScore) 
			: 0;
	}
}
