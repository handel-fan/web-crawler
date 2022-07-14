using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace WebCrawler
{
    internal class Program
    {
        private const string start_xpath = @"//span[@id='History']/parent::h2";
        private const string end_xpath = @"//span[@id='Corporate_affairs']/parent::h2/preceding::p[1]";
        private static string[] skippable_types = new string[]{"style", "link", "sup"};
        private static Dictionary<string, int> word_dict  = new Dictionary<string, int>();
        internal static Dictionary<string, int> Word_Dict {
            get{ return word_dict; }
            set{ word_dict = value; }
        }
        //Use the field below to specify how many words you would like to return!
        internal static int num_words = 1600;

        //Use the field below to specify any words you'd like to exclude from the search!
        internal static string[] excluded_words = {"the"};

        static void Main(string[] args)
        {
            if(num_words > 1600) {
                Console.WriteLine("Adjusting number of words to display to 1600.");
                num_words = 1600;
            }
            HtmlDocument? wiki_html_doc = get_load_html_doc();
            if (wiki_html_doc == null)
            {
                Console.WriteLine("Unable to access Wikipedia. Exiting.");
                return;
            }
            scrape_runner(wiki_html_doc);

            var sorted_descending_enumerable = 
            (from kvp in Word_Dict orderby kvp.Value descending select kvp).Take(num_words);
            foreach (KeyValuePair<string, int> kvp in sorted_descending_enumerable)
            {
                Console.WriteLine(kvp.Key + ": " + kvp.Value);
            }
        }

        static HtmlDocument? get_load_html_doc()
        {
            var html_doc = new HtmlDocument();
            try
            {
                var wikipedia_html = get_wikipedia_html();
                if (wikipedia_html == null)
                {
                    throw new Exception("Unknown error accessing wikipedia!");
                }
                html_doc.LoadHtml(wikipedia_html);
            }
            catch(Exception e) {
                Console.WriteLine("Unable to load HTML. Encountered the following exception:");
                Console.WriteLine(e.Message.ToString());
                return null;
            }
            return html_doc;
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

        static void scrape_runner(HtmlDocument wiki_html_doc)
        {
            var start_node = wiki_html_doc.DocumentNode.SelectSingleNode(start_xpath);
            var end_node = wiki_html_doc.DocumentNode.SelectSingleNode(end_xpath);
            while (start_node != end_node)
            {
                while (skippable_types.Contains(start_node.Name) && start_node.NextSibling != null)
                {
                    start_node = start_node.NextSibling;
                }

                scrape_nodes(start_node.DescendantsAndSelf().ToArray());

                if (start_node.NextSibling == null)
                {
                    break;
                }
                start_node = start_node.NextSibling;
            }
            scrape_nodes(start_node.DescendantsAndSelf().ToArray());
        }

        static void scrape_nodes(HtmlNode[] descendants)
        {
            for (int i = 0; i < descendants.Length; i++)
            {
                if (skippable_types.Contains(descendants[i].Name))
                {
                    if (descendants[i].HasChildNodes)
                    {
                        continue;
                    }
                    continue;
                }
                if (skip_useless_valid_descendant(descendants[i]))
                {
                    continue;
                }
                add_to_dictionary(match_scraped_string(descendants[i].InnerText));
            }
        }

        static bool skip_useless_valid_descendant(HtmlNode node) {
            return node.NodeType != HtmlNodeType.Text || node.InnerHtml == null || node.InnerHtml == "";
        }

        static string[] match_scraped_string(string s) {
            return Regex.Matches(s, @"\b[A-Za-z]+\b")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();
        }

        static void add_to_dictionary(string[] arr)
        {
            foreach (string s in arr)
            {
                if(excluded_words.Contains(s)) {
                    return;
                }
                if (!Word_Dict.ContainsKey(s))
                {
                    Word_Dict.Add(s, 1);
                }
                Word_Dict[s]++;
            }
        }
    }
}