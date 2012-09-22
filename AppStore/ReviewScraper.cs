using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace AppStore
{
    public class ReviewScraper
    {
        /// <summary>
        /// Get AppStore Reviews.
        /// </summary>
        /// <param name="id">
        /// You can see this in iTunes by web browser.
        /// For example, iBooks's id is 364709193.
        /// </param>
        /// <param name="reviewPages">
        /// 1 page contains 25 reviews. Default value is 4. (recent 100 reviews)
        /// </param>
        /// <param name="storeCode">International Store Code</param>
        public List<Review> GetReviews(int id, int reviewPages = 4, int storeCode = InternationalCode.Japan)
        {
            var reviews = new List<Review>();
            for (var i = 0; i < reviewPages; i++)
            {
                reviews.AddRange(ParseReviews(ConnectAppStore(id, i, storeCode)));
            }

            return reviews;
        }

        private string ConnectAppStore(int id, int page, int storeCode)
        {
            var AppStoreUri = @"http://ax.phobos.apple.com.edgesuite.net/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id={0}&pageNumber={1}&sortOrdering=4&type=Purple+Software";
            var req = (HttpWebRequest)WebRequest.Create(String.Format(AppStoreUri, id, page));
            req.UserAgent = "iTunes 9.1.1";
            req.Headers.Add(String.Format("X-Apple-Store-Front: {0}-1", storeCode));
            var res = req.GetResponse();

            var body = "";
            using (var stream = res.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                body = reader.ReadToEnd();
            }

            return body;
        }

        private IEnumerable<Review> ParseReviews(string xml)
        {
            var contentXPath = @"/document[1]/view[1]/scrollview[1]/vboxview[1]/view[1]/matrixview[1]/vboxview[1]/vboxview[1]/vboxview";
            
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(xml);

            return doc.DocumentNode.SelectNodes(contentXPath).Select(x => new AppStore.Review
            {
                Title = x.SelectSingleNode(x.XPath + "/hboxview[1]/textview[1]/setfontstyle[1]/b[1]").InnerText,
                Stars = SelectStars(x.SelectSingleNode(x.XPath + "/hboxview[1]/hboxview[1]").InnerHtml),
                Reviewer = x.SelectSingleNode(x.XPath + "/hboxview[2]/textview[1]/setfontstyle[1]/gotourl[1]") != null 
                    ? x.SelectSingleNode(x.XPath + "/hboxview[2]/textview[1]/setfontstyle[1]/gotourl[1]").InnerText.Trim()
                    : "Anonymous",
                Version = SelectVersion(x.SelectSingleNode(x.XPath + "/hboxview[2]/textview[1]/setfontstyle[1]").InnerText),
                Date = SelectDate(x.SelectSingleNode(x.XPath + "/hboxview[2]/textview[1]/setfontstyle[1]").InnerText),
                Comment = x.SelectSingleNode(x.XPath + "/textview[1]/setfontstyle[1]").InnerText,
            });
        }

        private int SelectStars(string content)
        {
            var reg = new Regex("alt\\s*=\\s*(?:\"(?<1>[^\"]*)|(?<1>\\S+)) star",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var stars = 0;
            return reg.IsMatch(content) && int.TryParse(reg.Match(content).Groups[1].Value, out stars)
                ? stars
                : 0;
        }

        private string SelectVersion(string content)
        {
            var reg = new Regex("Version\\s *\\s*(?:\"(?<1>[^\"]*)|(?<1>\\S+))",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return reg.IsMatch(content)
                ? reg.Match(content).Groups[1].Value
                : null;
        }

        private string SelectDate(string content)
        {
            var reg = new Regex(@"\d\d-...-\d\d\d\d",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return reg.IsMatch(content)
                ? reg.Match(content).Groups[0].Value
                : null;
        }
    }
}
