using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Chronoir_net.XSPADA {

	public static class SpacoRSSClient {

		/// <summary>
		///		指定したURLからXMLReaderオブジェクトを生成します。
		/// </summary>
		/// <param name="url">XMLのURL</param>
		/// <param name="cancellationToken">処理を中止するためのトークン</param>
		/// <exception cref="OperationCanceledException">処理の中止を要求された時</exception>
		/// <returns>XMLを格納したXMLReaderオブジェクト</returns>
		/// <remarks>cancellationTokenがnullの場合、処理を中止することができません。</remarks>
		public static async Task<XmlReader> GetXmlReaderAsync( string url, CancellationToken? cancellationToken ) {

			// HTTPリクエストとHTTPレスポンスを用意します。
			SpacoRSSHttpRequestResponse reqResp = new SpacoRSSHttpRequestResponse();

			reqResp.Request = ( HttpWebRequest )WebRequest.Create( url );

			string responseString = null;			

			bool isCompleted = false;

			// Windows Phone Silverlightもターゲットにしている場合、
			// Begin（End-）GetResponseを用いた方法しかできないようです・・・。
			// ↑を対象外にすれば、GetResponseAsyncで簡単に実装できます。

			// 
			IAsyncResult iAsyncResult = reqResp.Request.BeginGetResponse(
				new AsyncCallback( async ( asyncResult ) => {
					SpacoRSSHttpRequestResponse reqState2 = ( SpacoRSSHttpRequestResponse )asyncResult.AsyncState;

					reqState2.Response = ( HttpWebResponse )reqState2.Request.EndGetResponse( asyncResult );

					using( Stream st = reqState2.Response.GetResponseStream() ) {
						using( StreamReader sr = new StreamReader( st ) ) {
							responseString = await sr.ReadToEndAsync();
						}
					}
					isCompleted = true;
				} ),
				reqResp
			);

			while( !isCompleted ) {
				cancellationToken?.ThrowIfCancellationRequested();
			}

			return XmlReader.Create( new StringReader( responseString ) );
		}

	}

	/// <summary>
	///		HTTPリクエストとHTTPレスポンスを格納します。
	/// </summary>
	public class SpacoRSSHttpRequestResponse {
		/// <summary>
		///		HTTPリクエスト
		/// </summary>
		public HttpWebRequest Request { get; set; } = null;

		/// <summary>
		///		HTTPレスポンス
		/// </summary>
		public HttpWebResponse Response { get; set; } = null;

	}
}
