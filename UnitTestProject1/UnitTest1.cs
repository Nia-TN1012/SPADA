using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {

	[TestClass]
	public class UnitTest1 {

		static string url = "Spaco3.xml";

		private static XNamespace p = @"http://pinga.mu/terms/";
		private static XNamespace media = @"http://search.yahoo.com/mrss/";
		private static XNamespace dcndl = @"http://ndl.go.jp/dcndl/terms/";
		private static XNamespace dc = @"http://purl.org/dc/elements/1.1/";

		[TestMethod]
		public void LoadSpacoRSSbyXMLDOM() {
			XmlDocument spx = new XmlDocument();
			spx.Load( url );
			XmlNamespaceManager spxnm = new XmlNamespaceManager( spx.NameTable );
			spxnm.AddNamespace( "p", @"http://pinga.mu/terms/" );
			spxnm.AddNamespace( "media", @"http://search.yahoo.com/mrss/" );
			spxnm.AddNamespace( "dcndl", @"http://ndl.go.jp/dcndl/terms/" );
			spxnm.AddNamespace( "dc", @"http://purl.org/dc/elements/1.1/" );

			XmlNode spxChannel = spx.SelectSingleNode( "/rss/channel" );

			string value;
			value = spxChannel.SelectSingleNode( "./title" ).FirstChild?.Value;
			value = spxChannel.SelectSingleNode( "./description" ).FirstChild?.Value;
			value = spxChannel.SelectSingleNode( "./pubDate" ).FirstChild?.Value;
			value = spxChannel.SelectSingleNode( "./link" ).FirstChild?.Value;
			value = spxChannel.SelectSingleNode( "./image" ).FirstChild?.Value;
			value = spxChannel.SelectSingleNode( "./dc:creator", spxnm ).FirstChild?.Value;

			XmlNodeList spxItems = spxChannel.SelectNodes( "./item" );
			for( int i = 0; i < spxItems.Count; i++ ) {
				XmlNode spxItem = spxItems.Item( i );
				value = spxItem.SelectSingleNode( "./title" ).FirstChild?.Value;
				value = spxItem.SelectSingleNode( "./dc:creator", spxnm ).FirstChild?.Value;
				value = spxItem.SelectSingleNode( "./link" ).FirstChild?.Value;
				value = spxItem.SelectSingleNode( "./pubDate" ).FirstChild?.Value;
				value = spxItem.SelectSingleNode( "./description" ).FirstChild?.Value;
				value = spxItem.SelectSingleNode( "./dcndl:volume", spxnm ).FirstChild?.Value;
				value = spxItem.SelectSingleNode( "./dc:modified", spxnm ).FirstChild?.Value;
				value = spxItem.SelectSingleNode( "./p:isAvailable", spxnm ).FirstChild?.Value;
				value = spxItem.SelectSingleNode( "./media:content/@url", spxnm )?.Value;
				value = spxItem.SelectSingleNode( "./media:thumbnail", spxnm )?.Value;
				value = spxItem.SelectSingleNode( "guid" ).FirstChild?.Value;
			}
		}

		[TestMethod]
		public void LoadSpacoRSSbyLINQtoXML() {

			XElement spx = XElement.Load( url );

			XElement spxChannel = spx.Element( "channel" );

			string value;
			value = spxChannel.Element( "title" ).Value;
			value = spxChannel.Element( "link" ).Value;
			value = spxChannel.Element( "description" ).Value;
			value = spxChannel.Element( "pubDate" ).Value;
			value = spxChannel.Element( "image" ).Value;
			value = spxChannel.Element( dc + "creator" ).Value;

			var spxItems = spxChannel.Elements( "item" );
			foreach( var item in spxItems ) {
				value = item.Element( "title" ).Value;
				value = item.Element( dc + "creator" ).Value;
				value = item.Element( "link" ).Value;
				value = item.Element( "pubDate" ).Value;
				value = item.Element( "description" ).Value;
				value = item.Element( dcndl + "volume" ).Value;
				value = item.Element( dc + "modified" ).Value;
				value = item.Element( p + "isAvailable" ).Value;
				value = item.Element( media + "content" ).Attribute( "url" ).Value;
				value = item.Element( media + "thumbnail" ).Attribute( "url" ).Value;
				value = item.Element( "guid" ).Value;
			}
		}

		[TestMethod]
		public void LoadSpacoRSSbyXMLReader() {

			using( XmlReader reader = XmlReader.Create( url ) ) {
				reader.Read();
				reader.ReadStartElement( "rss" );
				reader.ReadStartElement( "channel" );

				string value;

				while( reader.Read() ) {
					if( reader.NodeType == XmlNodeType.Element ) {
						switch( reader.Name ) {
							case "title":
								reader.ReadStartElement( "title" );
								value = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "link":
								reader.ReadStartElement( "link" );
								value = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "description":
								reader.ReadStartElement( "description" );
								value = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "pubDate":
								reader.ReadStartElement( "pubDate" );
								value = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "image":
								reader.ReadStartElement( "image" );
								value = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "dc:creator":
								reader.ReadStartElement( "dc:creator" );
								value = reader.ReadContentAsString();
								reader.ReadEndElement();
								break;
							case "item":
								LoadItem( reader );
								break;
							default:
								break;
						}
					}
					else if( reader.NodeType == XmlNodeType.EndElement && reader.Name == "channel" ) {
						reader.ReadEndElement();
						break;
					}
				}
				while( reader.Read() ) ;
			}

		}

		private static void LoadItem( XmlReader reader ) {
			reader.ReadStartElement( "item" );
			string value;
			while( reader.Read() ) {

				if( reader.NodeType == XmlNodeType.Element ) {
					switch( reader.Name ) {
						case "title":
							reader.ReadStartElement( "title" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						case "dc:creator":
							reader.ReadStartElement( "dc:creator" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						case "link":
							reader.ReadStartElement( "link" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						case "pubDate":
							reader.ReadStartElement( "pubDate" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						case "description":
							reader.ReadStartElement( "description" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						case "dcndl:volume":
							reader.ReadStartElement( "dcndl:volume" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						case "dc:modified":
							reader.ReadStartElement( "dc:modified" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						case "p:isAvailable":
							reader.ReadStartElement( "p:isAvailable" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						case "media:content":
							reader.MoveToAttribute( "url" );
							value = reader.ReadContentAsString();
							reader.MoveToElement();
							reader.Skip();
							break;
						case "media:thumbnail":
							reader.MoveToAttribute( "url" );
							value = reader.ReadContentAsString();
							reader.MoveToElement();
							reader.Skip();
							break;
						case "guid":
							reader.ReadStartElement( "guid" );
							value = reader.ReadContentAsString();
							reader.ReadEndElement();
							break;
						default:
							break;
					}

				}
				else if( reader.NodeType == XmlNodeType.EndElement && reader.Name == "item" ) {
					reader.ReadEndElement();
					break;
				}
			}
		}
	}
}
