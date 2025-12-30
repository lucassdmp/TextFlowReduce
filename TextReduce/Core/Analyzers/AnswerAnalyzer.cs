using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextFlowReduce.Core.Models;

namespace TextFlowReduce.Core.Analyzers
{
	/// <summary>
	/// Analisador de respostas baseado em critérios definidos
	/// </summary>
	public class AnswerAnalyzer
	{
		/// <summary>
		/// Analisa uma resposta com base nos critérios fornecidos
		/// </summary>
		/// <param name="answer">A resposta a ser analisada</param>
		/// <param name="criteria">Os critérios que a resposta deve atender</param>
		/// <returns>Resultado da análise com scores e detalhes</returns>
		public static AnswerAnalysisResult AnalyzeAnswer(string answer, AnswerCriteria criteria)
		{
			if (string.IsNullOrWhiteSpace(answer))
			{
				throw new ArgumentException("A resposta não pode ser vazia ou nula.", nameof(answer));
			}

			if (criteria == null)
			{
				throw new ArgumentNullException(nameof(criteria));
			}

			criteria.ValidateWeights();

			var result = new AnswerAnalysisResult();

			string normalizedAnswer = NormalizeText(answer);

			result.TotalWords = CountWords(answer);
			result.TotalSentences = CountSentences(answer);

			AnalyzeRequiredKeywords(normalizedAnswer, criteria.RequiredKeywords, result);

			AnalyzeRequiredPhrases(normalizedAnswer, criteria.RequiredPhrases, result);

			AnalyzeOptionalKeywords(normalizedAnswer, criteria.OptionalKeywords, result);

			result.FinalScore = CalculateFinalScore(result, criteria);

			return result;
		}

		/// <summary>
		/// Normaliza o texto para análise (lowercase, remove acentos)
		/// </summary>
		private static string NormalizeText(string text)
		{
			string normalized = text.ToLowerInvariant();

			normalized = RemoveAccents(normalized);

			return normalized;
		}

		/// <summary>
		/// Remove acentos de uma string
		/// </summary>
		private static string RemoveAccents(string text)
		{
			var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
			var stringBuilder = new System.Text.StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
		}

		/// <summary>
		/// Conta o número de palavras no texto
		/// </summary>
		private static int CountWords(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return 0;

			return Regex.Matches(text, @"\b\w+\b").Count;
		}

		/// <summary>
		/// Conta o número de sentenças no texto
		/// </summary>
		private static int CountSentences(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return 0;

			return Regex.Matches(text, @"[.!?]+").Count;
		}

		/// <summary>
		/// Analisa as palavras-chave obrigatórias
		/// </summary>
		private static void AnalyzeRequiredKeywords(
		string normalizedAnswer,
		List<string> requiredKeywords,
		AnswerAnalysisResult result)
		{
			if (requiredKeywords == null || requiredKeywords.Count == 0)
			{
				result.RequiredKeywordsScore = 100;
				return;
			}

			var foundKeywords = new ConcurrentBag<string>();
			var missingKeywords = new ConcurrentBag<string>();

			Parallel.ForEach(requiredKeywords, keyword =>
			{
				string normalizedKeyword = NormalizeText(keyword);

				if (Regex.IsMatch(normalizedAnswer, $@"\b{Regex.Escape(normalizedKeyword)}\b"))
				{
					foundKeywords.Add(keyword);
				}
				else
				{
					missingKeywords.Add(keyword);
				}
			});

			result.FoundRequiredKeywords = foundKeywords.ToList();
			result.MissingRequiredKeywords = missingKeywords.ToList();

			result.RequiredKeywordsScore = requiredKeywords.Count > 0
		   ? (result.FoundRequiredKeywords.Count * 100.0 / requiredKeywords.Count)
	   : 100;
		}

		/// <summary>
		/// Analisa as frases obrigatórias
		/// </summary>
		private static void AnalyzeRequiredPhrases(
	 string normalizedAnswer,
	List<string> requiredPhrases,
		   AnswerAnalysisResult result)
		{
			if (requiredPhrases == null || requiredPhrases.Count == 0)
			{
				result.RequiredPhrasesScore = 100;
				return;
			}

			var foundPhrases = new ConcurrentBag<string>();
			var missingPhrases = new ConcurrentBag<string>();

			Parallel.ForEach(requiredPhrases, phrase =>
			{
				string normalizedPhrase = NormalizeText(phrase);

				if (normalizedAnswer.Contains(normalizedPhrase))
				{
					foundPhrases.Add(phrase);
				}
				else
				{
					missingPhrases.Add(phrase);
				}
			});

			result.FoundRequiredPhrases = foundPhrases.ToList();
			result.MissingRequiredPhrases = missingPhrases.ToList();

			result.RequiredPhrasesScore = requiredPhrases.Count > 0
	  ? (result.FoundRequiredPhrases.Count * 100.0 / requiredPhrases.Count)
	  : 100;
		}

		/// <summary>
		/// Analisa as palavras-chave opcionais
		/// </summary>
		private static void AnalyzeOptionalKeywords(
		  string normalizedAnswer,
	List<string> optionalKeywords,
 AnswerAnalysisResult result)
		{
			if (optionalKeywords == null || optionalKeywords.Count == 0)
			{
				result.OptionalKeywordsScore = 0;
				return;
			}

			var foundOptionalKeywords = new ConcurrentBag<string>();

			Parallel.ForEach(optionalKeywords, keyword =>
			{
				string normalizedKeyword = NormalizeText(keyword);

				if (Regex.IsMatch(normalizedAnswer, $@"\b{Regex.Escape(normalizedKeyword)}\b"))
				{
					foundOptionalKeywords.Add(keyword);
				}
			});

			result.FoundOptionalKeywords = foundOptionalKeywords.ToList();

			result.OptionalKeywordsScore = optionalKeywords.Count > 0
	? (result.FoundOptionalKeywords.Count * 100.0 / optionalKeywords.Count)
		: 0;
		}


		/// <summary>
		/// Calcula o score final baseado nos pesos
		/// </summary>
		private static double CalculateFinalScore(AnswerAnalysisResult result, AnswerCriteria criteria)
		{
			double finalScore =
				   (result.RequiredKeywordsScore * criteria.RequiredKeywordsWeight) +
			  (result.RequiredPhrasesScore * criteria.RequiredPhrasesWeight) +
		 (result.OptionalKeywordsScore * criteria.OptionalKeywordsWeight);

			return Math.Round(finalScore, 2);
		}
	}
}
