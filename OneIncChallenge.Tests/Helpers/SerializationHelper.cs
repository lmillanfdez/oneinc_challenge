using Newtonsoft.Json;

namespace OneIncChallenge.Tests.Helpers;

public class SerializationHelper
{
    public static string SerializeResult(object? result)
    {
        var serializingSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return JsonConvert.SerializeObject(result, serializingSettings);
    }
}