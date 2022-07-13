using HtmlAgilityPack;

namespace WebCrawler
{
    internal class Program
    {
        
        internal Dictionary<string, int> word_dict  = new Dictionary<string, int>();
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
            scrape_wiki_console(crawler, end_node);
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

        static string scrape_wiki_console(HtmlNode crawler, HtmlNode end_node)
        {
            Random rnd = new Random();
            int num = rnd.Next(100);
            string path = @"web-crawler-log/log.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                while (crawler != end_node)
                {
                    writer.WriteLine("Element name: " + crawler.Name);
                    foreach (HtmlNode crawler_elem in crawler.DescendantsAndSelf())
                    {
                        if (crawler_elem.NodeType != HtmlNodeType.Text)
                        {
                            continue;
                        }
                        writer.WriteLine("  NodeType: " + crawler_elem.NodeType);
                        writer.WriteLine("  Child Element: " + crawler_elem.Name);
                        writer.WriteLine("      InnerHtml: " + crawler_elem.InnerHtml);
                        writer.WriteLine("      InnerText: " + crawler_elem.InnerText);
                    }
                    if(crawler.NextSibling == null) {
                        break;
                    }
                    crawler = crawler.NextSibling;
                    writer.WriteLine("");
                }
            }
            return "";
        }

        static string scrape_wiki(HtmlNode crawler, HtmlNode end_node)
        {
            while (crawler != end_node)
            {
                foreach (HtmlNode crawler_elem in crawler.DescendantsAndSelf())
                {
                    if (crawler_elem.NodeType != HtmlNodeType.Text)
                    {
                        continue;
                    }
                    string s = crawler_elem.InnerText;
                    add_to_dictionary();
                }
            }
            return "";
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