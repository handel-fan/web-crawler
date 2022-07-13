using HtmlAgilityPack;

namespace WebCrawler
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            var start_xpath = @"//*[@id='History']/parent::h2";
            var end_xpath = @"//*[@id='cite_ref-:0_155-1']";

            //consider making below global
            var html_doc = new HtmlDocument();
            var wikipedia_html = get_wikipedia_html();
            html_doc.LoadHtml(wikipedia_html);

            var crawler = html_doc.DocumentNode.SelectSingleNode(start_xpath);
            var end_node = html_doc.DocumentNode.SelectSingleNode(end_xpath);
            scrape_wiki(crawler, end_node);
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
                    Console.WriteLine("  NodeType : " + crawler_elem.NodeType);
                    Console.WriteLine("  Child Element : " + crawler_elem.Name);
                    Console.WriteLine("      InnerHtml :" + crawler_elem.InnerHtml);
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

                }
            }
        }
    }
}