using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.IO;

namespace PhantomJS
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> postLinksList = new List<string>();
            List<string> extractedContent = new List<string>();
            List<string> extractedContentTitles = new List<string>();

            string[] pageSource;

            string url = @"http://testing.todvachev.com/sitemap-posttype-post.xml";
            string contentSelector = "#main-content > article > div";
            string titlesSelector = "#main-content > article > header > h1";
            string folderName = @"ExtractedContent\";
            string path;

            int startIndexOfLink;
            int lengthOfLink;

            IWebDriver phantom = new PhantomJSDriver();
            IWebElement content;
            IWebElement titles;
            
            phantom.Navigate().GoToUrl(url);
            pageSource = phantom.PageSource.Split(' ');
            
            // Extract all of the post links from the Sitemap.xml, work on it as a string
            foreach (var item in pageSource)
            {
                if (item.Contains(@"<loc>http://testing.todvachev.com/"))
                {
                    startIndexOfLink = item.IndexOf("<loc>") + 5;
                    lengthOfLink = item.LastIndexOf("</loc>") - startIndexOfLink;

                    postLinksList.Add(item.Substring(startIndexOfLink, lengthOfLink));
                }
            }

            // Open each of the posts and extract the title and the content
            foreach (var item in postLinksList)
            {
                phantom.Navigate().GoToUrl(item);
                content = phantom.FindElement(By.CssSelector(contentSelector));
                titles = phantom.FindElement(By.CssSelector(titlesSelector));
                //Console.WriteLine(titles.Text);
                //Console.WriteLine(content.Text);
                extractedContent.Add(content.Text);
                extractedContentTitles.Add(titles.Text);
            }

            // Create the directory for the text files
            Directory.CreateDirectory(folderName);

            // Write the titles and content into text files
            for (int i = 0; i < extractedContent.Count; i++)
            {
                path = String.Format(@"extractedContent\0{0} {1}.txt", i, extractedContentTitles[i]);
                Console.WriteLine(path);

                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("TITLE: {0}", extractedContentTitles[i]);
                    sw.WriteLine();
                    sw.WriteLine("CONTENT:");
                    sw.WriteLine(extractedContent[i]);
                }
            }
        }
    }
}   
