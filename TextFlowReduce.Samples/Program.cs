using TextFlowReduce.Core.Analyzers;

public class Program
{
    public static async Task Main(string[] args)
    {
        Func<string, int>[] wordAnalyzers = new Func<string, int>[]
        {
            text => text.Length > 5 ? 80 : 20,
            text => text.All(char.IsLetter) ? 90 : 50
        };

        Func<string, int>[] phraseAnalyzers = new Func<string, int>[]
        {
            text => text.Contains('?') ? 70 : 30,
            text => text.Length > 50 ? 60 : 40
        };

        int ParagraphWordCountAnalyzer(string paragraph, int minWordCount)
        {
            int wordCount = paragraph.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            return wordCount >= minWordCount ? 100 : (wordCount * 100 / minWordCount);
        }


        Func<string, int>[] paragraphAnalyzers = new Func<string, int>[]
        {
            text => text.Split('.').Length > 3 ? 75 : 45,
            text => ParagraphWordCountAnalyzer(text, 15)
        };

        var result = await TextAnalyzer.AnalyzeTextFile(
            @"C:/Content/text.txt",
            wordAnalyzers,
            phraseAnalyzers,
            paragraphAnalyzers);

        Console.WriteLine(result);
    }
}