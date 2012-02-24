using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace AppStoreReview
{
    public class AppStore
    {
        #region Constant

        /// <summary>AppStore URL</summary>
        private readonly string STORE_URL = @"http://ax.phobos.apple.com.edgesuite.net/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id={0}&pageNumber={1}&sortOrdering=4&type=Purple+Software";

        /// <summary>Country Code</summary>
        private readonly int JAPAN = 143462;

        #endregion

        #region Member

        /*
         * Initial page's value is 4. 
         * 1 page contais 25 reviews.
         */
        private int _pages = 4;

        /*
         * Application ID
         * you can see in iTunes by browser.
         * for example, iBooks's id is 364709193.
         */
        private int _id;

        #endregion

        #region Constructor

        public AppStore(int id)
        {
            _id = id;
        }

        public AppStore(int id, int pages)
        {
            _id = id;
            _pages = pages;
        }
        #endregion

        /// <summary>
        /// Get AppStore Reviews.
        /// </summary>
        /// <returns>Review lists of map</returns>
        public List<Dictionary<string, string>> GetReviews()
        {
            var ret = new List<Dictionary<string, string>>();
            for (var i = 0; i < _pages; i++)
                 ret.AddRange(ParseReviews(ConnectAppStore(i)));

            return ret;
        }

        /// <summary>
        /// Get AppStore Reviews.
        /// </summary>
        /// <param name="pages">pages (1 page contains 25 reviews.) </param>
        /// <returns>Review lists of map</returns>
        public List<Dictionary<string, string>> GetReviews(int pages)
        {
            var ret = new List<Dictionary<string, string>>();
            for (var i = 0; i < pages + 1; i++)
                ret.AddRange(ParseReviews(ConnectAppStore(i)));

            return ret;
        }

        /// <summary>
        /// Get responses after connecting AppStore
        /// </summary>
        /// <param name="page">pages (1 page contains 25 reviews.) </param>
        /// <returns>body string of responses</returns>
        protected string ConnectAppStore(int page)
        {
            var req = (HttpWebRequest)WebRequest.Create(String.Format(STORE_URL, _id, page));
            req.UserAgent = "iTunes/9.1.0.79";
            req.Headers.Add(String.Format("X-Apple-Store-Front: {0}-1", JAPAN));
            var res = req.GetResponse();

            var body = "";
            using (var st = res.GetResponseStream())
            using (var sr = new StreamReader(st, Encoding.UTF8))
            {
                body = sr.ReadToEnd();
            }

            return body;
        }

        /// <summary>
        /// Parse reviews
        /// </summary>
        /// <param name="xml">review body</param>
        /// <returns>parse result</returns>
        protected List<Dictionary<string, string>> ParseReviews(string xml)
        {
            var reviews = new List<Dictionary<string, string>>();
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
            if (cnt == stars.Count() && cnt == etcs.Count() && cnt == comments.Count())
            {
                for (var i = 0; i < cnt; i++)
                {
                    var dict = new Dictionary<string, string>();
                    dict.Add("title", titles.ElementAt(i));
                    dict.Add("stars", ConvertStar(stars.ElementAt(i)));
                    var str = (string)etcs.ElementAt(i);
                    var tmp = str.Split('\n');

                    foreach (var s in tmp)
                    {
                        var reg = new Regex(@"Version \d+.\d+.\d+");
                        //var reg = new Regex(@"\d+.\d+.\d+");
                        var result = reg.Match(s);
                        if (result.Success)
                            dict.Add("version", result.Value.Replace("Version ",""));

                        reg = new Regex(@"\d\d-...-\d\d\d\d");
                        result = reg.Match(s);
                        if (result.Success)
                            dict.Add("date", result.Value);

                        if (s.Contains("Anonymous"))
                            dict.Add("reviewer", "Anonymous");
                        else if (s.Contains("by"))
                        {
                            reg = new Regex(@" +.+ +");
                            result = reg.Match(s);
                            if (result.Success)
                                dict.Add("reviewer", tmp[4].Trim());
                        }
                    }
                    dict.Add("comment", comments.ElementAt(i));
                    reviews.Add(dict);
                }
            }

            return reviews;
        }

        protected string ConvertStar(string stars)
        {
            var star_str = "";

            if (stars.Contains('1'))
                star_str = "☆";
            else if (stars.Contains('2'))
                star_str = "☆☆";
            else if (stars.Contains('3'))
                star_str = "☆☆☆";
            else if (stars.Contains('4'))
                star_str = "☆☆☆☆";
            else if (stars.Contains('5'))
                star_str = "☆☆☆☆☆";

            return star_str;
        }

        public string ReviewToStr(List<Dictionary<string, string>> reviews)
        {
            var sb = new StringBuilder();
            
            foreach (var rev in reviews)
            {
                sb.Append("Title:");
                sb.Append(rev["title"]);
                sb.Append(Environment.NewLine);
                sb.Append("Stars:" + rev["stars"]);
                sb.Append(Environment.NewLine);
                sb.Append("Reviewer:");
                sb.Append(rev["reviewer"]);
                sb.Append(Environment.NewLine);

                if (rev["reviewer"] != "Anonymous")
                {
                    sb.Append("Version:" + rev["version"]);
                    sb.Append(Environment.NewLine);
                    sb.Append("Date:");
                    sb.Append(rev["date"]);
                    sb.Append(Environment.NewLine);
                }

                sb.Append("Comment:");
                sb.Append(rev["comment"]);
                sb.Append(Environment.NewLine);
                sb.Append("-------------");
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
