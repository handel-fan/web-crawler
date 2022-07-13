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
            html_doc.LoadHtml(wikipedia_html);
            string s = html_doc.ParsedText;
            var crawler = html_doc.DocumentNode.SelectSingleNode(start_xpath);
            var end_node = html_doc.DocumentNode.SelectSingleNode(end_xpath);
            scrape_wiki_console(crawler, end_node);
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

        static string scrape_wiki_console(HtmlNode crawler, HtmlNode end_node) {
            while(crawler != end_node) {
                Console.WriteLine("Element name: " + crawler.Name);
                foreach (HtmlNode crawler_elem in crawler.DescendantsAndSelf()) {
                    if(crawler_elem.NodeType != HtmlNodeType.Text) {
                        continue;
                    }
                    Console.WriteLine("  NodeType : " + crawler_elem.NodeType);
                    Console.WriteLine("  Child Element : " + crawler_elem.Name);
                    Console.WriteLine("      InnerHtml :" + crawler_elem.InnerHtml);
                    Console.WriteLine("      InnerText :" + crawler_elem.InnerText);
                }
                crawler = crawler.NextSibling;
                Console.WriteLine("");
            }
            return "";
        }

        static string scrape_wiki(HtmlNode crawler, HtmlNode end_node) {
            while(crawler != end_node) {
                foreach (HtmlNode crawler_elem in crawler.DescendantsAndSelf()) {
                    if(crawler_elem.NodeType != HtmlNodeType.Text) {
                        continue;
                    }
                    string s = crawler_elem.InnerText;
                    add_to_dictionary();
                }
            }
            return "";
        }

        static void add_to_dictionary() {

        }
    }
}