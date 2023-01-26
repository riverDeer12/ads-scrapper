using AdsScrapper.CarAds.Index;

namespace AdsScrapper.CarAds;

public static class CarAdsScrapper
{
    private const string NjuskaloGolf7Url = "https://www.njuskalo.hr/auti/vw-golf-7";

    private const string IndexGolf7Url =
        "https://www.index.hr/oglasi/osobni-automobili/gid/27?pojam=&sortby=1&elementsNum=10&cijenaod=0&cijenado=11000000&tipoglasa=1&markavozila=11944&modelvozila=12900&pojamZup=-2&attr_Int_179=&attr_Int_1190=&attr_Int_470=&attr_Int_910=&attr_bit_motor=&attr_bit_473=&attr_Int_1172=&attr_Int_1335=&attr_Int_359=&attr_Int_1192=&attr_bit_klima=&attr_bit_mjenjac=&attr_bit_brzina=&attr_bit_vrsta_pogona=&vezani_na=179-1190_470-910_1172-1335_359-1192";

    /*
     * Main method for running all
     * necessary car ads scrappers.
     */
    public static void Run()
    {
        // SnifferCarAds.GetAds(NjuskaloGolf7Url);
        IndexCarAds.GetAds(IndexGolf7Url);
    }
}