# すぱーダ（SPADA）

---

名称 : すぱーダ（SPADA）  
バージョン : Ver. 1.0.0 ( Dec. 27, 2015 )  
  
種別 : .NET クラスライブラリ  
ターゲットバージョン : .NET Framework 4.0  
ターゲットでの必要なランタイム : .NET Framework 4.0以上
  
作成者 : 智中 ニア（ [@nia_tn1012](https://twitter.com/nia_tn1012/) ）  
ライセンス : MIT Licence  
リンク  
　http://chronoir.net （ホームページ）  
　http://chronoir.net/spada （ライブラリのページ）  
　https://github.com/Nia-TN1012/SPADA （GitHubのリポジトリ）
　https://www.nuget.org/packages/Chronir_net.SPADA/ （NuGet Gallery）

---

##◆ はじめに

ダウンロードしていただき、ありがとうございます。  
すぱーダ（SPADA）は、プログラミング生放送のすぱこーRSSフィード
（ http://pronama.azurewebsites.net/spaco-feed/ ）を簡単に読み込むことができるライブラリです。

##◆ ダウンロードとインストール方法

以下のリンクからzipファイルをダウンロードし、解凍します。  
[Chronoir_net.SPADA_1.0.0.0.zip](http://chronoir.net/wp-content/uploads/Apps/Libraries/Chronoir_net.SPADA_1.0.0.0.zip)  

**Chronir_net.SPADA.dll**をアセンブリ参照に追加するか、**Chronir_net.SPADA.cs**をプロジェクトに追加します。

##◆ 使用方法

ライブラリのドキュメントはこちらです -> [SPADA: Main Page](http://chronoir.net/wp-content/uploads/contents/documents/libraries/SPADA)  

すぱーダは すぱこーRSSフィードを読み込む`SpacoRSSReader`クラスと
すぱこーの各話データを格納する`SpacoRSSItem`クラスの2つで構成されています。

いずれも名前空間は`Chronoir_net.SPADA`です。

```csharp
using Chronoir_net.SPADA;
```
##◆ すぱこーRSSフィードの読み込み

`SpacoRSSReader.Load( string )`メソッドで、
すぱこーRSSフィードのURLを指定して読み込みます（内部でLINQ to XMLを用いています）。

または、`SpacoRSSReader.Load( XmlReader )`メソッドで、
XmlReaderオブジェクトを指定して読み込むことも可能です（こちらの方が高速です）。

読み込みに成功すると、コンテンツのデータを格納した`SpacoRSSReader`オブジェクトが返ります。

```csharp
SpacoRSSReader srr = SpacoRSSReader.Load( "http://pronama-api.azurewebsites.net/feed/spaco" );
...
```

または

```csharp
using System.Xml;

using( XmlReader xr = XmlReader.Create( "http://pronama-api.azurewebsites.net/feed/spaco" ) ) {
	SpacoRSSReader srr = SpacoRSSReader.Load( xr );
	...
}
```

##◆ データの取り出し

`SpacoRSSReader`オブジェクトのプロパティからデータを取り出します。

※型の標記のないものは全て`string`型です。

* Title       : 作品のタイトル
* Link        : リンク
* Description : 作品の説明
* PubDate     : 最新話更新日（ `DateTime`型 ）
* BannerURL   : バナー画像のURL
* Author      : 作成者名

* Items       : 各話のデータのコレクション（ `IEnumerable<SpacoRSSItem>`型 ）

##◆ 各話のデータの列挙

`SpacoRSSReader.Items`プロパティからLINQを使って、各話のデータを列挙することができます。

```csharp
// 単純な列挙
foreach( SpacoRSSItem item in srr.Items ) {
	...
}

// 今日から起算して過去2ヶ月分のみを抽出
var srr_rc2 = srr.Items.Where( _ => _.PubDate >= DateTime.Now.AddMonths( -2 ) );
```

##◆ 各話のデータの取り出し

`SpacoRSSItem`オブジェクトのプロパティからデータを取り出します。

※型の標記のないものは全て`string`型です。

* Title        : タイトル
* Author       : 作成者名
* Link         : リンク
* PubDate      : 投稿日（ `DateTime`型 ）
* Description  : 作品の説明
* Volume       : 作品の話数（ `int`型 ）
* ModifiedDate : 更新日（ `DateTime`型 ）
* IsAvailable  : 利用可能（ `bool`型 ）
* MediaURL     : 漫画の画像
* ThumbnailURL : サムネイル画像のURL
* ID           : ID


##◆ サンプル

```csharp
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

using Chronoir_net.SPADA;

class Program {

	static void Main( string[] args ) {
		try {
			// すぱーダで、すぱこーRSSフィードの読み込みます。
			SpacoRSSReader srr = null;
			// XMLReaderクラス経由ですぱこーRSSフィードを読み込みます。
			using( XmlReader xr = XmlReader.Create( "http://pronama-api.azurewebsites.net/feed/spaco" ) ) {
				srr = SpacoRSSReader.Load( xr );
			}
			Console.WriteLine( "完了！" );

			// すぱこーRSSフィードのチャンネル情報を取得します。
			Console.WriteLine( $"タイトル : {srr.Title}" );
			Console.WriteLine( $"概要 : {srr.Description}" );
			Console.WriteLine( $"最新話公開日 : {srr.PubDate}" );
			Console.WriteLine( $"リンク : {srr.Link}" );

			// 各話のデータを取得します。
			Console.WriteLine( "\n各話の情報" );
			foreach( var item in srr.Items ) {
				Console.WriteLine( $"◆ 第{item.Volume}話" );
				Console.WriteLine( $"　タイトル : {item.Title}" );
				Console.WriteLine( $"　作者 : {item.Author}" );
				Console.WriteLine( $"　公開日 : {item.PubDate}" );
				Console.WriteLine( $"　リンク : {item.Link}" );
			}

		}
		catch( Exception ex ) {
			Console.WriteLine( $"\nエラー : {ex.Message}" );
		}
	}
}
```

##◆ 免責条項

このライブラリを使用しことにより生じたいかなるトラブル・損害において、
作成者及びChronoir.netは一切の責任を負いかねます。あらかじめご了承ください。


##◆ リリースノート

* Ver. 1.0.0 : 2015/12/27  
　 - CNR-00000 : 初版リリース

