Apple Store Review Client C# Library
=============================================================

アップルストアからレビューを取得するライブラリです。

取得したレビューをメールで定期的に送信したりすると開発者のモチベーションアップにつながるかも。

HOW TO USE
--------------------

* Example

var store = new AppStore(364709193); //iBooks

var review = store.GetReviews();

Console.WriteLine(store.ReviewToStr(review));

TODO
--------------------

* Correspond to app store outside of Japan
* Fix to anonymous reviews
