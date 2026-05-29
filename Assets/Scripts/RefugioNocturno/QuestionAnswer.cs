using System;

public enum ResponseTag
{
    Unknown,
    Coherent,
    Evasive,
    Contradictory,
    Dangerous
}

[Serializable]
public class QuestionAnswer
{
    public string question;
    public string answer;
    public ResponseTag responseTag = ResponseTag.Unknown;
}
