using System;
using System.Linq;

class Program
{
    public static void Main()
    {
        var scraper = new AppStore.ReviewScraper();

        //TODO: You have to search target's AppId.
        //iBooks http://itunes.apple.com/jp/app/ibooks/id364709193?mt=8
        var reviews = scraper.GetReviews(364709193, 2, AppStore.InternationalCode.UnitedStates);

        foreach (var review in reviews.Take(10))
        {
            Console.WriteLine(review);
        }

        Console.ReadKey();
    }
}