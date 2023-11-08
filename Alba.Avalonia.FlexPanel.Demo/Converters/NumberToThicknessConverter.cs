using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Alba.Avalonia.FlexPanel.Demo.Converters;

internal sealed class NumberToThicknessConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int x || !targetType.IsAssignableFrom(typeof(Thickness)))
            throw new NotSupportedException();
        var y = 16 + 2 * ((x * 5) % 9);
        return new Thickness(2 * y, y);

    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}