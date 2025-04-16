using TextFlowReduce.Core.Mappers;

var text = "This is a sample text. It contains several words, phrases, and paragraphs.\n This is the second sentence. And this is the third one!";
var dict = WordMapper.Map(text);
var phrases = PhraseMapper.Map(text);
var paragraph = ParagraphMapper.Map(text);

foreach (var kvp in dict)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}

Console.WriteLine("\nPhrases Count:" + phrases.Count);
Console.WriteLine(paragraph.Count);