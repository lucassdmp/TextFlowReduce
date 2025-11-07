using TextFlowReduce.Core.Analyzers;
using TextFlowReduce.Core.Models;

namespace TextFlowReduce.Tests.Unit
{
	[TestFixture]
	public class AnswerAnalyzerTests
	{
		[Test]
		public void AnalyzeAnswer_WithAllRequiredKeywords_ReturnsHighScore()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "classe", "objetos" }, // Usando plural que aparece no texto
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer = "Uma classe é um modelo para criar objetos.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

			// Assert
			Assert.That(result.FinalScore, Is.EqualTo(100.0));
			Assert.That(result.FoundRequiredKeywords.Count, Is.EqualTo(2));
			Assert.That(result.MissingRequiredKeywords.Count, Is.EqualTo(0));
		}

		[Test]
		public void AnalyzeAnswer_WithMissingRequiredKeywords_ReturnsLowerScore()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "classe", "objetos", "heranca", "polimorfismo" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer = "Uma classe é um modelo para criar objetos.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

			// Assert
			Assert.That(result.RequiredKeywordsScore, Is.EqualTo(50.0));
			Assert.That(result.FoundRequiredKeywords.Count, Is.EqualTo(2));
			Assert.That(result.MissingRequiredKeywords.Count, Is.EqualTo(2));
			Assert.That(result.MissingRequiredKeywords, Contains.Item("heranca"));
			Assert.That(result.MissingRequiredKeywords, Contains.Item("polimorfismo"));
		}

		[Test]
		public void AnalyzeAnswer_WithRequiredPhrases_DetectsCorrectly()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new(),
				RequiredPhrases = new() { "energia luminosa", "dioxido de carbono" },
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 0.0,
				RequiredPhrasesWeight = 1.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer = "A fotossintese converte energia luminosa usando dioxido de carbono.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

			// Assert
			Assert.That(result.RequiredPhrasesScore, Is.EqualTo(100.0));
			Assert.That(result.FoundRequiredPhrases.Count, Is.EqualTo(2));
			Assert.That(result.MissingRequiredPhrases.Count, Is.EqualTo(0));
		}

		[Test]
		public void AnalyzeAnswer_CaseInsensitive_WorksCorrectly()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "Classe", "OBJETO" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer = "uma classe cria um objeto";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

			// Assert
			Assert.That(result.RequiredKeywordsScore, Is.EqualTo(100.0));
		}

		[Test]
		public void AnalyzeAnswer_WithAccents_NormalizesCorrectly()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "fotossintese", "oxigenio" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer = "A fotossintese produz oxigenio.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

			// Assert
			Assert.That(result.RequiredKeywordsScore, Is.EqualTo(100.0));
		}

		[Test]
		public void AnalyzeAnswer_WithOptionalKeywords_AddsBonus()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "classe" },
				RequiredPhrases = new(),
				OptionalKeywords = new() { "metodos", "propriedades", "interface" },
				RequiredKeywordsWeight = 0.7,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.3
			};

			string answer = "Uma classe tem metodos e propriedades. Pode implementar uma interface.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

			// Assert
			Assert.That(result.OptionalKeywordsScore, Is.EqualTo(100.0));
			Assert.That(result.FoundOptionalKeywords.Count, Is.EqualTo(3));
			Assert.That(result.FinalScore, Is.EqualTo(100.0));
		}

		[Test]
		public void AnalyzeAnswer_WithEmptyAnswer_ThrowsException()
		{
			// Arrange
			var criteria = new AnswerCriteria();

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
		  AnswerAnalyzer.AnalyzeAnswer("", criteria));
		}

		[Test]
		public void AnalyzeAnswer_WithNullCriteria_ThrowsException()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
			AnswerAnalyzer.AnalyzeAnswer("test", null));
		}

		[Test]
		public void AnalyzeAnswer_WithInvalidWeights_ThrowsException()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywordsWeight = 0.5,
				RequiredPhrasesWeight = 0.3,
				OptionalKeywordsWeight = 0.3
			};

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() =>
		   AnswerAnalyzer.AnalyzeAnswer("test", criteria));
		}

		[Test]
		public void AnalyzeAnswer_WordBoundaries_DetectsWholeWordsOnly()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "sol" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer = "O sistema está isolado do ambiente.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

			// Assert
			Assert.That(result.RequiredKeywordsScore, Is.EqualTo(0.0));
			Assert.That(result.MissingRequiredKeywords, Contains.Item("sol"));
		}

		[Test]
		public void AnalyzeAnswer_CountsWordsAndSentences_Correctly()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "teste" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer = "Este é um teste. Contém duas sentenças!";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);

			// Assert
			Assert.That(result.TotalWords, Is.EqualTo(7));
			Assert.That(result.TotalSentences, Is.EqualTo(2));
		}

		[Test]
		public void AnalysisResult_ToString_ReturnsFormattedString()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "teste" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer = "Este é um teste.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(answer, criteria);
			string output = result.ToString();

			// Assert
			Assert.That(output, Does.Contain("Score Final"));
			Assert.That(output, Does.Contain("100"));
		}
	}

	[TestFixture]
	public class AnswerCriteriaTests
	{
		[Test]
		public void ValidateWeights_WithValidWeights_DoesNotThrow()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywordsWeight = 0.5,
				RequiredPhrasesWeight = 0.3,
				OptionalKeywordsWeight = 0.2
			};

			// Act & Assert
			Assert.DoesNotThrow(() => criteria.ValidateWeights());
		}

		[Test]
		public void ValidateWeights_WithInvalidWeights_ThrowsException()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywordsWeight = 0.5,
				RequiredPhrasesWeight = 0.5,
				OptionalKeywordsWeight = 0.5
			};

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() => criteria.ValidateWeights());
		}
	}
}