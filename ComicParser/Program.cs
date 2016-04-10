using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace ComicParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.DoExplosmShit();
            Console.Read();
        }

        private static async void DoDeathbulgeShit()
        {
            DeathbulgeParser parser = new DeathbulgeParser();
            await parser.DownloadAndLoadRSS();
            var prev = parser.GetPreviousComicPageURL();
            var next = parser.GetNextComicPageURL();
            var random = parser.GetRandomComicPageURL();
            await parser.GetComicURL(random);
        }

        private static async void DoEFBShit()
        {
            ExtraFabulousComicsParser parser = new ExtraFabulousComicsParser();
            await parser.GetSiteHTML();
            string comic = parser.GetComicURL();
            string next = parser.GetNextComicPageURL();
            string prev = parser.GetPreviousComicPageURL();
        }

        private static async void DoPFBShit()
        {
            PerryBibleFellowshipParser parser = new PerryBibleFellowshipParser();
            await parser.DownloadAndLoadRSS();
            string comic = await parser.GetComicURL("http://pbfcomics.com/123");
            string next = parser.GetNextComicPageURL();
            string prev = parser.GetPreviousComicPageURL();
            string random = parser.GetRandomComicPageURL();
        }

        private static async void DoExplosmShit()
        {
            ExplosmParser parser = new ExplosmParser();
            await parser.GetSiteHTML();
            string comic = parser.GetComicURL();
            string next = parser.GetNextComicURL();
            string prev = parser.GetPreviousComicURL();
            string random = parser.randomComicURL;

        }
    }
}
