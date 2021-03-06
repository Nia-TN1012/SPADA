﻿#region バージョン情報
/**
*	@file
*	@brief すぱこーRSSリーダー マルチプラットフォーム（すぱーダS（SPADAS））。
*	
*	すぱーダ（SPADA）は、プログラミング生放送のすぱこーRSSフィード
*	（ http://pronama.azurewebsites.net/spaco-feed/ ）を簡単に読み込むことができるライブラリです。
*
*	@対応プラットフォーム
*	- (.NET Standard 2.0に対応する.NETアプリで利用できます。)
*	- .NET Core 2.0 以上
*	- .NET Framework 4.6.1 以上 (バージョンによっては、NETStandard.Libraryが別途必要です。)
*	- ユニバーサルWindowsプラットフォーム 10.0.16299 以上
*	- Mono 5.4 以上
*	- Xamarin.Mac 3.8以上
*	- Xamarin.Android 8.0以上
*	- Xamarin.iOS 10.14以上
*	@par バージョン Version
*	1.0.0
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2014-2019 Chronoir.net
*	@par 作成日
*	2019/06/16
*	@par 最終更新日
*	2019/06/16
*	@par ライセンス Licence
*	MIT Licence
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*	- http://chronoir.net/spada （すぱーダのページ）
*	- https://github.com/Nia-TN1012/SPADA （GitHubのリポジトリ）
*	- https://www.nuget.org/packages/Chronoir_net.SPADAS/ （NuGet Gallery）
*	@par リリースノート Release note
*	- 2019/06/16 Ver. 1.0.0
*		- CNR-00000: 初版リリース
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

/// <summary>
///		すぱこーRSSフィードを取得するモジュール、
///		すぱこーRSSリーダー、略して「すぱーダ」（SPADA）です。
///		すぱーダS（SPADAS）はマルチプラットフォーム向けのすぱーダです。
/// </summary>
namespace Chronoir_net.SPADAS {
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
		private List<SpacoRSSItem> items = new List<SpacoRSSItem>();

		/// <summary>
		///		すぱこーの各話のデータコレクションを取得します。
		/// </summary>
		public IEnumerable<SpacoRSSItem> Items => items;

		// 名前空間を表します
		private static readonly XNamespace p = @"http://pinga.mu/terms/";
		private static readonly XNamespace media = @"http://search.yahoo.com/mrss/";
		private static readonly XNamespace dcndl = @"http://ndl.go.jp/dcndl/terms/";
		private static readonly XNamespace dc = @"http://purl.org/dc/elements/1.1/";

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
		///		指定した<see cref="XmlReader"/>オブジェクトからすぱこーRSSフィードを読み込み、<see cref="SpacoRSSReader"/>オブジェクトを生成します。
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
		/// <remarks><see cref="Load(XmlReader)"/>から呼び出します。</remarks>
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
		///		指定したURLからすぱこーRSSフィードを読み込み、<see cref="SpacoRSSReader"/>オブジェクトを生成します。
		/// </summary>
		/// <param name="uri">すぱこーRSSフィードのURL</param>
		/// <param name="cancellationToken">読み込みを中止するためのトークン</param>
		/// <returns>すぱこーRSSフィードのデータを格納した、<see cref="SpacoRSSReader"/>オブジェクト</returns>
		/// <exception cref="OperationCanceledException">読み込み中止を要求された時</exception>
		/// <remarks><paramref name="cancellationToken"/>がnullの場合、読み込みを中止することができません。</remarks>
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

			return Task.FromResult( srr );
		}

		/// <summary>
		///		指定した<see cref="XmlReader"/>からすぱこーRSSフィードを読み込み、<see cref="SpacoRSSReader"/>オブジェクトを生成します。
		/// </summary>
		/// <param name="reader">すぱこーRSSフィードを読み込むXmlReaderオブジェクト</param>
		/// <param name="cancellationToken">読み込みを中止するためのトークン</param>
		/// <returns>すぱこーRSSフィードのデータを格納した、<see cref="SpacoRSSReader"/>オブジェクト</returns>
		/// <exception cref="OperationCanceledException">読み込み中止を要求された時</exception>
		/// <remarks><paramref name="cancellationToken"/>がnullの場合、読み込みを中止することができません。</remarks>
		public static Task<SpacoRSSReader> LoadAsync( XmlReader reader, CancellationToken? cancellationToken = null ) {
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
							srr.items.Add( LoadItem( reader, cancellationToken ) );
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

			return Task.FromResult( srr );
		}

		/// <summary>
		///		すぱこーの1話分のデータを取得します。
		/// </summary>
		/// <param name="reader">すぱこーRSSフィードを読み込む<see cref="XmlReader"/>オブジェクト（現在位置がitem要素）</param>
		/// <param name="cancellationToken">読み込みを中止するためのトークン</param>
		/// <returns>すぱこー1話分のデータを格納した、<see cref="SpacoRSSItem"/>オブジェクト</returns>
		/// <exception cref="OperationCanceledException">読み込み中止を要求された時</exception>
		/// <remarks><see cref="LoadItem(XmlReader, CancellationToken?)"/>から呼び出します。<paramref name="cancellationToken"/>がnullの場合、読み込みを中止することができません。</remarks>
		/// <seealso cref="LoadAsync( XmlReader, CancellationToken? )"/>
		private static SpacoRSSItem LoadItem( XmlReader reader, CancellationToken? cancellationToken ) {
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

			return sri;
		}

		#endregion
	}
}
