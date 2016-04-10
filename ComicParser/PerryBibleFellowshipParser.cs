using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ComicParser
{
    public class PerryBibleFellowshipParser
    {
        public string currentComicPageURL = string.Empty;
        public readonly string CORE_URL = "http://pbfcomics.com/";
        public string latestComicPageURL = string.Empty;
        public int latestComicNumber;


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
                var rss = await oHttpClient.GetStringAsync(CORE_URL + "feed/feed.xml");
                XElement oXElement = XElement.Parse(rss);
                //wacky xml i tell ya
                var itemNode = oXElement.Elements().Where(e => e.Name.LocalName == "entry").FirstOrDefault();
                latestComicPageURL = itemNode.Elements().Where(e => e.Name.LocalName == "link").FirstOrDefault().Attribute("href").Value;
                var number = latestComicPageURL.Split(new string[] {"pbfcomics.com/"}, StringSplitOptions.None)[1];
                if(number.Contains('/'))
                {
                    number = number.Split('/')[0];
                }
                latestComicNumber = Int32.Parse(number);
            }
            catch { }
        }

        public async Task<string> GetComicURL(string comicPageURL)
        {
            string comicURL = string.Empty;
            try
            {
                HttpClient oHttpClient = new HttpClient();
                string html = await oHttpClient.GetStringAsync(comicPageURL);
                HtmlDocument oHtmlDocument = new HtmlDocument();
                oHtmlDocument.LoadHtml(html);
                comicURL = (oHtmlDocument.DocumentNode.Descendants("meta").Where(node => node.GetAttributeValue("property", "") == "og:image").FirstOrDefault()).GetAttributeValue("content", "");
                var currentComicNumber = comicPageURL.Split(new string[] { "pbfcomics.com/" }, StringSplitOptions.None).Last();
                if (currentComicNumber.Last() == '/')
                {
                    currentComicNumber = currentComicNumber.Split('/')[0];
                }
                currentComicPageURL = CORE_URL + currentComicNumber;
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
            try
            {
                if(currentComicPageURL.Split('/').Last() != latestComicNumber.ToString())
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

        public string GetRandomComicPageURL()
        {
            if (latestComicNumber == 0)
            {
                return string.Empty;
            }
            string randomComicNumber = (new Random().Next(1, latestComicNumber)).ToString();
            return CORE_URL + randomComicNumber;
        }
    }
}
