using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ComicParser
{
    public class ExtraFabulousComicsParser
    {
        string html = string.Empty;
        public readonly string CORE_URL = "http://extrafabulouscomics.com/comic/";
        public readonly string randomComicURL = "http://extrafabulouscomics.com/?random&amp;nocache=1";
        HtmlDocument oHtmlDocument = new HtmlDocument();


        /// <summary>
        /// CALL ME FIRST
        /// this substitues the constructor for a lack of async modifier
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task GetSiteHTML(string url = "http://extrafabulouscomics.com/")
        {
            HttpClient oHttpClient = new HttpClient();
            html = await oHttpClient.GetStringAsync(url);
            oHtmlDocument.LoadHtml(html);
        }

        /// <summary>
        /// returns youtube url if video,
        /// else returns comic url
        /// </summary>
        /// <returns></returns>
        public string GetComicURL()
        {
            string comicURL = string.Empty;
            try
            {
                if (oHtmlDocument.GetElementbyId("comic").Descendants("a").Any())
                {
                    comicURL = oHtmlDocument.GetElementbyId("comic").Descendants("a").FirstOrDefault().Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
                }
                else if (oHtmlDocument.GetElementbyId("comic").Descendants("img").Any())
                {
                    comicURL = oHtmlDocument.GetElementbyId("comic").Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
                }
                if (oHtmlDocument.GetElementbyId("comic").Descendants("iframe").Any())
                {
                    comicURL = oHtmlDocument.GetElementbyId("comic").Descendants("iframe").FirstOrDefault().GetAttributeValue("src", "");
                }
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
                prevComicPageURL = (oHtmlDocument.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("title", "") == "PREV").FirstOrDefault()).GetAttributeValue("href", "");
            }
            catch
            {
            }
            return prevComicPageURL;
        }

        public string GetNextComicPageURL()
        {
            string nextComicPageURL = string.Empty;
            try
            {
                nextComicPageURL = (oHtmlDocument.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("title", "") == "NEXT").FirstOrDefault()).GetAttributeValue("href", "");
            }
            catch
            {
            }
            return nextComicPageURL;
        }
    }
}
