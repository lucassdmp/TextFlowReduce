using TextFlowReduce.Core.Models.Enum;

namespace TextFlowReduce.Core.Models.Entity.Interfaces
{
    public interface ITextAnalysisFunction
    {
        TextType TargetLevel { get; }
        float Analyze(string text);
    }
}
