using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace UniSPADA_Test {
	/// <summary>
	/// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
	/// </summary>
	public sealed partial class MainPage : Page {
		public MainPage() {
			InitializeComponent();
		}

		private void Page_Loaded( object sender, RoutedEventArgs e ) {
			// RSSフィードを取得するコマンドを実行します。
			rssViewModel.GetRSS.Execute( null );
		}

		// RSSフィード取得後のイベントです。（ ViewModel側のハンドラーに設定します ）
		private void rssViewModel_GetRSSCompleted( object sender, TaskResultEventArgs e ) {
			
		}

		// リストボックス内の項目をダブルタップした時のイベントです。
		private async void RSSListBox_DoubleTapped( object sender, DoubleTappedRoutedEventArgs e ) {
			// リストボックスが空（ RSSフィールドを取得していない時 ）
			if( RSSListBox.Items.Count == 0 ) {
				//MessageBox.Show( "RSSフィードのコンテンツが空です。", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
				return;
			}
			try {
				// リンク先を既定のブラウザで開きます。
				await Windows.System.Launcher.LaunchUriAsync( new Uri( rssViewModel.Items[RSSListBox.SelectedIndex].Link ) );
				//Process.Start( rssViewModel.Items[RSSListBox.SelectedIndex].Link );
			}
			catch {
				//MessageBox.Show( "コンテンツのページを開く時にエラーが発生しました。", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
			}
		}

	}
}
