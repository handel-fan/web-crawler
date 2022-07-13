using HtmlAgilityPack;

namespace WebCrawler
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            var wiki_history_xpath = 
            "//div[@id=mw-content-text]/" + 
            "following-sibling::sup[@id=cite_ref-:0_155-1]" +
            "preceding-sibling::*";
            Console.SetOut(TextWriter.Null);
            var html_doc = new HtmlDocument();
            var wikipedia_html = get_wikipedia_html();
            html_doc.LoadHtml(wikipedia_html);
            var history_section_html = html_doc.DocumentNode.SelectNodes(wiki_history_xpath);
            
            if (wikipedia_html is string wiki_html_string)
            {
                var html_doc1 = new HtmlDocument();
                html_doc.Load(wiki_html_string);
                var history_section_html1 = html_doc.DocumentNode.SelectNodes(wiki_history_xpath);
                string s = scrape_wiki(html_doc);
            }
        }

        static string? get_wikipedia_html()
        {
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

        static string scrape_wiki(HtmlDocument html_doc) {
            return "";
        }
    }
}