using HtmlAgilityPack;

const string njuskaloGolf7Url = "https://www.njuskalo.hr/auti/vw-golf-7";

// Make request to wanted html document file
static HtmlDocument GetDocument(string url)
{
    var web = new HtmlWeb();
    var doc = web.Load(url);
    return doc;
}

// Get car ads wrappers
// Car ads will be extracted from wrappers
static List<HtmlNode> GetCarAdsWrappers(HtmlDocument doc)
{
    return doc.DocumentNode
        .Descendants("li")
        .Where(d => d.Attributes["class"].Value
            .Contains("EntityList-item EntityList-item--Regular")).ToList();
}

// Extract car ads from car ads wrappers
static List<HtmlNode> GetCarAds(List<HtmlNode> carAdsWrappers)
{
    var carAds = new List<HtmlNode>();

    foreach (var wrapper in carAdsWrappers)
    {
        var wrapperCarAds = wrapper
            .Descendants("article")
            .Where(d => d.Attributes["class"].Value
                .Contains("entity-body cf")).ToList();

        carAds.AddRange(wrapperCarAds);
    }

    return carAds;
}

// Write car ads to .txt file. 
void WriteCarAdsToFile(List<HtmlNode> carAds)
{
    using var w = File.AppendText("njuskalo_oglasi.txt");

    foreach (var ad in carAds)
    {
        var titleText = ad
            .Descendants("a")
            .FirstOrDefault()?.InnerText;

        var description = ad
            .Descendants("div")
            .FirstOrDefault(d => d.Attributes["class"].Value
                .Contains("entity-description-main"))
            ?.InnerText;

        var formattedDescription = description?.Trim();

        var carAd = $"Naziv: {titleText}, Opis: {formattedDescription} ";

        w.WriteLine(carAd);
    }
}

var doc = GetDocument(njuskaloGolf7Url);

var carAdsWrappers = GetCarAdsWrappers(doc);

var carAds = GetCarAds(carAdsWrappers);

WriteCarAdsToFile(carAds);