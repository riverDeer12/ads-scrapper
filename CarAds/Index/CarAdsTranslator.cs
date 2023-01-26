using System.Text.Json;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AdsScrapper.CarAds.Index;

public class CarAdsTranslator
{
    private readonly CarAd _carAd = new();
    private readonly HtmlNode _loadedAd;

    public CarAdsTranslator(HtmlNode loadedAd)
    {
        _loadedAd = loadedAd;

        ProcessAd();
    }

    /*
     * Process incoming HTML ad document
     * into class properties.
     */
    private void ProcessAd()
    {
        _carAd.Title = _loadedAd
            .Descendants("span")
            .FirstOrDefault(d => d.HasClass("title"))?.InnerText;

        var description = _loadedAd
            .Descendants("ul")
            .FirstOrDefault(d => d.HasClass("tags hide-on-small-only"));

        if (description == null) return;

        var descriptionItems = description.Descendants("li").ToList();

        _carAd.Year = descriptionItems[0].InnerText;
        _carAd.Mileage = descriptionItems[1].InnerText;
        _carAd.Power = descriptionItems[3].InnerText;
        _carAd.Link = GetLink();
    }

    /**
     * Get car ad link for additional
     * info about ad.
     */
    private string? GetLink()
    {
        var carLinkElement = _loadedAd
            .Descendants("a")
            .FirstOrDefault(x => x.HasClass("result"));

        return carLinkElement == null ? string.Empty : carLinkElement.Attributes["href"].Value;
    }

    /**
     * Convert car ad into
     * JSON object.
     */
    public string GetJson() => JsonConvert.SerializeObject(_carAd);
}