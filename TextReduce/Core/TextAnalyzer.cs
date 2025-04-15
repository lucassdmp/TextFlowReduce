using TextFlowReduce.Core.Mappers;
using TextFlowReduce.Core.Models.Entity.Interfaces;
using TextFlowReduce.Core.Models.Enum;

namespace TextFlowReduce.Core
{
    public class TextAnalyzer
    {
        private readonly List<ITextAnalysisFunction> _functions;
        private readonly object _lock = new object();

        public TextAnalyzer(IEnumerable<ITextAnalysisFunction> functions)
        {
            _functions = functions.ToList();
        }

        public async Task<Dictionary<TextType, float>> AnalyzeAsync(string text)
        {
            var textFragments = await TextMapperAsync.SplitTextAsync(text);

            var results = new Dictionary<TextType, List<float>>();

            await Task.WhenAll(textFragments.Select(async levelFragments =>
            {
                var level = levelFragments.Key;
                var fragments = levelFragments.Value;

                var levelFunctions = _functions.Where(f => f.TargetLevel == level).ToList();
                if (!levelFunctions.Any()) return;

                var levelResults = new List<float>();

                await Task.Run(() =>
                {
                    Parallel.ForEach(fragments, fragment =>
                    {
                        foreach (var function in levelFunctions)
                        {
                            var result = function.Analyze(fragment);
                            lock (_lock)
                            {
                                levelResults.Add(result);
                            }
                        }
                    });
                });

                lock (_lock)
                {
                    results[level] = levelResults;
                }
            }));


            return results.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Any() ? kv.Value.Average() : 0);
        }
    }
}
