#region バージョン情報
/**
*	@file
*	@brief すぱこーRSSリーダー クロスプラットフォーム（エクすぱーダ（X-SPADA））。
*	
*	すぱーダ（SPADA）は、プログラミング生放送のすぱこーRSSフィード
*	（ http://pronama.azurewebsites.net/spaco-feed/ ）を簡単に読み込むことができるライブラリです。
*
*	@対応プラットフォーム
*	- .NET Framework 4.5 以上
*	- Windows 8.x 用ストアアプリ
*	- ASP.NET Core 5.0
*	- Windows Phone 8.1（Sliverlightには非対応です。）
*	- Xamarin.Android
*	- Xamarin.iOS（Classicも含む）
*	@par バージョン Version
*	1.0.1
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2014-2016 Chronoir.net
*	@par 作成日
*	2016/01/30
*	@par 最終更新日
*	2016/02/03
*	@par ライセンス Licence
*	MIT Licence
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*	- http://chronoir.net/spada （すぱーダのページ）
*	- https://github.com/Nia-TN1012/SPADA （GitHubのリポジトリ）
*	- https://www.nuget.org/packages/Chronoir_net.XSPADA/ （NuGet Gallery）
*	@par リリースノート Release note
*	- 2016/02/03 Ver. 1.0.1
*		- CNR-00001 : LoadAsync、GetXmlReaderAsyncメソッドにおいて、CancellationTokenにデフォルト引数（null）を設定しました。
*	- 2016/01/30 Ver. 1.0.0
*		- CNR-00000 : 初版リリース
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Chronoir_net {

	/// <summary>
	///		すぱこーRSSフィードを取得するモジュール、
	///		すぱこーRSSリーダー、略して「すぱーダ」（SPADA）です。
	///		エクすぱーダ（X-SPADA）は、すぱーダ（オリジナル）を
	///		クロスプラットフォームに対応させたライブラリです。
	///		・Windows（ デスクトップ / ウェブ / ストアアプリ / Phone 8.1 ）
	///		・ASP.NET Core
	///		・Xamarin.Android、Xamarin.iOS
	///		など幅広いターゲットに対応しているのが特徴です。
	/// </summary>
	namespace XSPADA {
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
			///		利用可能可能かどうかを取得・設定します。
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
			///		SpacoRSSReaderクラスの新しいインスタンスを生成します。
			/// </summary>
			public SpacoRSSReader() {
				items = new List<SpacoRSSItem>();
			}

			#region 同期読み込み

			/// <summary>
			///		指定したURLからすぱこーRSSフィードを読み込み、SpacoRSSReaderオブジェクトを生成します。
			/// </summary>
			/// <param name="uri">すぱこーRSSフィードのURL</param>
			/// <returns>すぱこーRSSフィードのデータを格納した、SpacoRSSReaderオブジェクト</returns>
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
					SpacoRSSItem si = new SpacoRSSItem {
						Title = item.Element( "title" ).Value,
						Author = item.Element( dc + "creator" ).Value,
						Link = item.Element( "link" ).Value,
						PubDate = DateTime.Parse( item.Element( "pubDate" ).Value ),
						Description = item.Element( "description" ).Value,
						Volume = int.Parse( item.Element( dcndl + "volume" ).Value ),
						ModifiedDate = DateTime.Parse( item.Element( dc + "modified" ).Value ),
						IsAvailable = bool.Parse( item.Element( p + "isAvailable" ).Value ),
						MediaURL = item.Element( media + "content" ).Attribute( "url" ).Value,
						ThumbnailURL = item.Element( media + "thumbnail" ).Attribute( "url" ).Value,
						ID = item.Element( "guid" ).Value
					};
					srr.items.Add( si );
				}

				return srr;
			}

			/// <summary>
			///		指定したXmlReaderからすぱこーRSSフィードを読み込み、SpacoRSSReaderオブジェクトを生成します。
			/// </summary>
			/// <param name="reader">すぱこーRSSフィードを読み込むXmlReaderオブジェクト</param>
			/// <returns>すぱこーRSSフィードのデータを格納した、SpacoRSSReaderオブジェクト</returns>
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
			/// <param name="reader">すぱこーRSSフィードを読み込むXmlReaderオブジェクト（現在位置がitem要素）</param>
			/// <returns>すぱこー1話分のデータを格納した、SpacoRSSItemオブジェクト</returns>
			/// <remarks>SpacoRSSReader.Load( XmlReader )から呼び出します。</remarks>
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

			#region 非同期読み込み

			/// <summary>
			///		指定したURLからすぱこーRSSフィードを読み込み、SpacoRSSReaderオブジェクトを生成します。
			/// </summary>
			/// <param name="uri">すぱこーRSSフィードのURL</param>
			/// <param name="cancellationToken">読み込みを中止するためのトークン</param>
			/// <returns>すぱこーRSSフィードのデータを格納した、SpacoRSSReaderオブジェクト</returns>
			/// <exception cref="OperationCanceledException">読み込み中止を要求された時</exception>
			/// <remarks>cancellationTokenがnullの場合、読み込みを中止することができません。</remarks>
			public static Task<SpacoRSSReader> LoadAsync( string uri, CancellationToken? cancellationToken = null ) {
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

					// cancellationTokenが有効かつ、キャンセル通知が発生した場合、OperationCanceledExceptionをスローします。
					cancellationToken?.ThrowIfCancellationRequested();

					SpacoRSSItem si = new SpacoRSSItem {
						Title = item.Element( "title" ).Value,
						Author = item.Element( dc + "creator" ).Value,
						Link = item.Element( "link" ).Value,
						PubDate = DateTime.Parse( item.Element( "pubDate" ).Value ),
						Description = item.Element( "description" ).Value,
						Volume = int.Parse( item.Element( dcndl + "volume" ).Value ),
						ModifiedDate = DateTime.Parse( item.Element( dc + "modified" ).Value ),
						IsAvailable = bool.Parse( item.Element( p + "isAvailable" ).Value ),
						MediaURL = item.Element( media + "content" ).Attribute( "url" ).Value,
						ThumbnailURL = item.Element( media + "thumbnail" ).Attribute( "url" ).Value,
						ID = item.Element( "guid" ).Value
					};
					srr.items.Add( si );
				}

				return Task.FromResult( srr );
			}

			/// <summary>
			///		指定したXmlReaderからすぱこーRSSフィードを読み込み、SpacoRSSReaderオブジェクトを生成します。
			/// </summary>
			/// <param name="reader">すぱこーRSSフィードを読み込むXmlReaderオブジェクト</param>
			/// <param name="cancellationToken">読み込みを中止するためのトークン</param>
			/// <returns>すぱこーRSSフィードのデータを格納した、SpacoRSSReaderオブジェクト</returns>
			/// <exception cref="OperationCanceledException">読み込み中止を要求された時</exception>
			/// <remarks>cancellationTokenがnullの場合、読み込みを中止することができません。</remarks>
			public static async Task<SpacoRSSReader> LoadAsync( XmlReader reader, CancellationToken? cancellationToken = null ) {
				SpacoRSSReader srr = new SpacoRSSReader();

				reader.Read();
				reader.ReadStartElement( "rss" );
				reader.ReadStartElement( "channel" );

				// すぱこーのチャンネル情報と各話のデータを取得します。
				while( reader.Read() ) {

					cancellationToken?.ThrowIfCancellationRequested();

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
								srr.items.Add( await LoadItemAsync( reader, cancellationToken ) );
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
			/// <param name="reader">すぱこーRSSフィードを読み込むXmlReaderオブジェクト（現在位置がitem要素）</param>
			/// <param name="cancellationToken">読み込みを中止するためのトークン</param>
			/// <returns>すぱこー1話分のデータを格納した、SpacoRSSItemオブジェクト</returns>
			/// <exception cref="OperationCanceledException">読み込み中止を要求された時</exception>
			/// <remarks>SpacoRSSReader.LoadAsync( XmlReader )から呼び出します。cancellationTokenがnullの場合、読み込みを中止することができません。</remarks>
			/// <seealso cref="LoadAsync( XmlReader, CancellationToken? )"/>
			private static Task<SpacoRSSItem> LoadItemAsync( XmlReader reader, CancellationToken? cancellationToken = null ) {
				SpacoRSSItem sri = new SpacoRSSItem();
				reader.ReadStartElement( "item" );

				while( reader.Read() ) {

					cancellationToken?.ThrowIfCancellationRequested();

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

				return Task.FromResult( sri );
			}

			#endregion
		}
	}
}