using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AdsScrapper.CarAds.Index;

public class IndexCarAdTranslator
{
    private readonly List<CarAd> _carAds = new();
    private readonly List<HtmlNode> _loadedAds;

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
            var carAd = new CarAd();

            carAd.Title = ad
                .Descendants("span")
                .FirstOrDefault(d => d.HasClass("title"))
                ?.InnerText
                .Trim();

            if (carAd.Title == null) continue;

            var descriptionContainer = ad
                .Descendants("ul")?.ToList();

            if (descriptionContainer == null || !descriptionContainer.Any())
            {
                _carAds.Add(carAd);
                continue;
            }

            var descriptionItems = descriptionContainer[0]
                .Descendants("li").ToList();

            carAd.Year = descriptionItems[0].InnerText.Trim();
            carAd.Mileage = descriptionItems.Count > 2
                ? descriptionItems[1].InnerText.Trim().Replace("\r\n", string.Empty)
                : "No data about mileage.";
            carAd.Power = descriptionItems.Count > 3
                ? descriptionItems[3].InnerText.Trim().Replace("\r\n", string.Empty)
                : "No data about power.";
            carAd.Link = GetLink(ad);

            _carAds.Add(carAd);
        }
    }

    /**
     * Get car ad link for additional
     * info about ad.
     */
    private static string? GetLink(HtmlNode ad)
    {
        var carLinkElement = ad
            .Descendants("a")
            .FirstOrDefault(x => x.HasClass("result"));

        return carLinkElement == null ? string.Empty : carLinkElement.Attributes["href"].Value;
    }
}