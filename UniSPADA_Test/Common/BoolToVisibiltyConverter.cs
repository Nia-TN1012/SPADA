using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace UniSPADA_Test {

	/// <summary>
	///		bool値とVisibility値の相互変換を行います。
	/// </summary>
	public sealed class BoolToVisibilityConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			( value is bool && ( bool )value ) ? Visibility.Visible : Visibility.Collapsed;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is Visibility && ( Visibility )value == Visibility.Visible;
	}

}
