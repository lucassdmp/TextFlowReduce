using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextFlowReduce.Samples
{
	/// <summary>
	/// Leitor de arquivos CSV para análise de respostas
	/// </summary>
	public class CsvQuestionReader
	{
		/// <summary>
		/// Lê as respostas de estudantes de um arquivo CSV
		/// </summary>
		/// <param name="filePath">Caminho do arquivo CSV</param>
		/// <returns>Lista de respostas por estudante e questão</returns>
		public static List<StudentAnswerSet> ReadStudentAnswersFromCsv(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");
			}

			var studentAnswers = new List<StudentAnswerSet>();
			var lines = File.ReadAllLines(filePath);

			if (lines.Length < 2)
			{
				throw new InvalidOperationException("O arquivo CSV deve conter pelo menos uma linha de cabeçalho e uma linha de dados.");
			}

			// Ler cabeçalhos (primeira linha) - IDs das questões
			var headers = ParseCsvLine(lines[0]);
			var questionIds = headers.Skip(1).ToList(); // Pular "Nome do Estudante"

			// Ler dados dos estudantes (linhas 2 em diante)
			for (int i = 1; i < lines.Length; i++)
			{
				var values = ParseCsvLine(lines[i]);
				
				if (values.Count == 0 || string.IsNullOrWhiteSpace(values[0]))
				{
					continue; // Pular linhas vazias
				}

				var answerSet = new StudentAnswerSet
				{
					StudentName = values[0].Trim(),
					Answers = new Dictionary<string, string>()
				};

				// Ler respostas para cada questão
				for (int j = 0; j < questionIds.Count && j + 1 < values.Count; j++)
				{
					var answer = values[j + 1].Trim();
					
					if (!string.IsNullOrEmpty(answer))
					{
						answerSet.Answers[questionIds[j]] = answer;
					}
				}

				if (answerSet.Answers.Count > 0)
				{
					studentAnswers.Add(answerSet);
				}
			}

			return studentAnswers;
		}

		/// <summary>
		/// Cria um arquivo CSV de exemplo para demonstração
		/// </summary>
		public static void CreateSampleCsvFile(string filePath, List<QuestionData> questions)
		{
			using (var writer = new StreamWriter(filePath))
			{
				// Cabeçalho
				var header = "Nome do Estudante," + string.Join(",", questions.Select(q => q.Id));
				writer.WriteLine(header);

				// Dados de exemplo
				var sampleData = new[]
				{
					new { Name = "João Silva", Answers = new[] 
					{
						"Uma classe é um modelo que define atributos e métodos para criar objetos.",
						"Herança é quando uma classe derivada recebe características da classe base.",
						"Acoplamento mede a dependência entre módulos de software.",
						"HTTP é um protocolo para transferência de dados entre cliente e servidor.",
						"Chave primária é um identificador único para registros em uma tabela.",
						"RAM é memória volátil de acesso rápido para o processador.",
						"Coesão indica se funções de uma classe têm propósito único.",
						"Polimorfismo permite que objetos assumam várias formas.",
						"Encapsulamento protege dados usando modificadores de acesso.",
						"Recursividade é quando função chama a si mesmo com condição de parada."
					}},
					new { Name = "Maria Santos", Answers = new[] 
					{
						"Classe é um blueprint para criar objetos com atributos.",
						"Herança permite reutilizar código de uma classe base.",
						"É sobre dependência entre componentes.",
						"Protocolo para comunicação web.",
						"Um identificador único em tabelas.",
						"Memória rápida e volátil.",
						"Funções relacionadas em uma classe.",
						"Múltiplas implementações do mesmo método.",
						"Esconder detalhes internos do objeto.",
						"Função que chama ela mesma."
					}},
					new { Name = "Pedro Costa", Answers = new[] 
					{
						"Modelo que define atributos e métodos.",
						"Classe derivada herda comportamentos da base.",
						"Nível de dependência entre módulos.",
						"Permite transferência de dados entre navegador e servidor.",
						"Identificador único que garante exclusividade de registro em uma tabela.",
						"Memória volátil para dados de execução imediata pelo processador.",
						"Funções intimamente relacionadas a um único propósito.",
						"Objeto com múltiplas formas permitindo várias implementações.",
						"Técnica usando modificadores de acesso para esconder detalhes.",
						"Função chama a si mesmo exigindo condição de parada."
					}}
				};

				foreach (var student in sampleData)
				{
					var line = EscapeCsvValue(student.Name) + "," + 
						string.Join(",", student.Answers.Select(a => EscapeCsvValue(a)));
					writer.WriteLine(line);
				}
			}
		}

		/// <summary>
		/// Faz parse de uma linha CSV considerando aspas
		/// </summary>
		private static List<string> ParseCsvLine(string line)
		{
			var values = new List<string>();
			var currentValue = "";
			var insideQuotes = false;

			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];

				if (c == '"')
				{
					if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
					{
						// Aspas duplas dentro de aspas
						currentValue += '"';
						i++;
					}
					else
					{
						insideQuotes = !insideQuotes;
					}
				}
				else if (c == ',' && !insideQuotes)
				{
					values.Add(currentValue);
					currentValue = "";
				}
				else
				{
					currentValue += c;
				}
			}

			values.Add(currentValue);
			return values;
		}

		/// <summary>
		/// Escapa valores CSV com aspas quando necessário
		/// </summary>
		private static string EscapeCsvValue(string value)
		{
			if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
			{
				return "\"" + value.Replace("\"", "\"\"") + "\"";
			}
			return value;
		}
	}

	/// <summary>
	/// Representa o conjunto de respostas de um estudante
	/// </summary>
	public class StudentAnswerSet
	{
		public string StudentName { get; set; } = string.Empty;
		public Dictionary<string, string> Answers { get; set; } = new Dictionary<string, string>();
	}
}
