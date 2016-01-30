# すぱーダ（SPADA）

---

名称 : すぱーダ（SPADA）  
バージョン : Ver. 1.0.5 ( Jan. 30, 2016 )  
  
種別 : .NET クラスライブラリ  
  
ターゲットプラットフォーム :  
* **すぱーダ（SPADA）** : .NET Framework 4.0  
* **エクすぱーダ（XSPADA）** : .NET Framework 4.5、Xamarin.Android、Xamarin.iOS、Windows 8.x用ストアアプリ、Windows Phone 8.1（Silverlightには非対応）   
* **ユニすぱーダ（UniSPADA）** : ユニバーサルWindows（Build 10240以上） 
  
作成者 : 智中 ニア（ [@nia_tn1012](https://twitter.com/nia_tn1012/) ）  
ライセンス : MIT Licence  
  
リンク  
　http://chronoir.net （ホームページ）  
　http://chronoir.net/spada （ライブラリのページ）  
　https://github.com/Nia-TN1012/SPADA （GitHubのリポジトリ）  
  
NuGet Gallery  
* すぱーダ（SPADA）: https://www.nuget.org/packages/Chronir_net.SPADA/
* エクすぱーダ（XSPADA）: https://www.nuget.org/packages/Chronoir_net.XSPADA/
* ユニすぱーダ（UniSPADA）: https://www.nuget.org/packages/Chronoir_net.UniSPADA/ 

---

##◆ はじめに

ダウンロードしていただき、ありがとうございます。  
すぱーダ（SPADA）は、プログラミング生放送のすぱこーRSSフィード
（ http://pronama.azurewebsites.net/spaco-feed/ ）を簡単に読み込むことができるライブラリです。

##◆ ダウンロードとインストール方法

以下のリンクからzipファイルをダウンロードし、解凍します。  
* [Chronoir_net.SPADA.zip](http://chronoir.net/wp-content/uploads/Apps/Libraries/Chronoir_net.SPADA.zip)
* [Chronoir_net.XSPADA.zip](http://chronoir.net/wp-content/uploads/Apps/Libraries/Chronoir_net.XSPADA.zip)
* [Chronoir_net.UniSPADA.zip](http://chronoir.net/wp-content/uploads/Apps/Libraries/Chronoir_net.UniSPADA.zip)

DLLファイルをアセンブリ参照に追加するか、プロジェクトに追加します。

##◆ 使用方法

ライブラリのドキュメントはこちらです -> [SPADA: Main Page](http://chronoir.net/wp-content/uploads/contents/documents/libraries/SPADA)  

すぱーダは すぱこーRSSフィードを読み込む`SpacoRSSReader`クラスと
すぱこーの各話データを格納する`SpacoRSSItem`クラスの2つで構成されています。

いずれも名前空間は`Chronoir_net.SPADA`です。  
※エクすぱーダは`Chronoir_net.XSPADA`、ユニすぱーダは`Chronoir_net.UniSPADA`です。

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

エクすぱーダ及びユニすぱーダでは、非同期で読み込むための`SpacoRSSReaderAsync.Load( string, CancellationToken? )`メソッドと`SpacoRSSReader.LoadAsync( XmlReader, CancellationToken? )`メソッドを
用意しています。また、WebからXmlReaderオブジェクトを生成する`SpacoRSSClient.GetXmlReaderAsync( string, CancellationToken? )`メソッドも用意しています。  
これらのメソッドは、CancellationTokenを使って処理を中止させることができます。

```csharp
using System.Threading;
using System.Xml;

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

using( XmlReader reader = await Task.Run( () => SpacoRSSClient.GetXmlReaderAsync( "http://pronama-api.azurewebsites.net/feed/spaco", cancellationTokenSource.Token ) ) ) {
	SpacoRSSReader srr = await Task.Run( () => SpacoRSSReader.LoadAsync( reader, cancellationTokenSource.Token ) );
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
* Ver. 1.0.5 : 2016/01/30  
　 - すぱーダの動作改善、エクすぱーダとユニすぱーダを追加

* Ver. 1.0.0 : 2015/12/27  
　 - 初版リリース

