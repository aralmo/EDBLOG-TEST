using FluentAssertions;
using System.Text.Json;
using System.Text.Json.Serialization;

public class POC
{

    //eventtype:serializer:data
    //newpost:{ "title" : "post title" }
    //editpost:{ "title" : "new post title" }

    [Fact]
    public void EventSerializer()
    {
        var data = new NewPostEvent()
        {
            Title = "some post title"
        };

        SerializeEvent(data).Should().Be("{\"title\":\"some post title\"}");
    }

    [Fact]
    public void DeSerializeEvent()
    {
        var newpost = JsonSerializer.Deserialize<NewPostEvent>("{\"title\":\"some post title\"}", eventSerializationOptions);
    }

    [Fact]
    public void AggregateStream()
    {
        IEnumerable<string> stream = new[]
        {
            "newpost:{\"title\":\"some post title\"}",
            "editpost:{\"title\":\"better post title\"}",
            "editpost:{\"title\":\"post title\", \"content\":\"some content\"}",
            "editpost:{\"content\":\"final content\"}"
        };

        
    }



    class NewPostEvent : IState
    {
        public required string Title { get; set; }
        public string? Content { get; set; }
    }

    class EditPostEvent : IState
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
    }

    interface IState
    {
        string? Title { get; }
        string? Content { get; }
    }

    static string SerializeEvent<T>(T data)
        => JsonSerializer.Serialize(data, eventSerializationOptions);

    static JsonSerializerOptions eventSerializationOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };


}