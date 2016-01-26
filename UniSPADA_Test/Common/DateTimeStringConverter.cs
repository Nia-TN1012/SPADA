using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using System.Globalization;

namespace UniSPADA_Test {

	public class DateTimeStringConverter : IValueConverter {

		public object Convert( object value, Type targetType, object parameter, string language ) {
			if( value != null && value is DateTime ) {
				if( parameter != null && language != null ) {
					return ( ( DateTime )value ).ToString( parameter.ToString(), new CultureInfo( language ) );
				}
				else if( parameter != null ) {
					return ( ( DateTime )value ).ToString( parameter.ToString() );
				}
				else if( language != null ) {
					return ( ( DateTime )value ).ToString( new CultureInfo( language ) );
				}
				return value;
			}
			return null;
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language ) => null;
	}
}
