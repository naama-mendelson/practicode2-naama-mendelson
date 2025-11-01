using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PRACTICODE_2_NAAMA_MENDELSON
{
    internal class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }

        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public Selector(string tagName = "", string id = "", List<string> classes = null)
        {
            TagName = tagName;
            Id = id;
            Classes = classes ?? new List<string>();
        }

        public void SetChild(Selector child)
        {
            Child = child;
            child.Parent = this;
        }

        // ממיר מחרוזת של סלקטור לאובייקט Selector
        public static Selector FromQuery(string query)
        {
            var validTags = new HashSet<string>(HtmlHelper.Instance.Tags, StringComparer.OrdinalIgnoreCase);

            var levels = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Selector root = new Selector();
            Selector current = root;

            foreach (var level in levels)
            {
                string tagName = "";
                string id = "";
                List<string> classes = new List<string>();

                var parts = Regex.Split(level, @"(?=[#.])");
                foreach (var part in parts)
                {
                    if (string.IsNullOrEmpty(part)) continue;

                    if (part.StartsWith("#"))
                        id = part.Substring(1);
                    else if (part.StartsWith("."))
                        classes.Add(part.Substring(1));
                    else if (validTags.Contains(part.ToLower()))
                        tagName = part; // שם תג חוקי
                }

                Selector newSelector = new Selector(tagName, id, classes);
                current.SetChild(newSelector);
                current = newSelector;
            }

            return root.Child; // מחזיר את הסלקטור הראשון עם מידע ממשי
        }
    }
}
