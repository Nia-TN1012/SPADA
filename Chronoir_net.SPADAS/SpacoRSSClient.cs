using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Chronoir_net.SPADAS {

	/// <summary>
	///		WebからすぱこーRSSフィードを取得する機能を提供します。
	/// </summary>
	public static class SpacoRSSClient {

		/// <summary>
		///		HTTPクライアント
		/// </summary>
		private static readonly HttpClient client = new HttpClient();

		/// <summary>
		///		指定したURLからXMLReaderオブジェクトを生成します。
		/// </summary>
		/// <param name="url">XMLのURL</param>
		/// <param name="cancellationToken">処理を中止するためのトークン</param>
		/// <returns>XMLを格納したXMLReaderオブジェクト</returns>
		/// <exception cref="SpacoRSSClientHTTPException">XMLデータの取得に失敗した時</exception>
		/// <remarks>cancellationTokenがnullの場合、処理を中止することができません。</remarks>
		public static Task<XmlReader> GetXmlReaderAsync( string url, CancellationToken? cancellationToken = null ) {

			string responseString = null;

			var task = client.GetAsync( new Uri( url ) );
			// ※cancellationTokenがnullの場合、ダミーとしてCancellationToken.Noneを指定します。
			task.Wait( cancellationToken ?? CancellationToken.None );

			using( var message = task.Result ) {
				if( !message.IsSuccessStatusCode ) {
					throw new SpacoRSSClientHTTPException( $"XMLデータの取得に失敗しました。（HTTP {message.StatusCode}）", message.StatusCode );
				}

				var response = message.Content.ReadAsStringAsync();
				response.Wait( cancellationToken ?? CancellationToken.None );
				responseString = response.Result;
			}

			return Task.FromResult( XmlReader.Create( new StringReader( responseString ) ) );
		}

	}

}
