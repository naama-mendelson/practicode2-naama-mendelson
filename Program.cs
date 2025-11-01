using PRACTICODE_2_NAAMA_MENDELSON;
using System.Linq;
string html = await HtmlElement.LoadHtmlFromUrl("https://forum.netfree.link:20907");
HtmlElement root = HtmlElement.BuildFromHtml(html);
string tagToFind = "li";
string classToFind = "nav-item";

var matchingElements = root.Descendants()
                           .Where(el => el.Name == tagToFind &&
                                        el.Classes.Contains(classToFind))
                           .ToList();
Console.WriteLine($"Found {matchingElements.Count} elements matching <{tagToFind} class='{classToFind}'>:");
foreach (var el in matchingElements)
    Console.WriteLine(el);
Console.WriteLine("\nDone!");