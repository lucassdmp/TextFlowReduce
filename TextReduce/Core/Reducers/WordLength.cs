using TextFlowReduce.Core.Models.Entity.Interfaces;
using TextFlowReduce.Core.Models.Enum;

namespace TextFlowReduce.Core.Reducers
{
    public class WordLength : ITextAnalysisFunction
    {
        public TextType TargetLevel => TextType.Word;

        public float Analyze(string word)
        {
            return Math.Min(word.Length * 10f, 100f);
        }
    }
}
