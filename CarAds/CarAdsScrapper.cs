using AdsScrapper.CarAds.Index;
using AdsScrapper.CarAds.Sniffer;

namespace AdsScrapper.CarAds;

public static class CarAdsScrapper
{
    /*
     * Main method for running all
     * necessary car ads scrappers.
     *
     */
    public static void Run(CarType carType)
    {
        // SnifferCarAds.GetAds(carType);
        
        IndexCarAds.GetAds(carType);
    }
}