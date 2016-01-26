using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace SPADA_Test {

	// RSSリーダーのViewです。
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			// RSSフィードを取得するコマンドを実行します。
			rssViewModel.GetRSS.Execute( null );
		}

		// RSSフィード取得後のイベントです。（ ViewModel側のハンドラーに設定します ）
		private void rssViewModel_GetRSSCompleted( object sender, TaskResultEventArgs e ) {
			if( e.Result == TaskResult.Canceled )
				MessageBox.Show( "RSSフィードの取得を中止しました。", "Info.", MessageBoxButton.OK, MessageBoxImage.Information );
			else if( e.Result == TaskResult.Failed )
				MessageBox.Show( "RSSフィードの取得中にエラーが発生しました。", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
		}

		// リストボックス内の項目をダブルクリックした時のイベントです。
		private void RSSListBox_MouseDoubleClick( object sender, MouseButtonEventArgs e ) {
			// リストボックスが空（ RSSフィールドを取得していない時 ）
			if( RSSListBox.Items.Count == 0 ) {
				MessageBox.Show( "RSSフィードのコンテンツが空です。", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
				return;
			}
			try {
				// リンク先を既定のブラウザで開きます。
				Process.Start( rssViewModel.Items[RSSListBox.SelectedIndex].Link );
			}
			catch {
				MessageBox.Show( "コンテンツのページを開く時にエラーが発生しました。", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
			}
		}
	}
}
