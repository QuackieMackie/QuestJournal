using System.IO;
using System.Reflection;

namespace QuestJournal.Utils;

public class EmbeddedResourceLoader
{
    public static string LoadJson(string resourcePath, string? namespacePrefix = null)
    {
        var assembly = Assembly.GetExecutingAssembly();

        if (!string.IsNullOrWhiteSpace(namespacePrefix))
        {
            resourcePath = $"{namespacePrefix}.{resourcePath}";
        }

        var fullResourcePath = $"{assembly.GetName().Name}.Data.{resourcePath.Replace("\\", ".").Replace("/", ".")}.json";

        using Stream? resourceStream = assembly.GetManifestResourceStream(fullResourcePath);
        if (resourceStream == null)
        {
            throw new FileNotFoundException($"Embedded resource not found: {fullResourcePath}");
        }

        using var reader = new StreamReader(resourceStream);
        return reader.ReadToEnd();
    }
}
