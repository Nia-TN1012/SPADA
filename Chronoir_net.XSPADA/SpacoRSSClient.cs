using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Chronoir_net.XSPADA {

	public static class SpacoRSSClient {

		/// <summary>
		///		指定したURLからXMLReaderオブジェクトを生成します。
		/// </summary>
		/// <param name="url">XMLのURL</param>
		/// <returns>XMLを格納したXMLReaderオブジェクト</returns>
		public static Task<XmlReader> GetXmlReaderAsync( string url, CancellationToken? cancellationToken ) {

			// コンテンツの文字列を可能するための文字列
			string responseString = null;

			// HttpClientオブジェクトを生成します。
			using( HttpClient client = new HttpClient() ) {
				// GETリクエストを送信します。
				var task = client.GetAsync( new Uri( url ) );
				// レスポンスが返るまで待機します。
				// ※cancellationTokenがnullの時は、ダミーのCancellationTokenを指定します。
				task.Wait( cancellationToken ?? new CancellationToken() );

				// レスポンスを格納します。
				using( var message = task.Result ) {
					// レスポンスから文字列を取得します。
					var response = task.Result.Content.ReadAsStringAsync();
					// 待機します。
					response.Wait( cancellationToken ?? new CancellationToken() );
					// 文字列を格納します。
					responseString = response.Result;
				}
			}

			// 文字列（XML）からXmlReaderオブジェクトを生成します。
			return Task.FromResult( XmlReader.Create( new StringReader( responseString ) ) );
		}

	}

}
