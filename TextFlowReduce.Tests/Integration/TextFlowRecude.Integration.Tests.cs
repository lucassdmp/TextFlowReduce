using TextFlowReduce.Core.Analyzers;
using TextFlowReduce.Core.Models;

namespace TextFlowReduce.Tests.Integration
{
	[TestFixture]
	public class AnswerAnalyzerIntegrationTests
	{
		[Test]
		public void CompleteScenario_BiologyQuestion_AnalyzesCorrectly()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "luz", "clorofila", "oxigenio", "glicose" },
				RequiredPhrases = new() { "energia luminosa", "dioxido de carbono" },
				OptionalKeywords = new() { "cloroplastos", "fotossistema", "ATP" },
				RequiredKeywordsWeight = 0.4,
				RequiredPhrasesWeight = 0.4,
				OptionalKeywordsWeight = 0.2
			};

			string goodAnswer = @"A fotossintese é o processo pelo qual as plantas convertem 
 energia luminosa em energia quimica. Durante este processo, a clorofila nas folhas 
     absorbe luz e utiliza dioxido de carbono do ar e agua do solo para produzir glicose 
   e liberar oxigenio. Este processo ocorre nos cloroplastos.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(goodAnswer, criteria);

			// Assert
			Assert.That(result.FinalScore, Is.GreaterThan(80.0));
			Assert.That(result.FoundRequiredKeywords.Count, Is.EqualTo(4));
			Assert.That(result.FoundRequiredPhrases.Count, Is.EqualTo(2));
			Assert.That(result.FoundOptionalKeywords.Count, Is.GreaterThan(0));
		}

		[Test]
		public void CompleteScenario_ProgrammingQuestion_DetectsIncompleteAnswer()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "classes", "objetos", "heranca", "polimorfismo" },
				RequiredPhrases = new() { "encapsulamento de dados" },
				OptionalKeywords = new() { "abstracao", "interface" },
				RequiredKeywordsWeight = 0.5,
				RequiredPhrasesWeight = 0.3,
				OptionalKeywordsWeight = 0.2
			};

			string incompleteAnswer = "POO usa classes e objetos. Heranca é importante.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(incompleteAnswer, criteria);

			// Assert
			Assert.That(result.FinalScore, Is.LessThan(60.0));
			Assert.That(result.MissingRequiredKeywords, Contains.Item("polimorfismo"));
			Assert.That(result.MissingRequiredPhrases, Contains.Item("encapsulamento de dados"));
		}

		[Test]
		public void CompleteScenario_MultipleAnswers_ComparesScores()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "agua", "ciclo", "evaporacao", "precipitacao" },
				RequiredPhrases = new() { "mudanca de estado" },
				OptionalKeywords = new() { "condensacao", "infiltracao" },
				RequiredKeywordsWeight = 0.5,
				RequiredPhrasesWeight = 0.3,
				OptionalKeywordsWeight = 0.2
			};

			string answer1 = "O ciclo da agua envolve evaporacao e precipitacao.";
			string answer2 = @"O ciclo da agua é um processo continuo onde a agua passa por
     mudanca de estado atraves de evaporacao, condensacao e precipitacao. 
    A agua evapora, forma nuvens e retorna como chuva.";

			// Act
			var result1 = AnswerAnalyzer.AnalyzeAnswer(answer1, criteria);
			var result2 = AnswerAnalyzer.AnalyzeAnswer(answer2, criteria);

			// Assert
			Assert.That(result2.FinalScore, Is.GreaterThan(result1.FinalScore));
			Assert.That(result2.FoundRequiredKeywords.Count, Is.GreaterThanOrEqualTo(result1.FoundRequiredKeywords.Count));
		}

		[Test]
		public void RealWorldScenario_ChemistryQuestion_FullAnalysis()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "atomo", "protons", "eletrons", "neutrons" },
				RequiredPhrases = new() { "carga eletrica" },
				OptionalKeywords = new() { "nucleo", "orbitais" },
				RequiredKeywordsWeight = 0.4,
				RequiredPhrasesWeight = 0.4,
				OptionalKeywordsWeight = 0.2
			};

			string studentAnswer = @"O atomo é a menor unidade da materia. Ele é composto por
        protons com carga eletrica positiva, eletrons com carga eletrica negativa, e 
       neutrons sem carga. Os protons e neutrons ficam no nucleo, enquanto os eletrons 
   se movem em orbitais ao redor do nucleo.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(studentAnswer, criteria);
			string report = result.GetDetailedReport();

			// Assert
			Assert.That(result.FinalScore, Is.GreaterThan(70.0));
			Assert.That(result.FoundRequiredKeywords, Has.Count.EqualTo(4));
			Assert.That(result.FoundRequiredPhrases, Has.Count.EqualTo(1));
			Assert.That(result.FoundOptionalKeywords, Has.Count.GreaterThan(0));
			Assert.That(report, Does.Contain("Score Final"));
			Assert.That(result.TotalWords, Is.GreaterThan(30));
		}

		[Test]
		public void EdgeCase_VeryShortAnswer_StillAnalyzes()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "sim" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string shortAnswer = "Sim.";

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(shortAnswer, criteria);

			// Assert
			Assert.That(result.FinalScore, Is.EqualTo(100.0));
			Assert.That(result.TotalWords, Is.EqualTo(1));
			Assert.That(result.TotalSentences, Is.EqualTo(1));
		}

		[Test]
		public void EdgeCase_VeryLongAnswer_HandlesEfficiently()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "teste", "analise" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			var longAnswer = string.Join(" ", Enumerable.Repeat(
		  "Este é um teste de analise com muitas palavras para verificar performance.", 100));

			// Act
			var result = AnswerAnalyzer.AnalyzeAnswer(longAnswer, criteria);

			// Assert
			Assert.That(result.FinalScore, Is.EqualTo(100.0));
			Assert.That(result.TotalWords, Is.GreaterThan(1000));
		}

		[Test]
		public void Regression_AccentHandling_ConsistentBehavior()
		{
			// Arrange
			var criteria = new AnswerCriteria
			{
				RequiredKeywords = new() { "agua", "acida" },
				RequiredPhrases = new(),
				OptionalKeywords = new(),
				RequiredKeywordsWeight = 1.0,
				RequiredPhrasesWeight = 0.0,
				OptionalKeywordsWeight = 0.0
			};

			string answer1 = "A água pode ser ácida.";
			string answer2 = "A agua pode ser acida.";

			// Act
			var result1 = AnswerAnalyzer.AnalyzeAnswer(answer1, criteria);
			var result2 = AnswerAnalyzer.AnalyzeAnswer(answer2, criteria);

			// Assert
			Assert.That(result1.FinalScore, Is.EqualTo(result2.FinalScore));
			Assert.That(result1.FinalScore, Is.EqualTo(100.0));
			Assert.That(result1.FoundRequiredKeywords.Count, Is.EqualTo(result2.FoundRequiredKeywords.Count));
		}
	}
}