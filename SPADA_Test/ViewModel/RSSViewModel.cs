using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Chronoir_net.XSPADA;

namespace SPADA_Test {
	// RSSリーダーのViewModelです。
	class RSSViewModel : INotifyPropertyChanged {

		// Model
		RSSModel rssModel = new RSSModel();

		// コンストラクター
		public RSSViewModel() {
			rssModel.PropertyChanged += ( sender, e ) =>
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( e.PropertyName ) );
			// RSSフィードの取得が完了したことをView側に通知します。
			rssModel.GetRSSCompleted += ( sender, e ) => {
				// RSSフィード取得中のフラグをオフにします。
				IsProgress = false;
				// RSSフィード取得完了したことをView側に通知します。
				GetRSSCompleted?.Invoke( this, e );
			};
		}

		// RSSフィードのタイトル
		public string Title =>
			IsProgress ? "RSSフィールドを取得中 ..." : rssModel.RSSProvider.Title;

		// RSSフィードの説明
		public string Description =>
			IsProgress ? "Recieving ..." : rssModel.RSSProvider.Description;

		// RSSフィードの最終更新日
		public DateTime LastUpdatedTime =>
			IsProgress ? DateTime.MinValue : rssModel.RSSProvider.LastUpdatedTime;

		public string BannerURL =>
			IsProgress ? "" : rssModel.RSSProvider.BannerURL;

		// RSSフィードのコンテンツ
		public ObservableCollection<SpacoRSSItem> Items => rssModel.Items;

		// RSSフィードのURL
		public string Url {
			get { return rssModel.Url; }
			set { rssModel.Url = value; }
		}

		// RSSフィード取得中のフラグ
		private bool isProgress = false;
		public bool IsProgress {
			get { return isProgress; }
			set {
				isProgress = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged( nameof( Title ) );
				NotifyPropertyChanged( nameof( Description ) );
				NotifyPropertyChanged( nameof( LastUpdatedTime ) );
				NotifyPropertyChanged( nameof( BannerURL ) );
			}
		}

		// RSSの取得完了後に発生させるイベントハンドラーです。
		public event TaskResultEventHandler GetRSSCompleted;

		// プロパティ変更後に発生させるイベントハンドラーです。
		public event PropertyChangedEventHandler PropertyChanged;

		// プロパティ変更を通知します。
		private void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}
		
		private ICommand getRSS;
		public ICommand GetRSS => getRSS ?? ( getRSS = new GetRSSCommand( this ) );

		private ICommand cancelGetRSS;
		public ICommand CancelGetRSS => cancelGetRSS ?? ( cancelGetRSS = new CancelGetRSSCommand( this ) );

		// RSSフィードを取得するコマンドです。
		private class GetRSSCommand : ICommand {

			// ViewModelの参照
			private RSSViewModel rssViewModel;

			//　コンストラクター
			public GetRSSCommand( RSSViewModel viewModel ) {
				rssViewModel = viewModel;
				// コマンド実行の可否の変更を通知します。
				rssViewModel.PropertyChanged += ( sender, e ) =>
					CanExecuteChanged?.Invoke( sender, e );
			}

			// コマンドを実行できるかどうかを取得します。
			public bool CanExecute( object parameter ) =>
				!rssViewModel.IsProgress;

			// コマンド実行の可否の変更した時のイベントハンドラーです。
			public event EventHandler CanExecuteChanged;

			// コマンドを実行し、RSSフィードを取得します。
			public void Execute( object parameter ) {
				// RSSフィード取得中のフラグをオンにします。
				rssViewModel.IsProgress = true;
				// RSSフィード取得します。
				rssViewModel.rssModel.GetRSSAsync();
			}

		}

		// RSSフィードを取得を中止するコマンドです。
		private class CancelGetRSSCommand : ICommand {

			// ViewModelの参照
			private RSSViewModel rssViewModel;

			//　コンストラクター
			public CancelGetRSSCommand( RSSViewModel viewModel ) {
				rssViewModel = viewModel;
				// コマンド実行の可否の変更を通知します。
				rssViewModel.PropertyChanged += ( sender, e ) =>
					CanExecuteChanged?.Invoke( sender, e );
			}

			// コマンドを実行できるかどうかを取得します。
			public bool CanExecute( object parameter ) =>
				rssViewModel.IsProgress;

			// コマンド実行の可否の変更した時のイベントハンドラーです。
			public event EventHandler CanExecuteChanged;

			// コマンドを実行し、RSSフィードを取得します。
			public void Execute( object parameter ) {
				// RSSフィード取得を中止します。
				rssViewModel.rssModel.CancelGetRSS();
				// RSSフィード取得中のフラグをオフにします。
				rssViewModel.IsProgress = false;
			}

		}

	}
}