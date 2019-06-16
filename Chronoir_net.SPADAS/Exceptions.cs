using System;
using System.Net;

namespace Chronoir_net.SPADAS {
	/// <summary>
	///		すぱこーRSSフィードを取得に失敗した時の例外クラスです。
	/// </summary>
	public class SpacoRSSClientHTTPException : Exception {
		/// <summary>
		///		HTTPステータスコードを取得します。
		/// </summary>
		public HttpStatusCode StatusCode { get; }

		/// <summary>
		///		エラーメッセージとステータスコードより、<see cref="SpacoRSSClientHTTPException"/>クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <param name="message">エラーメッセージ</param>
		/// <param name="statusCode">ステータスコード</param>
		public SpacoRSSClientHTTPException( string message, HttpStatusCode statusCode ) : base( message ) {
			StatusCode = statusCode;
		}

		/// <summary>
		///		エラーメッセージと例外、ステータスコードより、<see cref="SpacoRSSClientHTTPException"/>クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <param name="message">エラーメッセージ</param>
		/// <param name="innerException">この例外を送出する原因となった例外クラス</param>
		/// <param name="statusCode">ステータスコード</param>
		public SpacoRSSClientHTTPException( string message, Exception innerException, HttpStatusCode statusCode ): base( message, innerException ) {
			StatusCode = statusCode;
		}
	}
}
