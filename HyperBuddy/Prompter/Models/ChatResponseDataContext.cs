using System.Text.Json.Serialization;

namespace HyperBuddy.Prompter.Models;

public class Citation
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("id")]
    public object Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("filepath")]
    public string Filepath { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }

    [JsonPropertyName("chunk_id")]
    public string ChunkId { get; set; }
}

public class Metadata
{
    [JsonPropertyName("chunking")]
    public string Chunking { get; set; }
}
public class ChatResponseDataContext
{
    [JsonPropertyName("citations")]
    public List<Citation> Citations { get; set; }

    [JsonPropertyName("intent")]
    public string Intent { get; set; }
}

