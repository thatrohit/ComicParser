using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ComicParser
{
    public class ExplosmParser
    {
        string html = string.Empty;
        public string randomComicURL = "http://explosm.net/comics/random";
        public string comicPageURL = string.Empty;
        HtmlDocument oHtmlDocument = new HtmlDocument();

        /// <summary>
        /// call me first to load the htmldocument object
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task GetSiteHTML(string url = "http://explosm.net/comics/latest")
        {
            HttpClient oHttpClient = new HttpClient();
            html = await oHttpClient.GetStringAsync(url);
            comicPageURL = url;
            oHtmlDocument.LoadHtml(html);
        }

        /// <summary>
        /// call me to get the site's comic url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetComicURL(string url = "http://explosm.net/comics/latest")
        {
            string comicURL = string.Empty;
            if (!string.IsNullOrEmpty(html))
            {
                try
                {
                    var shortURL = oHtmlDocument.GetElementbyId("comic-container").ChildNodes[1].ChildNodes[1].ChildNodes[1].GetAttributeValue("href", "");
                    comicURL = oHtmlDocument.GetElementbyId("main-comic").GetAttributeValue("src", "");
                    if (comicURL.Substring(0, 4) != "http")
                    {
                        comicURL = "http:" + comicURL;
                    }
                }
                catch
                {
                }
            }
            return comicURL;
        }

        public string GetNextComicURL()
        {
            string nextComicURL = string.Empty;
            try
            {
                nextComicURL = "http://explosm.net" + oHtmlDocument.DocumentNode.Descendants().Where(n => n.GetAttributeValue("title", "") == "Next comic").FirstOrDefault().GetAttributeValue("href", "");
            }
            catch
            {
            }
            return nextComicURL;
        }

        public string GetPreviousComicURL()
        {
            string prevComicURL = string.Empty;
            try
            {
                prevComicURL = "http://explosm.net" + oHtmlDocument.DocumentNode.Descendants().Where(n => n.GetAttributeValue("title", "") == "Previous comic").FirstOrDefault().GetAttributeValue("href", "");
            }
            catch
            {
            }
            return prevComicURL;
        }

        public object ComicInfo()
        {
            string comicAuthor = string.Empty;
            string comicNumber = string.Empty;
            string comicName = string.Empty;
            string comicDate = string.Empty;

            try
            {
                var splitUrl = (oHtmlDocument.GetElementbyId("main-comic").GetAttributeValue("src", "")).Split('/');
                comicName = splitUrl.Last();

                if (splitUrl.Where(n => n.ToLower() == "dave").Any())
                {
                    comicAuthor = "Dave";
                }
                if (splitUrl.Where(n => n.ToLower() == "kris").Any())
                {
                    comicAuthor = "Kris";
                }
                if (splitUrl.Where(n => n.ToLower() == "rob").Any())
                {
                    comicAuthor = "Rob";
                }
            }
            catch
            {
            }

            try
            {
                comicDate = oHtmlDocument.DocumentNode.Descendants().Where(n => n.GetAttributeValue("class", "") == "zeta small-bottom-margin past-week-comic-title").FirstOrDefault().DescendantNodes().ElementAt(0).InnerText;
            }
            catch
            {
            }

            try
            {
                comicNumber = comicPageURL.Split('/').Last();
            }
            catch
            {
            }

            return new { comicAuthor = comicAuthor, comicName = comicName, comicNumber = comicNumber, comicPageURL = comicPageURL, comicDate = comicDate };
        }
    }
}
