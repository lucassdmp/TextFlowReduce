using System.Collections.Concurrent;
using TextFlowReduce.Core.Utils;

namespace TextFlowReduce.Core.Mappers
{
    public class WordMapper
    {
        private static readonly char[] _separators = new[]
        {
            ' ', '\n', '\r', '\t', '.', ',', '!', '?', ';', ':',
            '-', '_', '(', ')', '[', ']', '{', '}', '"', '\''
        };

        public static ConcurrentDictionary<string, int> Map(string textContent)
        {
            var wordCounts = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(textContent))
                return wordCounts;

            if (textContent.Length > 10000)
            {
                var chunks = TextProcessingUtils.SplitIntoChunks(textContent, 10000, _separators);

                Parallel.ForEach(chunks, chunk =>
                {
                    var localCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    ProcessChunk(chunk.Span, localCounts);

                    foreach (var pair in localCounts)
                    {
                        wordCounts.AddOrUpdate(pair.Key, pair.Value, (_, count) => count + pair.Value);
                    }
                });
            }
            else
            {
                ProcessTextSequential(textContent.AsSpan(), wordCounts);
            }

            return wordCounts;
        }

        private static void ProcessChunk(ReadOnlySpan<char> chunk, Dictionary<string, int> localCounts)
        {
            int start = 0;
            for (int i = 0; i < chunk.Length; i++)
            {
                if (Array.IndexOf(_separators, chunk[i]) >= 0)
                {
                    var wordSpan = chunk.Slice(start, i - start).Trim();
                    if (!wordSpan.IsEmpty)
                    {
                        string word = new string(wordSpan);
                        localCounts[word] = localCounts.TryGetValue(word, out var count) ? count + 1 : 1;
                    }
                    start = i + 1;
                }
            }

            if (start < chunk.Length)
            {
                var remaining = chunk.Slice(start).Trim();
                if (!remaining.IsEmpty)
                {
                    string word = new string(remaining);
                    localCounts[word] = localCounts.TryGetValue(word, out var count) ? count + 1 : 1;
                }
            }
        }

        private static void ProcessTextSequential(ReadOnlySpan<char> textSpan, ConcurrentDictionary<string, int> wordCounts)
        {
            int start = 0;
            for (int i = 0; i < textSpan.Length; i++)
            {
                if (Array.IndexOf(_separators, textSpan[i]) >= 0)
                {
                    var wordSpan = textSpan.Slice(start, i - start).Trim();
                    if (!wordSpan.IsEmpty)
                    {
                        string word = new string(wordSpan);
                        wordCounts.AddOrUpdate(word, 1, (_, count) => count + 1);
                    }
                    start = i + 1;
                }
            }

            if (start < textSpan.Length)
            {
                var remaining = textSpan.Slice(start).Trim();
                if (!remaining.IsEmpty)
                {
                    wordCounts.AddOrUpdate(new string(remaining), 1, (_, count) => count + 1);
                }
            }
        }
    }
}