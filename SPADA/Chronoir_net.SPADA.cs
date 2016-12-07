#region バージョン情報
/**
*	@file
*	@brief すぱこーRSSリーダー（すぱーダ（SPADA））。
*	
*	すぱーダ（SPADA）は、プログラミング生放送のすぱこーRSSフィード
*	（ http://pronama.azurewebsites.net/spaco-feed/ ）を簡単に読み込むことができるライブラリです。
*
*	@対応プラットフォーム
*	- .NET Framework 4.0 以上
*	@par バージョン Version
*	1.0.6
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2014-2016 Chronoir.net
*	@par 作成日
*	2015/12/27
*	@par 最終更新日
*	2016/12/08
*	@par ライセンス Licence
*	MIT Licence
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*	- http://chronoir.net/spada （すぱーダのページ）
*	- https://github.com/Nia-TN1012/SPADA （GitHubのリポジトリ）
*	- https://www.nuget.org/packages/Chronoir_net.SPADA/ （NuGet Gallery）
*	@par リリースノート Release note
*	- 2016/12/08 Ver. 1.0.6
*		- CNR-00003 : SpacoRSSItemクラスのコンストラクターを強化しました。
*	- 2016/01/30 Ver. 1.0.5
*		- CNR-00001 : SpacoRSSReader.Load( XmlReader )メソッドにおいて、読み込み後にXmlReaderのカーソル位置を最後まで移動する処理を廃止しました。
*		- CNR-00002 : SpacoRSSReaderのチャンネル情報の値を外部から設定できるように変更しました。
*	- 2015/12/27 Ver. 1.0.0
*		- CNR-00000 : 初版リリース
*/
#endregion

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Chronoir_net {

	/// <summary>
	///		すぱこーRSSフィードを取得するモジュール、
	///		すぱこーRSSリーダー、略して「すぱーダ」（SPADA）です。
	/// </summary>
	namespace SPADA {
		/// <summary>
		///		すぱこーの各話のデータを表します。
		/// </summary>
		public class SpacoRSSItem {
			/// <summary>
			///		タイトルを取得・設定します。
			/// </summary>
			public string Title { get; set; }
			/// <summary>
			///		作者名を取得・設定します。
			/// </summary>
			public string Author { get; set; }
			/// <summary>
			///		作品のリンクを取得・設定します。
			/// </summary>
			public string Link { get; set; }
			/// <summary>
			///		公開日を取得・設定します。
			/// </summary>
			public DateTime PubDate { get; set; }
			/// <summary>
			///		作品のあらすじを取得・設定します。
			/// </summary>
			public string Description { get; set; }
			/// <summary>
			///		作品の話数を取得・設定します。
			/// </summary>
			public int Volume { get; set; }
			/// <summary>
			///		更新日を取得・設定します。
			/// </summary>
			public DateTime ModifiedDate { get; set; }
			/// <summary>
			///		利用可能かどうかを取得・設定します。
			/// </summary>
			/// <value>true : 利用可能 / false : 利用不可能</value>
			public bool IsAvailable { get; set; }
			/// <summary>
			///		漫画の画像のURLを取得・設定します。
			/// </summary>
			public string MediaURL { get; set; }
			/// <summary>
			///		サムネイル画像のURLを取得・設定します。
			/// </summary>
			public string ThumbnailURL { get; set; }
			/// <summary>
			///		IDを取得・設定します。
			/// </summary>
			public string ID { get; set; }

			/// <summary>
			///		指定したすぱこーRSSフィードの各要素の値から、<see cref="SpacoRSSItem"/>クラスの新しいインスタンスを生成します。
			/// </summary>
			/// <param name="title">タイトル</param>
			/// <param name="author">作者名</param>
			/// <param name="link">作品のリンク</param>
			/// <param name="pubDate">公開日</param>
			/// <param name="description">作品のあらすじ</param>
			/// <param name="volume">作品の話数</param>
			/// <param name="modifiedDate">更新日</param>
			/// <param name="isAvailable">利用可能かどうかを表す値</param>
			/// <param name="mediaURL">漫画の画像のURL</param>
			/// <param name="thumbnailURL">サムネイル画像のURL</param>
			/// <param name="id">ID</param>
			public SpacoRSSItem(
				 string title = default( string ), string author = default( string ), string link = default( string ), DateTime pubDate = default( DateTime ),
				 string description = default( string ), int volume = default( int ), DateTime modifiedDate = default( DateTime ), bool isAvailable = default( bool ),
				 string mediaURL = default( string ), string thumbnailURL = default( string ), string id = default( string ) ) {
				Title = title;
				Author = author;
				Link = link;
				PubDate = pubDate;
				Description = description;
				Volume = volume;
				ModifiedDate = modifiedDate;
				IsAvailable = isAvailable;
				MediaURL = mediaURL;
				ThumbnailURL = thumbnailURL;
				ID = id;
			}

			/// <summary>
			///		指定した<see cref="SpacoRSSItem"/>オブジェクトから、<see cref="SpacoRSSItem"/>の新しいインスタンスを生成します。
			/// </summary>
			/// <param name="item">各メンバーにセットする<see cref="SpacoRSSItem"/></param>
			/// <remarks>派生クラスのコンストラクターで使用します。</remarks>
			protected SpacoRSSItem( SpacoRSSItem item ) {
				new SpacoRSSItem(
					title: item.Title, author: item.Author, link: item.Link, pubDate: item.PubDate,
					description: item.Description, volume:  item.Volume, modifiedDate: item.ModifiedDate, isAvailable: item.IsAvailable,
					mediaURL: item.MediaURL, thumbnailURL: item.ThumbnailURL, id: item.ID
				);
			}
		}

		/// <summary>
		///		すぱこーRSSフィードを取得するクラスです。
		/// </summary>
		public class SpacoRSSReader {

			#region チャンネル情報

			/// <summary>
			///		作品タイトルを取得・設定します。
			/// </summary>
			public string Title { get; set; }
			/// <summary>
			///		リンクを取得・設定します。
			/// </summary>
			public string Link { get; set; }
			/// <summary>
			///		作品の説明を取得・設定します。
			/// </summary>
			public string Description { get; set; }
			/// <summary>
			///		最新話更新日を取得・設定します。
			/// </summary>
			public DateTime PubDate { get; set; }
			/// <summary>
			///		バナー画像のURLを取得・設定します。
			/// </summary>
			public string BannerURL { get; set; }
			/// <summary>
			///		作者名を取得・設定します。
			/// </summary>
			public string Author { get; set; }

			#endregion

			/// <summary>
			///		すぱこーの各話を格納します
			/// </summary>
			private List<SpacoRSSItem> items;

			/// <summary>
			///		すぱこーの各話のデータコレクションを取得します。
			/// </summary>
			public IEnumerable<SpacoRSSItem> Items => items;

			// 名前空間を表します
			private static XNamespace p = @"http://pinga.mu/terms/";
			private static XNamespace media = @"http://search.yahoo.com/mrss/";
			private static XNamespace dcndl = @"http://ndl.go.jp/dcndl/terms/";
			private static XNamespace dc = @"http://purl.org/dc/elements/1.1/";

			/// <summary>
			///		<see cref="SpacoRSSReader"/>クラスの新しいインスタンスを生成します。
			/// </summary>
			public SpacoRSSReader() {
				items = new List<SpacoRSSItem>();
			}

			#region 同期読み込み

			/// <summary>
			///		指定したURLからすぱこーRSSフィードを読み込み、<see cref="SpacoRSSReader"/>オブジェクトを生成します。
			/// </summary>
			/// <param name="uri">すぱこーRSSフィードのURL</param>
			/// <returns>すぱこーRSSフィードのデータを格納した、<see cref="SpacoRSSReader"/>オブジェクト</returns>
			public static SpacoRSSReader Load( string uri ) {
				SpacoRSSReader srr = new SpacoRSSReader();

				// Linq to XMLですぱこーRSSフィードの読み込みます。
				XElement spx = XElement.Load( uri );

				// すぱこーのチャンネル情報を取得します。
				XElement spxChannel = spx.Element( "channel" );
				srr.Title = spxChannel.Element( "title" ).Value;
				srr.Link = spxChannel.Element( "link" ).Value;
				srr.Description = spxChannel.Element( "description" ).Value;
				srr.PubDate = DateTime.Parse( spxChannel.Element( "pubDate" ).Value );
				srr.BannerURL = spxChannel.Element( "image" ).Value;
				srr.Author = spxChannel.Element( dc + "creator" ).Value;

				// すぱこーの各話のデータを取得します。
				var spxItems = spxChannel.Elements( "item" );
				foreach( var item in spxItems ) {
					SpacoRSSItem si = new SpacoRSSItem(
						title: item.Element( "title" ).Value,
						author: item.Element( dc + "creator" ).Value,
						link: item.Element( "link" ).Value,
						pubDate: DateTime.Parse( item.Element( "pubDate" ).Value ),
						description: item.Element( "description" ).Value,
						volume: int.Parse( item.Element( dcndl + "volume" ).Value ),
						modifiedDate: DateTime.Parse( item.Element( dc + "modified" ).Value ),
						isAvailable: bool.Parse( item.Element( p + "isAvailable" ).Value ),
						mediaURL: item.Element( media + "content" ).Attribute( "url" ).Value,
						thumbnailURL: item.Element( media + "thumbnail" ).Attribute( "url" ).Value,
						id: item.Element( "guid" ).Value
					);
					srr.items.Add( si );
				}

				return srr;
			}

			/// <summary>
			///		指定したXmlReaderからすぱこーRSSフィードを読み込み、<see cref="SpacoRSSReader"/>オブジェクトを生成します。
			/// </summary>
			/// <param name="reader">すぱこーRSSフィードを読み込む<see cref="XmlReader"/>オブジェクト</param>
			/// <returns>すぱこーRSSフィードのデータを格納した、<see cref="SpacoRSSReader"/>オブジェクト</returns>
			public static SpacoRSSReader Load( XmlReader reader ) {
				SpacoRSSReader srr = new SpacoRSSReader();

				reader.Read();
				reader.ReadStartElement( "rss" );
				reader.ReadStartElement( "channel" );

				// すぱこーのチャンネル情報と各話のデータを取得します。
				while( reader.Read() ) {
					if( reader.NodeType == XmlNodeType.Element ) {
						// 各要素名に対応したプロパティに値を代入します。
						switch( reader.Name ) {
							case "title":
								reader.ReadStartElement( "title" );
								srr.Title = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "link":
								reader.ReadStartElement( "link" );
								srr.Link = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "description":
								reader.ReadStartElement( "description" );
								srr.Description = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "pubDate":
								reader.ReadStartElement( "pubDate" );
								srr.PubDate = DateTime.Parse( reader.ReadContentAsString() );
								reader.ReadEndElement();
								break;
							case "image":
								reader.ReadStartElement( "image" );
								srr.BannerURL = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "dc:creator":
								reader.ReadStartElement( "dc:creator" );
								srr.Author = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "item":
								srr.items.Add( LoadItem( reader ) );
								break;
							default:
								break;
						}
					}
					// channel内の要素をすべて読み終えたら、このwhile文から脱出します。
					else if( reader.NodeType == XmlNodeType.EndElement && reader.Name == "channel" ) {
						reader.ReadEndElement();
						break;
					}
				}

				return srr;
			}

			/// <summary>
			///		すぱこーの1話分のデータを取得します。
			/// </summary>
			/// <param name="reader">すぱこーRSSフィードを読み込む<see cref="XmlReader"/>オブジェクト（現在位置がitem要素）</param>
			/// <returns>すぱこー1話分のデータを格納した、<see cref="SpacoRSSItem"/>オブジェクト</returns>
			/// <remarks><see cref="SpacoRSSReader.Load(XmlReader)"/>から呼び出します。</remarks>
			/// <seealso cref="Load(XmlReader)"/>
			private static SpacoRSSItem LoadItem( XmlReader reader ) {
				SpacoRSSItem sri = new SpacoRSSItem();
				reader.ReadStartElement( "item" );

				while( reader.Read() ) {

					if( reader.NodeType == XmlNodeType.Element ) {
						// 各要素名に対応したプロパティに値を代入します。
						switch( reader.Name ) {
							case "title":
								reader.ReadStartElement( "title" );
								sri.Title = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "dc:creator":
								reader.ReadStartElement( "dc:creator" );
								sri.Author = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "link":
								reader.ReadStartElement( "link" );
								sri.Link = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "pubDate":
								reader.ReadStartElement( "pubDate" );
								sri.PubDate = DateTime.Parse( reader.ReadContentAsString() );
								reader.ReadEndElement();
								break;
							case "description":
								reader.ReadStartElement( "description" );
								sri.Description = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "dcndl:volume":
								reader.ReadStartElement( "dcndl:volume" );
								sri.Volume = int.Parse( reader.ReadContentAsString() );
								reader.ReadEndElement();
								break;
							case "dc:modified":
								reader.ReadStartElement( "dc:modified" );
								sri.ModifiedDate = DateTime.Parse( reader.ReadContentAsString() );
								reader.ReadEndElement();
								break;
							case "p:isAvailable":
								reader.ReadStartElement( "p:isAvailable" );
								sri.IsAvailable = bool.Parse( reader.ReadContentAsString() );
								reader.ReadEndElement();
								break;
							case "media:content":
								reader.MoveToAttribute( "url" );
								sri.MediaURL = reader.ReadContentAsString();
								reader.MoveToElement();
								reader.Skip();
								break;
							case "media:thumbnail":
								reader.MoveToAttribute( "url" );
								sri.ThumbnailURL = reader.ReadContentAsString();
								reader.MoveToElement();
								reader.Skip();
								break;
							case "guid":
								reader.ReadStartElement( "guid" );
								sri.ID = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							default:
								break;
						}
					}
					// item内の要素をすべて読み終えたら、このwhile文から脱出します。
					else if( reader.NodeType == XmlNodeType.EndElement && reader.Name == "item" ) {
						reader.ReadEndElement();
						break;
					}
				}

				return sri;
			}

			#endregion
		}
	}
}