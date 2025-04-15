namespace TextFlowReduce.Utilities
{
    public class TextFlowReduceUtil
    {
        public static IEnumerable<string> SplitIntoWords(string text)
        {
            return text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
        public static IEnumerable<string> SplitIntoSentences(string text)
        {
            return text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => s.Trim());
        }
        public static IEnumerable<string> SplitIntoParagraphs(string text)
        {
            return text.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(p => p.Trim());
        }
    }
}
