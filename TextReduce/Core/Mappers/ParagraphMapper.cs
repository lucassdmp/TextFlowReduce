using System.Collections.Concurrent;
using TextFlowReduce.Core.Utils;

namespace TextFlowReduce.Core.Mappers
{
    public class ParagraphMapper
    {
        public static ConcurrentBag<string> Map(string textContent)
        {
            var paragraphs = new ConcurrentBag<string>();

            if (string.IsNullOrWhiteSpace(textContent))
                return paragraphs;

            if (textContent.Length > 10000)
            {
                var chunks = TextProcessingUtils.SplitIntoChunks(textContent, 10000, '\n');

                Parallel.ForEach(chunks, chunk =>
                {
                    var localParagraphs = new List<string>();
                    ProcessChunk(chunk.Span, localParagraphs);

                    foreach (var paragraph in localParagraphs)
                    {
                        if (!string.IsNullOrWhiteSpace(paragraph))
                        {
                            paragraphs.Add(paragraph);
                        }
                    }
                });
            }
            else
            {
                ProcessTextSequential(textContent.AsSpan(), paragraphs);
            }

            return paragraphs;
        }

        private static void ProcessChunk(ReadOnlySpan<char> chunk, List<string> localParagraphs)
        {
            int start = 0;
            for (int i = 0; i < chunk.Length; i++)
            {
                if (chunk[i] == '\n')
                {
                    var paragraphSpan = chunk.Slice(start, i - start).Trim();
                    if (!paragraphSpan.IsEmpty)
                    {
                        localParagraphs.Add(new string(paragraphSpan));
                    }
                    start = i + 1;
                }
            }

            if (start < chunk.Length)
            {
                var remaining = chunk.Slice(start).Trim();
                if (!remaining.IsEmpty)
                {
                    localParagraphs.Add(new string(remaining));
                }
            }
        }

        private static void ProcessTextSequential(ReadOnlySpan<char> textSpan, ConcurrentBag<string> paragraphs)
        {
            int start = 0;
            for (int i = 0; i < textSpan.Length; i++)
            {
                if (textSpan[i] == '\n')
                {
                    var paragraphSpan = textSpan.Slice(start, i - start).Trim();
                    if (!paragraphSpan.IsEmpty)
                    {
                        paragraphs.Add(new string(paragraphSpan));
                    }
                    start = i + 1;
                }
            }

            if (start < textSpan.Length)
            {
                var remaining = textSpan.Slice(start).Trim();
                if (!remaining.IsEmpty)
                {
                    paragraphs.Add(new string(remaining));
                }
            }
        }
    }
}