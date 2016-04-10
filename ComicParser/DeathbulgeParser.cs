using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using HtmlAgilityPack;

namespace ComicParser
{
    /// <summary>
    /// can't get it to work yet
    /// any sitehtml returns the latest comic
    /// work on it later
    /// </summary>
    public class DeathbulgeParser
    {
        public string currentComicPageURL = string.Empty;
        public readonly string CORE_URL = "http://deathbulge.com/#/comics/";
        public int latestComicNumber;
        Dictionary<string, string> dictRSS = new Dictionary<string, string>();
        
        /// <summary>
        /// CALL THIS FIRST
        /// this substitues the constructor for a lack of async modifier
        /// buils a dict of the latest comics provided by the rss feed
        /// </summary>
        public async Task DownloadAndLoadRSS()
        {
            try
            {
                HttpClient oHttpClient = new HttpClient();
                var rss = await oHttpClient.GetStringAsync("http://www.deathbulge.com/rss.xml");
                XElement oXElement = XElement.Parse(rss);
                var itemNode = oXElement.Elements("channel").FirstOrDefault().Elements("item");
                foreach(var item in itemNode)
                {
                    dictRSS.Add(item.Element("link").Value, item.Element("description").Value.Split('"')[1]);
                }
                currentComicPageURL = itemNode.FirstOrDefault().Element("link").Value;
                latestComicNumber = Int32.Parse(currentComicPageURL.Split('/').Last());
            }
            catch { }
        }

        public string WebViewHTML(string comicURL)
        {
            string webViewHTML = String.Format(@"<!doctype html>
                <html>
                <head>
                <meta name='viewport' content='width=device-width, initial-scale=1' />
                <title></title>
                </head>
                <body>
                <div style='width:100%; height:100%; margin: auto; margin-top:10%'><img src='{0}' width='100%' alt='' /></div>
                </body>
                </html>", comicURL);
            return webViewHTML;
        }

        /// <summary>
        /// checks if the image for the param exists in the rss dict
        /// if yes then return, if no then download the html for that page
        /// and get the image from the meta tag and return
        /// </summary>
        /// <param name="comicPageURL"></param>
        /// <returns></returns>
        public async Task<string> GetComicURL(string comicPageURL)
        {
            string comicURL = string.Empty;
            if (dictRSS.Keys.Contains(comicPageURL))
            {
                return dictRSS[comicPageURL];
            }
            try
            {
                HttpClient oHttpClient = new HttpClient();
                string html = await oHttpClient.GetStringAsync(comicPageURL);
                HtmlDocument oHtmlDocument = new HtmlDocument();
                oHtmlDocument.LoadHtml(html);
                comicURL = (oHtmlDocument.DocumentNode.Descendants("meta").Where(node => node.GetAttributeValue("property", "") == "og:image").FirstOrDefault()).GetAttributeValue("content", "");
                currentComicPageURL = CORE_URL + comicPageURL.Split('/').Last();
            }
            catch
            {
            }
            return comicURL;
        }

        public string GetPreviousComicPageURL()
        {
            string prevComicPageURL = string.Empty;
            try
            {
                if (currentComicPageURL.Split('/').Last() != "1")
                {
                    string previousComicNumber = (Int32.Parse(currentComicPageURL.Split('/').Last()) - 1).ToString();
                    prevComicPageURL = CORE_URL + previousComicNumber;
                }
            }
            catch
            {
            }
            return prevComicPageURL;
        }

        public string GetNextComicPageURL()
        {
            string nextComicPageURL = string.Empty;
            if (!dictRSS.Any())
            {
                return nextComicPageURL;
            }
            if (dictRSS.Keys.FirstOrDefault() == currentComicPageURL)
            {
                return nextComicPageURL;
            }
            else
            {
                try
                {
                    if (currentComicPageURL.Split('/').Last() != latestComicNumber.ToString())
                    {
                        string nextComicNumber = (Int32.Parse(currentComicPageURL.Split('/').Last()) + 1).ToString();
                        nextComicPageURL = CORE_URL + nextComicNumber;
                    }
                }
                catch
                {
                }
                return nextComicPageURL;
            }
        }

        public string GetRandomComicPageURL()
        {
            if(latestComicNumber == 0)
            {
                return string.Empty;
            }
            string randomComicNumber = (new Random().Next(1, latestComicNumber)).ToString();
            return CORE_URL + randomComicNumber;
        }
    }
}
