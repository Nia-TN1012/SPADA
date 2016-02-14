using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Chronoir_net.XSPADA;
using System.Diagnostics;

namespace XSPADA_Test {
	public partial class MainPage1 : ContentPage {
		private RSSViewModel rssViewModel;

		public MainPage1() {
			this.LoadFromXaml( typeof( MainPage1 ) );

			rssViewModel = new RSSViewModel();
			rssViewModel.Url = "http://pronama-api.azurewebsites.net/feed/spaco?count=20";

			BindingContext = rssViewModel;
			rssViewModel.GetRSS.Execute( null );
		}
	}
}
