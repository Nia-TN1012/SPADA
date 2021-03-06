﻿#region バージョン情報
/**
*	@file 
*	@brief HTTPクライアント for ユニすぱーダ（Uni-SPADA）
*	
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
*	@note リリースノートはエクすぱーダを参照してください。
*/
#endregion
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Web.Http;

namespace Chronoir_net {

	namespace UniSPADA {

		/// <summary>
		///		WebからすぱこーRSSフィードを取得する機能を提供します。
		/// </summary>
		public static class SpacoRSSClient {

			/// <summary>
			///		指定したURLからXMLReaderオブジェクトを生成します。
			/// </summary>
			/// <param name="url">XMLのURL</param>
			/// <param name="cancellationToken">処理を中止するためのトークン</param>
			/// <returns>XMLを格納したXMLReaderオブジェクト</returns>
			/// <remarks>cancellationTokenがnullの場合、処理を中止することができません。</remarks>
			public static Task<XmlReader> GetXmlReaderAsync( string url, CancellationToken? cancellationToken = null ) {

				// コンテンツの文字列を可能するための文字列
				string responseString = null;

				// HttpClientオブジェクトを生成します。
				using( HttpClient client = new HttpClient() ) {
					// GETリクエストを送信します。
					// UWP用は
					var task = client.GetAsync( new Uri( url ) ).AsTask();
					// レスポンスが返るまで待機します。
					// ※cancellationTokenがnullの場合、ダミーとしてCancellationToken.Noneを指定します。
					task.Wait( cancellationToken ?? CancellationToken.None );

					// レスポンスを格納します。
					using( var message = task.Result ) {
						// レスポンスから文字列を取得します。
						var response = task.Result.Content.ReadAsStringAsync().AsTask();
						// 待機します。
						response.Wait( cancellationToken ?? CancellationToken.None );
						// 文字列を格納します。
						responseString = response.Result;
					}
				}

				// 文字列（XML）からXmlReaderオブジェクトを生成します。
				return Task.FromResult( XmlReader.Create( new StringReader( responseString ) ) );
			}

		}
	}
}
