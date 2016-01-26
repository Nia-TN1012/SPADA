using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Linq;
using System.Threading;

using Chronoir_net.XSPADA;
using System.Xml;

namespace SPADA_Test {

	// 任意のタスクの成功・失敗を表す列挙体です。
	enum TaskResult {
		// Succeeded : 成功 / Failed : 失敗
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
	class RSSContent {
		// タイトル
		public string Title { get; set; }
		// 概要
		public string Summary { get; set; }
		// 投稿日時
		public DateTime PubDate { get; set; }
		// リンク
		public string Link { get; set; }
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
			Items = new ObservableCollection<SpacoRSSItem>();
			BindingOperations.EnableCollectionSynchronization( Items, new object() );
		}

		// RSSのコンテンツを格納するコレクションです。
		public ObservableCollection<SpacoRSSItem> Items { get; private set; }

		// RSSフィードの配信元情報です。
		public RSSProviderInfo RSSProvider { get; private set; } = new RSSProviderInfo();

		// RSSのURL（ ViewModel、Viewから設定できるようにします。 ）
		public string Url { get; set; } = "";

		CancellationTokenSource cancellationTokenSource;

		// RSSフィードを取得します。（ 非同期メソッド ）
		public async Task<TaskResult> GetRSS() {
			TaskResult result = TaskResult.Succeeded;
			RSSProvider = new RSSProviderInfo();
			Items.Clear();
			cancellationTokenSource = new CancellationTokenSource();
			try {
				/*using( XmlReader xr = XmlReader.Create( Url ) ) {
					SpacoRSSReader srr = SpacoRSSReader.Load( xr );
					RSSProvider.Title = srr.Title;
					RSSProvider.Description = srr.Description;
					RSSProvider.LastUpdatedTime = srr.PubDate;
					RSSProvider.BannerURL = srr.BannerURL;
					foreach( var item in srr.Items.Where( _ => _.IsAvailable ) ) {
						Items.Add( item );
					}
				}*/
				using( XmlReader reader = await Task.Run( () => SpacoRSSClient.GetXmlReaderAsync( Url, cancellationTokenSource.Token ) ) ) {
					SpacoRSSReader srr = await Task.Run( () => SpacoRSSReader.LoadAsync( reader, cancellationTokenSource.Token ) );

					RSSProvider.Title = srr.Title;
					RSSProvider.Description = srr.Description;
					RSSProvider.LastUpdatedTime = srr.PubDate;
					RSSProvider.BannerURL = srr.BannerURL;
					foreach( var item in srr.Items.Where( _ => _.IsAvailable ) ) {
						Items.Add( item );
					}
				}
			}
			catch( OperationCanceledException ) {
				result = TaskResult.Canceled;
			}
			catch {
				// RSSフィードの取得中に例外が発生したら、失敗フラグを立てます。
				result = TaskResult.Failed;
			}
			return result;
		}

		// RSSの取得完了後に発生させるイベントハンドラーです。
		public event TaskResultEventHandler GetRSSCompleted;

		// プロパティ変更後に発生させるイベントハンドラーです。
		public event PropertyChangedEventHandler PropertyChanged;

		// プロパティ変更を通知します。
		private void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		// RSSフィードを取得します。
		public async void GetRSSAsync() {
			// RSSフィードを非同期で取得します。
			TaskResult result =  await GetRSS();
			// RSSフィードの取得が完了したことをViewModel側に通知します。
			GetRSSCompleted?.Invoke( this, new TaskResultEventArgs( result ) );
		}

		public void CancelGetRSS() {
			cancellationTokenSource?.Cancel();
		}
	}
}