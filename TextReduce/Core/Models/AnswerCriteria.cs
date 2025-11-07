namespace TextFlowReduce.Core.Models
{
	/// <summary>
	/// Define os critérios que uma resposta deve atender
	/// </summary>
	public class AnswerCriteria
	{
		/// <summary>
		/// Palavras-chave que devem aparecer na resposta (case-insensitive)
		/// </summary>
		public List<string> RequiredKeywords { get; set; } = new List<string>();

		/// <summary>
		/// Frases que devem aparecer na resposta (case-insensitive)
		/// </summary>
		public List<string> RequiredPhrases { get; set; } = new List<string>();

		/// <summary>
		/// Palavras-chave opcionais que aumentam a pontuação se presentes
		/// </summary>
		public List<string> OptionalKeywords { get; set; } = new List<string>();

		/// <summary>
		/// Peso das palavras-chave obrigatórias no score final (0-1)
		/// </summary>
		public double RequiredKeywordsWeight { get; set; } = 0.4;

		/// <summary>
		/// Peso das frases obrigatórias no score final (0-1)
		/// </summary>
		public double RequiredPhrasesWeight { get; set; } = 0.4;

		/// <summary>
		/// Peso das palavras-chave opcionais no score final (0-1)
		/// </summary>
		public double OptionalKeywordsWeight { get; set; } = 0.2;

		/// <summary>
		/// Valida se os pesos somam 1.0
		/// </summary>
		public void ValidateWeights()
		{
			double sum = RequiredKeywordsWeight + RequiredPhrasesWeight + OptionalKeywordsWeight;
			if (Math.Abs(sum - 1.0) > 0.001)
			{
				throw new InvalidOperationException($"A soma dos pesos deve ser 1.0. Soma atual: {sum:F3}");
			}
		}
	}
}
