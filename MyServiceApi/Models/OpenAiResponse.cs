using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ChatCompletion
{
    public string? Id { get; set; }
    public string? Object { get; set; }
    public long Created { get; set; }
    public string? Model { get; set; }
    public Choice[]? Choices { get; set; }
    public Usage? Usage { get; set; }
    public string? SystemFingerprint { get; set; }
}

public class Choice
{
    public int Index { get; set; }
    public Message? Message { get; set; }
    public object? Logprobs { get; set; }
    public string? FinishReason { get; set; }
}

public class Message
{
    public string? Role { get; set; }
    public string? Content { get; set; }
}

public class Usage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}
