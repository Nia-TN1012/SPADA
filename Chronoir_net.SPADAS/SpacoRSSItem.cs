using System;

namespace Chronoir_net.SPADAS {
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
			 string title = default, string author = default, string link = default, DateTime pubDate = default,
			 string description = default, int volume = default, DateTime modifiedDate = default, bool isAvailable = default,
			 string mediaURL = default, string thumbnailURL = default, string id = default ) {
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
		protected SpacoRSSItem( SpacoRSSItem item )
			: this(
				title: item?.Title, author: item?.Author, link: item?.Link, pubDate: item?.PubDate ?? default,
				description: item?.Description, volume: item?.Volume ?? default, modifiedDate: item?.ModifiedDate ?? default, isAvailable: item?.IsAvailable ?? default,
				mediaURL: item?.MediaURL, thumbnailURL: item?.ThumbnailURL, id: item?.ID
			) {
		}
	}
}
