Apple Store Review Client C# Library
=============================================================

アップルストアからレビューを取得するライブラリです。

取得したレビューをメールで定期的に送信したりすると開発者のモチベーションアップにつながるかも。

How to use
--------------------

* Example

var store = new AppStore(364709193); //iBooks

var review = store.GetReviews();

Console.WriteLine(store.ReviewToStr(review));

Todo
--------------------

* Correspond to app store outside of Japan
* Fix to anonymous reviews

License
--------------------

MIT License. Copyright (c) 2012 Yasuyuki Harada