using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PRACTICODE_2_NAAMA_MENDELSON
{
    internal class HtmlElement
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public List<string> Attributes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        public HtmlElement(string name = "", string id = "")
        {
            Name = name;
            Id = id;
        }

        public void AddChild(HtmlElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        // 🧩 פונקציה שבונה עץ HTML ממחרוזת HTML אמיתית
        public static HtmlElement BuildFromHtml(string html)
        {
            var root = new HtmlElement("root");
            var stack = new Stack<HtmlElement>();
            stack.Push(root);

            var tagRegex = new Regex(@"<(?<close>/)?(?<tag>[a-zA-Z0-9]+)(?<attrs>[^>]*)>");
            var matches = tagRegex.Matches(html);

            foreach (Match match in matches)
            {
                bool isClosing = match.Groups["close"].Success;
                string tag = match.Groups["tag"].Value.ToLower();
                string attrs = match.Groups["attrs"].Value;

                if (isClosing)
                {
                    if (stack.Count > 1)
                        stack.Pop();
                }
                else
                {
                    var element = new HtmlElement(tag);
                    ParseAttributes(attrs, element);
                    stack.Peek().AddChild(element);

                    // תגיות סגירה עצמית לא נכנסות לערימה
                    if (!attrs.TrimEnd().EndsWith("/"))
                    {
                        stack.Push(element);
                    }
                }
            }

            return root;
        }

        // ⚙️ עוזרת לפענח Id, Class ו־Attributes
        private static void ParseAttributes(string attrs, HtmlElement elem)
        {
            var attrRegex = new Regex(@"([a-zA-Z\-]+)=""([^""]*)""");
            foreach (Match match in attrRegex.Matches(attrs))
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                elem.Attributes.Add($"{key}={value}");

                if (key == "id") elem.Id = value;
                if (key == "class") elem.Classes.AddRange(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }

        // 📄 קריאת קובץ HTML מקומי
        public static async Task<string> LoadHtmlFromFile(string path)
        {
            return await Task.Run(() => System.IO.File.ReadAllText(path));
        }

        // 🌐 קריאת HTML מאתר
        public static async Task<string> LoadHtmlFromUrl(string url)
        {
            using HttpClient client = new HttpClient();
            return await client.GetStringAsync(url);
        }

        // 🔍 פונקציה שמחזירה את כל הצאצאים (כולל הנוכחי)
        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }
        }

        public override string ToString()
        {
            string cls = Classes.Count > 0 ? $" class='{string.Join(" ", Classes)}'" : "";
            string id = string.IsNullOrEmpty(Id) ? "" : $" id='{Id}'";
            return $"<{Name}{id}{cls}> (children: {Children.Count})";
        }
    }
}
