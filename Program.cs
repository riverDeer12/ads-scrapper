using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

static HtmlDocument GetDocument(string url) 
{ 
    HtmlWeb web = new HtmlWeb(); 
    HtmlDocument doc = web.Load(url); 
    return doc; 
}

const string njuskaloGolf7Url = "https://www.njuskalo.hr/auti/vw-golf-7";

var doc = GetDocument(njuskaloGolf7Url);

// wrappers
var carAdsWrappers = doc.DocumentNode
    .Descendants("li")
    .Where(d => d.Attributes["class"].Value
        .Contains("EntityList-item EntityList-item--Regular")).ToList();

// articles

var carAds = new List<HtmlNode>();

foreach (var wrapper in carAdsWrappers)
{
    var wrapperCarAds = wrapper
        .Descendants("article")
        .Where(d => d.Attributes["class"].Value
            .Contains("entity-body cf")).ToList();
    
    carAds.AddRange(wrapperCarAds);
}

// titles
foreach (var ad in carAds)
{
    var description = ad
        .Descendants("div")
        .FirstOrDefault(d => d.Attributes["class"].Value
            .Contains("entity-description-main"))
        ?.InnerText;

    var titleText = ad
        .Descendants("a")
        .FirstOrDefault()?.InnerText;
    
    Console.WriteLine("Naziv: {0}", titleText);
    Console.WriteLine("Opis: {0}", description);
    Console.WriteLine();
}