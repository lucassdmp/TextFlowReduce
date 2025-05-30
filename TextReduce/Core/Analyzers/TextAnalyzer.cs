using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextFlowReduce.Core.Mappers;

namespace TextFlowReduce.Core.Analyzers
{
    public class TextAnalyzer
    {
        public static async Task<TextAnalysisResult> AnalyzeTextFile(
            string filePath,
            Func<string, int>[] wordAnalyzers,
            Func<string, int>[] phraseAnalyzers,
            Func<string, int>[] paragraphAnalyzers)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified text file was not found.", filePath);
            }

            string textContent = await File.ReadAllTextAsync(filePath);

            return AnalyzeText(textContent, wordAnalyzers, phraseAnalyzers, paragraphAnalyzers);
        }

        public static TextAnalysisResult AnalyzeText(
            string textContent,
            Func<string, int>[] wordAnalyzers,
            Func<string, int>[] phraseAnalyzers,
            Func<string, int>[] paragraphAnalyzers)
        {
            var wordTask = Task.Run(() => WordMapper.Map(textContent));
            var phraseTask = Task.Run(() => PhraseMapper.Map(textContent));
            var paragraphTask = Task.Run(() => ParagraphMapper.Map(textContent));

            Task.WaitAll(wordTask, phraseTask, paragraphTask);

            var words = wordTask.Result;
            var phrases = phraseTask.Result;
            var paragraphs = paragraphTask.Result;

            var wordAnalysisTask = Task.Run(() => AnalyzeContent(words.Keys, wordAnalyzers));
            var phraseAnalysisTask = Task.Run(() => AnalyzeContent(phrases, phraseAnalyzers));
            var paragraphAnalysisTask = Task.Run(() => AnalyzeContent(paragraphs, paragraphAnalyzers));

            Task.WaitAll(wordAnalysisTask, phraseAnalysisTask, paragraphAnalysisTask);

            var wordScores = wordAnalysisTask.Result;
            var phraseScores = phraseAnalysisTask.Result;
            var paragraphScores = paragraphAnalysisTask.Result;

            double wordScoreAvg = wordScores.Any() ? wordScores.Average() : 0;
            double phraseScoreAvg = phraseScores.Any() ? phraseScores.Average() : 0;
            double paragraphScoreAvg = paragraphScores.Any() ? paragraphScores.Average() : 0;

            double finalScore = (wordScoreAvg + phraseScoreAvg + paragraphScoreAvg) / 3;

            return new TextAnalysisResult
            {
                WordScore = wordScoreAvg,
                PhraseScore = phraseScoreAvg,
                ParagraphScore = paragraphScoreAvg,
                FinalScore = finalScore,
                WordCount = words.Sum(kv => kv.Value),
                PhraseCount = phrases.Count,
                ParagraphCount = paragraphs.Count
            };
        }

        private static List<double> AnalyzeContent(IEnumerable<string> contentItems, Func<string, int>[] analyzers)
        {
            if (analyzers == null || analyzers.Length == 0)
            {
                return new List<double>();
            }

            var scores = new ConcurrentBag<double>();

            Parallel.ForEach(contentItems, item =>
            {
                if (string.IsNullOrWhiteSpace(item))
                    return;

                foreach (var analyzer in analyzers)
                {
                    try
                    {
                        int score = analyzer(item);
                        if (score >= 0 && score <= 100)
                        {
                            scores.Add(score);
                        }
                    }
                    catch
                    {
                    }
                }
            });

            return scores.ToList();
        }
    }

    public class TextAnalysisResult
    {
        public double WordScore { get; set; }
        public double PhraseScore { get; set; }
        public double ParagraphScore { get; set; }
        public double FinalScore { get; set; }
        public int WordCount { get; set; }
        public int PhraseCount { get; set; }
        public int ParagraphCount { get; set; }

        public override string ToString()
        {
            return $"Final Score: {FinalScore:0.00} (Words: {WordScore:0.00}, Phrases: {PhraseScore:0.00}, Paragraphs: {ParagraphScore:0.00})";
        }
    }
}