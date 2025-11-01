using System;
using System.IO;
using System.Text.Json;

namespace PRACTICODE_2_NAAMA_MENDELSON
{
    internal sealed class HtmlHelper
    {
        private static readonly Lazy<HtmlHelper> _instance = new Lazy<HtmlHelper>(() => new HtmlHelper());
        public static HtmlHelper Instance => _instance.Value;

        public string[] Tags { get; private set; }
        public string[] HtmlVoidTags { get; private set; }

        private HtmlHelper()
        {
            Tags = LoadJsonArray("JSON Files\\HtmlTags.json");
            HtmlVoidTags = LoadJsonArray("JSON Files\\HtmlVoidTags.json");
        }

        private string[] LoadJsonArray(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"[Warning] File not found: {fullPath}");
                    return Array.Empty<string>();
                }

                string json = File.ReadAllText(fullPath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine($"[Warning] File empty: {fullPath}");
                    return Array.Empty<string>();
                }

                var result = JsonSerializer.Deserialize<string[]>(json);
                return result ?? Array.Empty<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to load JSON {relativePath}: {ex.Message}");
                return Array.Empty<string>();
            }
        }
    }
}
