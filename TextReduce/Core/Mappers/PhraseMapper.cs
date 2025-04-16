using System.Collections.Concurrent;
using TextFlowReduce.Core.Utils;

namespace TextFlowReduce.Core.Mappers
{
    public class PhraseMapper
    {
        public static ConcurrentBag<string> Map(string textContent)
        {
            var phrases = new ConcurrentBag<string>();
            if (string.IsNullOrWhiteSpace(textContent))
            {
                return phrases;
            }

            char[] separators = { '.', '!', '?' };
            ProcessTextChunk(textContent.AsSpan(), separators, phrases);

            return phrases;
        }

        private static void ProcessTextChunk(ReadOnlySpan<char> textSpan, char[] separators, ConcurrentBag<string> phrases)
        {
            int start = 0;
            for (int i = 0; i < textSpan.Length; i++)
            {
                if (Array.IndexOf(separators, textSpan[i]) >= 0)
                {
                    if (TextProcessingUtils.IsValidSentenceEnding(textSpan, i))
                    {
                        var phraseSpan = textSpan.Slice(start, i - start + 1).Trim();
                        if (!phraseSpan.IsEmpty)
                        {
                            phrases.Add(phraseSpan.ToString());
                        }
                        start = i + 1;

                        while (start < textSpan.Length && char.IsWhiteSpace(textSpan[start]))
                        {
                            start++;
                        }
                    }
                }
            }

            if (start < textSpan.Length)
            {
                var remaining = textSpan.Slice(start).Trim();
                if (!remaining.IsEmpty)
                {
                    phrases.Add(remaining.ToString());
                }
            }
        }
    }
}