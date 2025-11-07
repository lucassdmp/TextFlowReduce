namespace TextFlowReduce.Core.Models
{
	/// <summary>
	/// Resultado da análise de uma resposta
	/// </summary>
	public class AnswerAnalysisResult
	{
		/// <summary>
		/// Score final da resposta (0-100)
		/// </summary>
		public double FinalScore { get; set; }

		/// <summary>
		/// Score das palavras-chave obrigatórias (0-100)
		/// </summary>
		public double RequiredKeywordsScore { get; set; }

		/// <summary>
		/// Score das frases obrigatórias (0-100)
		/// </summary>
		public double RequiredPhrasesScore { get; set; }

		/// <summary>
		/// Score das palavras-chave opcionais (0-100)
		/// </summary>
		public double OptionalKeywordsScore { get; set; }

		/// <summary>
		/// Palavras-chave obrigatórias encontradas
		/// </summary>
		public List<string> FoundRequiredKeywords { get; set; } = new List<string>();

		/// <summary>
		/// Palavras-chave obrigatórias não encontradas
		/// </summary>
		public List<string> MissingRequiredKeywords { get; set; } = new List<string>();

		/// <summary>
		/// Frases obrigatórias encontradas
		/// </summary>
		public List<string> FoundRequiredPhrases { get; set; } = new List<string>();

		/// <summary>
		/// Frases obrigatórias não encontradas
		/// </summary>
		public List<string> MissingRequiredPhrases { get; set; } = new List<string>();

		/// <summary>
		/// Palavras-chave opcionais encontradas
		/// </summary>
		public List<string> FoundOptionalKeywords { get; set; } = new List<string>();

		/// <summary>
		/// Total de palavras na resposta
		/// </summary>
		public int TotalWords { get; set; }

		/// <summary>
		/// Total de sentenças na resposta
		/// </summary>
		public int TotalSentences { get; set; }

		/// <summary>
		/// Retorna uma descrição resumida do resultado
		/// </summary>
		public override string ToString()
		{
			return $@"Score Final: {FinalScore:F2}/100
					- Palavras-chave obrigatórias: {RequiredKeywordsScore:F2}/100 ({FoundRequiredKeywords.Count} de {FoundRequiredKeywords.Count + MissingRequiredKeywords.Count} encontradas)
					- Frases obrigatórias: {RequiredPhrasesScore:F2}/100 ({FoundRequiredPhrases.Count} de {FoundRequiredPhrases.Count + MissingRequiredPhrases.Count} encontradas)
					- Palavras-chave opcionais: {OptionalKeywordsScore:F2}/100 ({FoundOptionalKeywords.Count} encontradas)
					- Total de palavras: {TotalWords}
					- Total de sentenças: {TotalSentences}";
		}

		/// <summary>
		/// Retorna um relatório detalhado da análise
		/// </summary>
		public string GetDetailedReport()
		{
			var report = ToString();

			if (MissingRequiredKeywords.Count > 0)
			{
				report += $"\n\nPalavras-chave obrigatórias ausentes:\n  - {string.Join("\n  - ", MissingRequiredKeywords)}";
			}

			if (MissingRequiredPhrases.Count > 0)
			{
				report += $"\n\nFrases obrigatórias ausentes:\n  - {string.Join("\n  - ", MissingRequiredPhrases)}";
			}

			if (FoundOptionalKeywords.Count > 0)
			{
				report += $"\n\nPalavras-chave opcionais encontradas:\n  - {string.Join("\n  - ", FoundOptionalKeywords)}";
			}

			return report;
		}
	}
}
