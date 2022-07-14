using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;

namespace WebCrawler
{
    internal class Program
    {
        
        internal Dictionary<string, int> word_dict  = new Dictionary<string, int>();
        internal static string[] skippable_types = {"style", "link", "sup"};
        static void Main(string[] args)
        {
            var start_xpath = @"//*[@id='History']/parent::h2";
            var end_xpath = @"//*[@id='cite_ref-:0_155-1']";

            //consider making below global
            var html_doc = new HtmlDocument();
            var wikipedia_html = get_wikipedia_html();
            if (wikipedia_html == null ) {
                return;
            }
            html_doc.LoadHtml(wikipedia_html);
            var crawler = html_doc.DocumentNode.SelectSingleNode(start_xpath);
            var end_node = html_doc.DocumentNode.SelectSingleNode(end_xpath);
            scrape_runner(crawler, end_node);
        }

        static void debug_test() {
            var html_doc = new HtmlDocument();
            var wikipedia_html = get_wikipedia_html();
            html_doc.LoadHtml(wikipedia_html);
            var body_xpath = @"//body";
            var body= html_doc.DocumentNode.SelectSingleNode(body_xpath);
            var body_tags = body.Descendants("*");
            //var count = body_tags.
            //for (var i = 0; i < html_doc.Count<body_tags>; )
        }

        static string? get_wikipedia_html() {
            string url = "https://en.wikipedia.org/wiki/Microsoft";
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        return content.ReadAsStringAsync().Result;
                    }
                }
            }
        }

        static string scrape_runner(HtmlNode start_node, HtmlNode end_node)
        {
            string path = @"web-crawler-log/log.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                while (start_node != end_node)
                {
                    while (skippable_types.Contains(start_node.Name))
                    {
                        start_node = start_node.NextSibling;
                    }

                    Console.WriteLine("Element name: " + start_node.Name);
                    writer.WriteLine("Element name: " + start_node.Name);

                    scrape_nodes(start_node.DescendantsAndSelf().ToArray());

                    if (start_node.NextSibling == null)
                    {
                        break;
                    }
                    start_node = start_node.NextSibling;

                    writer.WriteLine("");
                    Console.WriteLine("");
                }
            }
            return "";
        }

        static void scrape_nodes(HtmlNode[] descendants)
        {
            string path = @"web-crawler-log/log.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < descendants.Length; i++)
                {
                    if (skippable_types.Contains(descendants[i].Name))
                    {
                        if (descendants[i].HasChildNodes)
                        {
                            foreach (HtmlNode skippable_type_descendant in descendants[i].Descendants())
                            {
                                i++;
                            }
                            break;
                        }
                        continue;
                    }
                    if (skip_useless_valid_descendant(descendants[i]))
                    {
                        continue;
                    }
                    string[] arr = format_scraped_string(descendants[i].InnerText);
                    Console.WriteLine("      InnerHtml: " + HttpUtility.HtmlDecode(descendants[i].InnerText));
                    writer.WriteLine("      InnerHtml: " + HttpUtility.HtmlDecode(descendants[i].InnerText));
                }
            }
        }

        static bool skip_useless_valid_descendant(HtmlNode node) {
            return node.NodeType != HtmlNodeType.Text || node.InnerHtml == null || node.InnerHtml == "";
        }

        static string[] format_scraped_string(string s) {
            return Regex.Matches(s, @"\b[\w']+\b")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();
        }

        static void add_to_dictionary()
        {

        }

        static string StripHTML(string htmlStr)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlStr);
            var root = doc.DocumentNode;
            string s = "";
            foreach (var node in root.DescendantsAndSelf())
            {
                if (!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                        s += text.Trim() + " ";
                }
            }
            return s.Trim();
        }
    }
}