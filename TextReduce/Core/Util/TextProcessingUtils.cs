namespace TextFlowReduce.Core.Utils
{
    public static class TextProcessingUtils
    {
        public static List<ReadOnlyMemory<char>> SplitIntoChunks(string text, int approximateChunkSize, char[] separators)
        {
            var chunks = new List<ReadOnlyMemory<char>>();
            if (string.IsNullOrEmpty(text)) return chunks;

            int start = 0;
            while (start < text.Length)
            {
                int end = Math.Min(start + approximateChunkSize, text.Length);

                if (end < text.Length)
                {
                    while (end > start && !separators.Contains(text[end]))
                    {
                        end--;
                    }

                    if (end == start)
                    {
                        end = Math.Min(start + approximateChunkSize, text.Length);
                    }
                }

                chunks.Add(text.AsMemory(start, end - start));
                start = end;

                while (start < text.Length && separators.Contains(text[start]))
                {
                    start++;
                }
            }

            return chunks;
        }

        public static List<ReadOnlyMemory<char>> SplitIntoChunks(string text, int approximateChunkSize, char separator)
        {
            var chunks = new List<ReadOnlyMemory<char>>();
            if (string.IsNullOrEmpty(text)) return chunks;

            int start = 0;
            while (start < text.Length)
            {
                int end = Math.Min(start + approximateChunkSize, text.Length);

                if (end < text.Length)
                {
                    while (end > start && text[end] != separator)
                    {
                        end--;
                    }

                    if (end == start)
                    {
                        end = Math.Min(start + approximateChunkSize, text.Length);
                    }
                }

                chunks.Add(text.AsMemory(start, end - start));
                start = end;

                if (start < text.Length && text[start] == separator)
                {
                    start++;
                }
            }

            return chunks;
        }

        public static bool IsValidSentenceEnding(ReadOnlySpan<char> text, int index)
        {
            if (index > 0 && char.IsDigit(text[index - 1]))
            {
                return false;
            }

            if (index > 0 && char.IsLetter(text[index - 1]) && index < text.Length - 1 && char.IsLetter(text[index + 1]))
            {
                return false;
            }

            if (index < text.Length - 1 && !char.IsWhiteSpace(text[index + 1]))
            {
                return false;
            }

            return true;
        }
    }
}