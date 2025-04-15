using TextFlowReduce.Core.Models.Enum;
using TextFlowReduce.Utilities;

namespace TextFlowReduce.Core.Mappers
{
    public class TextMapperAsync
    {
        public static async Task<Dictionary<TextType, IEnumerable<string>>> SplitTextAsync(string text)
        {
            var tasks = new Dictionary<TextType, Task<IEnumerable<string>>>
            {
                [TextType.Word] = Task.Run(() => TextFlowReduceUtil.SplitIntoWords(text)),
                [TextType.Sentence] = Task.Run(() => TextFlowReduceUtil.SplitIntoSentences(text)),
                [TextType.Paragraph] = Task.Run(() => TextFlowReduceUtil.SplitIntoParagraphs(text))
            };

            await Task.WhenAll(tasks.Values);

            return tasks.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Result);
        }
    }
}
