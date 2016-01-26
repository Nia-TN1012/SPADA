using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace XSPADA_Test {
	public partial class MainPage1 : ContentPage {


		Label label;

		public MainPage1() {
			InitializeComponent();

			label = new Label();

			Button b = new Button {
				Text = "あ",
			};
			b.Clicked += B_Clicked;

			this.Content = new StackLayout {
				Children = {
					b,
					label
				}
			};
		}

		private void B_Clicked( object sender, EventArgs e ) {
			label.Text = "Z";
			X2();
		}

		private async Task X2() {
			label.Text = await Task.Run( XX2 );
		}

		private Task<string> XX2() {
			string z = "-";
			try {
				using( XmlReader xr = XmlReader.Create( "http://pronama-api.azurewebsites.net/feed/spaco?count=5" ) ) {
					xr.Read();
					xr.ReadStartElement( "rss" );
					xr.ReadStartElement( "channel" );
					xr.ReadStartElement( "title" );
					z = xr.ReadContentAsString();
				}
			}
			catch( XmlException e ) {
				throw;
			}
			return Task.FromResult( z );
		}

	}
}
