using TextFlowReduce.Core.Models.Entity.Interfaces;
using TextFlowReduce.Core.Models.Enum;

namespace TextFlowReduce.Core.Reducers
{
    public class SentenceComplexity : ITextAnalysisFunction
    {
        public TextType TargetLevel => TextType.Sentence;

        public float Analyze(string sentence)
        {
            var wordCount = sentence.Split(' ').Length;
            return Math.Min(wordCount * 5f, 100f);
        }
    }
}
