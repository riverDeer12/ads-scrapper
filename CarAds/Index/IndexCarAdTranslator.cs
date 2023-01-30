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
     * Convert car container into
     * JSON object.
     */
    public string GetJson() =>
        JsonConvert.SerializeObject(_carAds);

    /*
     * Process incoming HTML container document
     * into class properties.
     */
    private void ProcessAds()
    {
        foreach (var ad in _loadedAds)
        {
            _carAdInProcess = new CarAd();

            GetTitle(ad);

            GetLink(ad);

            GetPrice(ad);

            GetDescription(ad);

            _carAds.Add(_carAdInProcess);
        }
    }

    /**
     * Check if car container description
     * is available.
     */
    private void GetDescription(HtmlNode ad)
    {
        var descriptionContainer = ad.Descendants("ul")?.ToList();

        if (descriptionContainer == null || !descriptionContainer.Any()) return;

        GetDescriptionItems(descriptionContainer[0]);
    }

    /**
     * Run additional getters
     * for description items.
     */
    private void GetDescriptionItems(HtmlNode container)
    {
        var containerItems = container
            .Descendants("li")?.ToList();

        if (containerItems == null || !containerItems.Any()) return;

        GetMileage(containerItems);
        GetYear(containerItems);
        GetPower(containerItems);
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
     * more clear container data about car mileage.
     */
    private void GetMileage(IReadOnlyList<HtmlNode> container)
    {
        var mileageContainer = container[1];

        var validMileageContainer = mileageContainer.InnerText.Contains("km");

        if (!validMileageContainer) return;

        _carAdInProcess.Mileage = mileageContainer.InnerText;
    }

    /**
     * Helper method for getting
     * more clear container data about car year
     * when car is manufactured.
     */
    private void GetYear(IReadOnlyList<HtmlNode> container)
    {
        var yearContainer = container[0];

        var validYearContainer = yearContainer.InnerText.Contains("Godište");

        if (!validYearContainer) return;

        _carAdInProcess.Year = yearContainer.InnerText;
    }

    /**
     * Helper method for getting
     * more clear container data about car power.
     */
    private void GetPower(IReadOnlyList<HtmlNode> container)
    {
        var powerContainer = container[3];

        var validPowerContainer = powerContainer.InnerText.Contains("kW");

        if (!validPowerContainer) return;

        _carAdInProcess.Power = powerContainer.InnerText;
    }

    /**
     * Helper method for getting car container title.
     */
    private void GetTitle(HtmlNode ad)
    {
        _carAdInProcess.Title = ad.Descendants("span")
            .FirstOrDefault(d => d.HasClass("title"))
            ?.InnerText
            .Trim();
    }

    /**
     * Helper method for getting
     * more clear container data about car price.
     */
    private void GetPrice(HtmlNode ad)
    {
        var priceContainer = ad.Descendants("ul")?.ToList();

        if (priceContainer == null || !priceContainer.Any()) return;

        var innerPriceContainer =
            priceContainer[1].Descendants("span").ToList();

        if (innerPriceContainer != null || !innerPriceContainer.Any()) return;

        var priceSpan = innerPriceContainer.FirstOrDefault(x => x.HasClass("price"));

        _carAdInProcess.Price = priceSpan?.InnerText;
    }
}