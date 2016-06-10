﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Funda
{
    public class Crawler
    {
        public Crawler()
        {
            this.Driver = new ChromeDriver(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\Driver\"));

        }

        public void Navigate(Search search)
        {
            this.Driver.Url = search.Url;
            this.Driver.Navigate();
        }

        public List<Sale> AddNewSales(Search search)
        {
                var list = new List<Sale>();
                var adds = this.Driver.FindElementsByCssSelector(".search-result");
                foreach (var advert in adds)
                {
                    list.Add(this.GetSale(advert));
                }

            return list;
        }

        public List<Rent> AddNewRents(Search search)
        {
            var list = new List<Rent>();
            var adds = this.Driver.FindElementsByCssSelector(".search-result");
            foreach (var advert in adds)
            {
                list.Add(this.GetRent(advert));
            }

            return list;
        }

        public class Search
        {
            public int? MinRooms { get; set; }
            public int? MaxRooms { get; set; }
            public int? PriceMin { get; set; }
            public int? PriceMax { get; set; }
            public string Sorting { get { return "sorteer-datum-af/"; } }
            public string Text { get; set; }
            public bool IsSale { get; set; }
            public int? PaginationNumber { get; set; }

            public string Url
            {
                get
                {
                    string url = "http://www.funda.nl/";
                    url += IsSale ? "koop" : "huur";
                    url += "/";
                    url += this.Text + "/";
                    if (this.MinRooms.HasValue || this.MaxRooms.HasValue) // jei minRooms turi reiksme ARBA maxRooms turi reiksme tada varyt per koda kur tarp {}
                        {
                            var maxString = !this.MaxRooms.HasValue ? "+kamers" : string.Format("-{0}-kamers", this.MaxRooms.Value);
                            if (!this.MinRooms.HasValue) { this.MinRooms = 0; }
                            url += this.MinRooms.ToString();
                            url += maxString;
                            url += "/";
                        }

                    if (this.PriceMin.HasValue || this.PriceMax.HasValue)
                   {
                          var maxString = !this.PriceMax.HasValue ? "+" : string.Format("-{0}", this.PriceMax.Value);
                          if (!this.PriceMin.HasValue) { this.PriceMin = 0; }
                          url += this.PriceMin.ToString();
                          url += maxString;
                          url += "/";
                  }
                    if (this.PaginationNumber.HasValue)
                    {
                        url += string.Format("p{0}/", this.PaginationNumber.Value);
                    }
                     
                       

                  //s  return "http://www.funda.nl/huur/rotterdam/1-4-kamers/sorteer-datum-af/p2/";
                    url += this.Sorting;
                    return url;
                }
            }
        }

        public Sale GetSale(IWebElement element)
        {
        var url = element.FindElements(By.CssSelector("a"));
        var title = element.FindElement(By.CssSelector(".search-result-title"));
        var subTitle = element.FindElement(By.CssSelector(".search-result-subtitle"));
        var price = element.FindElement(By.CssSelector(".search-result-price"));
        var livingArea = element.FindElement(By.CssSelector("[title='Woonoppervlakte']"));
            var totalArea = element.FindElement(By.CssSelector("[title='Perceeloppervlakte']"));
            var roomCount = livingArea.FindElement(By.XPath(".."));
            var parsedPrice = 0M;
            var parsedLivingArea = 0;
            var parsedTotalArea = 0;
            var parsedRoomCount = 0;
            var postCodeRegex = new Regex("([1-9][0-9]{3} ?(?!sa|sd|ss)[a-z]{2})", RegexOptions.IgnoreCase).Matches(subTitle.Text);
            return new Sale
            {
                Url = url[2].GetAttribute("href"),
                Title = title.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)[0],
                Subtitle = subTitle.Text,
                Price = decimal.TryParse(new Regex("([0-9.]*)").Matches(price.Text)[2].Value, out parsedPrice) ? parsedPrice*1000 : (decimal?)null,
                LivingArea = int.TryParse(new System.Text.RegularExpressions.Regex("([0-9]*)").Matches(livingArea.Text)[0].Value, out parsedLivingArea) ? parsedLivingArea : (int?)null,
                TotalArea = int.TryParse(new System.Text.RegularExpressions.Regex("([0-9]*)").Matches(totalArea.Text)[0].Value, out parsedTotalArea) ? parsedTotalArea : (int?)null,
                RoomCount = int.TryParse(roomCount.Text.Split(new string[] { "\r\n", "\n", "•", "kamers" }, StringSplitOptions.None)[1].Trim(), out parsedRoomCount) ? parsedRoomCount : (int?)null,
                Address = title.Text.Replace("\r\n", ""),
                PostCode = postCodeRegex.Count != 0 ? postCodeRegex[0].Value : null
            };
        }

        public Rent GetRent(IWebElement element)
        {
            var url = element.FindElements(By.CssSelector("a"));
            var title = element.FindElement(By.CssSelector(".search-result-title"));
            var subTitle = element.FindElement(By.CssSelector(".search-result-subtitle"));
            var price = element.FindElement(By.CssSelector(".search-result-price"));
            var livingArea = element.FindElement(By.CssSelector("[title='Woonoppervlakte']"));
            var totalArea = element.FindElement(By.CssSelector("[title='Perceeloppervlakte']"));
            var roomCount = livingArea.FindElement(By.XPath(".."));
            var parsedPrice = 0M;
            var parsedLivingArea = 0;
            var parsedTotalArea = 0;
            var parsedRoomCount = 0;
            var postCodeRegex = new Regex("([1-9][0-9]{3} ?(?!sa|sd|ss)[a-z]{2})", RegexOptions.IgnoreCase).Matches(subTitle.Text);
            return new Rent
            {
                Url = url[2].GetAttribute("href"),
                Title = title.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)[0],
                Subtitle = subTitle.Text,
                Price = decimal.TryParse(new Regex("([0-9.]*)").Matches(price.Text)[2].Value, out parsedPrice) ? parsedPrice*1000 : (decimal?)null,
                LivingArea = int.TryParse(new System.Text.RegularExpressions.Regex("([0-9]*)").Matches(livingArea.Text)[0].Value, out parsedLivingArea) ? parsedLivingArea : (int?)null,
                TotalArea = int.TryParse(new System.Text.RegularExpressions.Regex("([0-9]*)").Matches(totalArea.Text)[0].Value, out parsedTotalArea) ? parsedTotalArea : (int?)null,
                RoomCount = int.TryParse(roomCount.Text.Split(new string[] { "\r\n", "\n", "•", "kamers" }, StringSplitOptions.None)[1].Trim(), out parsedRoomCount) ? parsedRoomCount : (int?)null,
                Address = title.Text.Replace("\r\n", ""),
                PostCode = postCodeRegex.Count !=0 ? postCodeRegex[0].Value : null
            };
        }

        private ChromeDriver Driver { get; set; }
    }
}
