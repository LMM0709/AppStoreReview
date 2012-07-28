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

            for (var i = 0; i <= reviewPages; i++)
                reviews.AddRange(ParseReviews(ConnectAppStore(id, i, storeCode)));

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
            using (var st = res.GetResponseStream())
            using (var sr = new StreamReader(st, Encoding.UTF8))
            {
                body = sr.ReadToEnd();
            }

            return body;
        }

        private List<Review> ParseReviews(string xml)
        {
            var elem = XElement.Load(new StringReader(xml));
            XNamespace ns = "http://www.apple.com/itms/";

            //title
            var titles = from item
            in elem.Elements(ns + "View").Elements(ns + "ScrollView")
            .Elements(ns + "VBoxView").Elements(ns + "View")
            .Elements(ns + "MatrixView").Elements(ns + "VBoxView").Elements(ns + "VBoxView").Elements(ns + "VBoxView")
            .Elements(ns + "HBoxView").Elements(ns + "TextView").Elements(ns + "SetFontStyle").Elements(ns + "b")
                         select item.Value;

            //stars
            var stars = from item
            in elem.Elements(ns + "View").Elements(ns + "ScrollView")
                .Elements(ns + "VBoxView").Elements(ns + "View")
                .Elements(ns + "MatrixView").Elements(ns + "VBoxView").Elements(ns + "VBoxView").Elements(ns + "VBoxView")
                .Elements(ns + "HBoxView").Elements(ns + "HBoxView").Elements(ns + "HBoxView")
            where (string)item.Attribute("topInset") == "1"
            select item.Attribute("alt").Value;

            //version & date
            var etcs = from item
            in elem.Elements(ns + "View").Elements(ns + "ScrollView")
                .Elements(ns + "VBoxView").Elements(ns + "View")
                .Elements(ns + "MatrixView").Elements(ns + "VBoxView").Elements(ns + "VBoxView").Elements(ns + "VBoxView")
                .Elements(ns + "HBoxView").Elements(ns + "TextView").Elements(ns + "SetFontStyle")
            where ((string)item.Value).Contains("by")
            select item.Value;

            //comment
            var comments = from item
            in elem.Elements(ns + "View").Elements(ns + "ScrollView")
                .Elements(ns + "VBoxView").Elements(ns + "View")
                .Elements(ns + "MatrixView").Elements(ns + "VBoxView").Elements(ns + "VBoxView").Elements(ns + "VBoxView")
                .Elements(ns + "TextView").Elements(ns + "SetFontStyle")
            where (string)item.Attribute("normalStyle") == "textColor"
            select item.Value;

            var cnt = titles.Count();

            var reviews = new List<Review>();
            if (cnt == stars.Count() && cnt == etcs.Count() && cnt == comments.Count())
            {
                for (var i = 0; i < cnt; i++)
                {
                    var review = new Review();
                    review.Title = titles.ElementAt(i);

                    var starsCount = 0;
                    int.TryParse(new Regex(@"\d").Match(stars.ElementAt(i)).Value, out starsCount);
                    review.Stars = starsCount;

                    var str = (string)etcs.ElementAt(i);
                    var tmp = str.Split('\n');

                    foreach (var s in tmp)
                    {
                        var reg = new Regex(@"Version \d+.\d+.\d+");
                        var result = reg.Match(s);
                        if (result.Success)
                            review.Version = result.Value.Replace("Version ", "");

                        reg = new Regex(@"\d\d-...-\d\d\d\d");
                        result = reg.Match(s);
                        if (result.Success)
                            review.Date = result.Value;

                        if (s.Contains("Anonymous"))
                            review.Reviewer = "Anonymous";
                        else if (s.Contains("by"))
                        {
                            reg = new Regex(@" +.+ +");
                            result = reg.Match(s);
                            if (result.Success)
                                review.Reviewer = tmp[4].Trim();
                        }
                    }
                    review.Comment = comments.ElementAt(i);
                    reviews.Add(review);
                }
            }

            return reviews;
        }
    }
}
