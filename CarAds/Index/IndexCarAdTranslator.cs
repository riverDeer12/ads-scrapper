using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AdsScrapper.CarAds.Index;

public class IndexCarAdTranslator
{
    private readonly List<CarAd> _carAds = new();

    private readonly List<HtmlNode> _loadedAds;

    private CarAd _carAdInProcess = new();

    public IndexCarAdTranslator(List<HtmlNode> loadedAds)
    {
        _loadedAds = loadedAds;

        ProcessAds();
    }

    /**
     * Convert car ad into
     * JSON object.
     */
    public string GetJson() =>
        JsonConvert.SerializeObject(_carAds);

    /*
     * Process incoming HTML ad document
     * into class properties.
     */
    private void ProcessAds()
    {
        foreach (var ad in _loadedAds)
        {
            _carAdInProcess = new CarAd
            {
                Title = GetTitle(ad),
                Link = GetLink(ad),
                Price = GetPrice(ad)
            };

            ManageDescription(ad);

            _carAds.Add(_carAdInProcess);
        }
    }

    /**
     * Check if car ad description
     * is available.
     */
    private void ManageDescription(HtmlNode ad)
    {
        var descriptionContainer = ad.Descendants("ul")
            .FirstOrDefault(x => x.HasClass("tags hide-on-small-only"));

        if (descriptionContainer == null) return;

        GetDescriptionItems(ad);
    }

    /**
     * Run additional getters
     * for description items.
     */
    private void GetDescriptionItems(HtmlNode ad)
    {
        GetMileage(ad);
        GetYear(ad);
        GetPower(ad);
    }

    /**
     * Helper method for getting
     * more clear ad data about car mileage.
     */
    private string? GetMileage(HtmlNode ad)
    {
        return ad.InnerText.Trim().Replace("\r\n", string.Empty);
    }

    /**
     * Helper method for getting
     * external link to more details about car.
     */
    private static string? GetLink(HtmlNode ad)
    {
        var carLinkElement = ad
            .Descendants("a")
            .FirstOrDefault(x => x.HasClass("result"));

        return carLinkElement == null ? string.Empty : carLinkElement.Attributes["href"].Value;
    }

    /**
     * Helper method for getting
     * more clear ad data about car power.
     */
    private static string? GetPower(HtmlNode ad)
    {
        return ad.InnerText.Trim().Replace("\r\n", string.Empty);
    }

    /**
     * Helper method for getting car ad title.
     */
    private static string? GetTitle(HtmlNode ad)
    {
        return ad.Descendants("span")
            .FirstOrDefault(d => d.HasClass("title"))
            ?.InnerText
            .Trim();
    }

    /**
     * Helper method for getting
     * more clear ad data about car year
     * when car is manufactured.
     */
    private static string? GetYear(HtmlNode ad)
    {
        return ad.InnerText.Trim().Replace("\r\n", string.Empty);
    }

    /**
     * Helper method for getting
     * more clear ad data about car price.
     */
    private static string? GetPrice(HtmlNode ad)
    {
        var priceContainer = ad.Descendants("ul").FirstOrDefault(x => x.HasClass("info"));

        return priceContainer == null ? string.Empty : priceContainer.InnerText.Trim();
    }
}