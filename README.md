Scraping Apple Store Review C# Library
=============================================================

アップルストアからレビューを取得するライブラリです。

取得したレビューをメールで定期的に送信したりすると開発者のモチベーションアップにつながるかも。

How to use
--------------------

* Example

You have to search target's AppId in iTunes Store.

    var scraper = new AppStore.ReviewScraper();

    var reviews = scraper.GetReviews(364709193, 1, AppStore.InternationalCode.UnitedStates);

    reviews.ForEach(x => Console.WriteLine(x));

Todo
--------------------

* [DONE]Correspond to app store outside of Japan
* Fix to anonymous reviews

License
--------------------

Ms-PL
