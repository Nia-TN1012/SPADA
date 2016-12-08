using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using Chronoir_net.UniSPADA;
using System.Xml;
using Windows.UI.Xaml.Media.Imaging;

namespace UniSPADA_Test {

	// 任意のタスクの成功・失敗を表す列挙体です。
	enum TaskResult {
		// Succeeded : 成功 / Canceled : 中止 / Failed : 失敗
		Succeeded, Canceled, Failed
	}

	// 任意のタスクの成功・失敗のフラグを扱うイベント引数です。
	class TaskResultEventArgs : EventArgs {
		public TaskResult Result { get; set; }
		public TaskResultEventArgs( TaskResult r ) {
			Result = r;
		}
	}

	// 任意のタスクの成功・失敗のフラグを扱うイベント用デリゲートです。
	delegate void TaskResultEventHandler( object sender, TaskResultEventArgs e );

	// RSSのコンテンツです。
	class SpacoRSSContent : SpacoRSSItem {
		private BitmapImage GetImage( string url ) {
			BitmapImage bi = new BitmapImage();
			try {
				bi = new BitmapImage( new Uri( url ) );
				return bi;
			}
			catch( Exception ) {
				return null;
			}
		}

		// サムネイル画像のキャッシュ
		private BitmapImage thumbnail;
		public BitmapImage Thumbnail => thumbnail ?? ( thumbnail = GetImage( ThumbnailURL ) );

		// 漫画画像のキャッシュ
		private BitmapImage media;
		public BitmapImage Media => media ?? ( media = GetImage( MediaURL ) );

		public SpacoRSSContent( SpacoRSSItem item ) : base( item ) {
		}
	}

	// RSSフィードの配信元情報です。
	class RSSProviderInfo {
		// タイトル
		public string Title { get; set; } = "No RSS Feed";
		// 概要
		public string Description { get; set; } = "";
		// 最終更新日時
		public DateTime LastUpdatedTime { get; set; } = DateTime.MinValue;

		public string BannerURL { get; set; } = "";

		// 自動実装プロパティの初期化子にRSSフィードを取得していない時のダミー情報を設定します。
	}

	// RSSリーダーのModelです。
	class RSSModel : INotifyPropertyChanged {

		// コンストラクター
		public RSSModel() {
			Items = new ObservableCollection<SpacoRSSContent>();
		}

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		// RSSのコンテンツを格納するコレクションです。
		public ObservableCollection<SpacoRSSContent> Items { get; private set; }

		// RSSフィードの配信元情報です。
		public RSSProviderInfo RSSProvider { get; private set; } = new RSSProviderInfo();

		// RSSのURL（ ViewModel、Viewから設定できるようにします。 ）
		public string Url { get; set; } = "";

		// RSSフィードを取得します。
		public async Task GetRSS() {
			TaskResult result = TaskResult.Succeeded;
			RSSProvider = new RSSProviderInfo();
			Items.Clear();
			cancellationTokenSource = new CancellationTokenSource();
			try {
				// ストアアプリやユニバーサルアプリでは、インターネット上のコンテンツを読み込む時は、
				// HttpClientを用いて読み込む必要があります。
				// ObservableCollectionクラスの更新はバインディング先のUIと同じスレッドで行う必要があります。
				// しかし、ストアアプリやユニバーサルアプリではSystem.Windows.Data名前空間が利用できないので、
				// HttpClientによるRSS取得からSpacoRSSReaderのインスタンス作成までを非同期で行い、
				// ObservableCollectionへの更新処理は同期で行います。
				using( XmlReader reader = await Task.Run( () => SpacoRSSClient.GetXmlReaderAsync( Url, cancellationTokenSource.Token ) ) ) {
					SpacoRSSReader srr = await Task.Run( () => SpacoRSSReader.LoadAsync( reader, cancellationTokenSource.Token ) );

					RSSProvider.Title = srr.Title;
					RSSProvider.Description = srr.Description;
					RSSProvider.LastUpdatedTime = srr.PubDate;
					RSSProvider.BannerURL = srr.BannerURL;
					foreach( var item in srr.Items.Where( _ => _.IsAvailable ) ) {
						if( cancellationTokenSource.IsCancellationRequested ) {
							throw new OperationCanceledException();
						}
						Items.Add( new SpacoRSSContent( item ) );
					}
				}

				
			}
			catch( OperationCanceledException ) {
				result = TaskResult.Canceled;
			}
			catch( Exception ) {
				// RSSフィードの取得中に例外が発生したら、失敗フラグを立てます。
				result = TaskResult.Failed;
			}

			// RSSフィードの取得が完了したことをViewModel側に通知します。
			GetRSSCompleted?.Invoke( this, new TaskResultEventArgs( result ) );
		}

		// RSSの取得完了後に発生させるイベントハンドラーです。
		public event TaskResultEventHandler GetRSSCompleted;

		// プロパティ変更後に発生させるイベントハンドラーです。
		public event PropertyChangedEventHandler PropertyChanged;

		// プロパティ変更を通知します。
		private void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		public void CancelGetRSS() {
			cancellationTokenSource?.Cancel();
		}
	}
}