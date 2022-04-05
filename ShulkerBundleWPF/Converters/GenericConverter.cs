using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShulkerBundleWPF;
public abstract class GenericConverter<TFrom, TTo> : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert((TFrom)value);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ConvertBack((TTo)value);
    }

    public abstract TTo Convert(TFrom value);
    public abstract TFrom ConvertBack(TTo value);
}

public abstract class GenericParamConverter<TFrom, TTo, TParam> : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert((TFrom)value, (TParam)parameter);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ConvertBack((TTo)value, (TParam)parameter);
    }

    public abstract TTo Convert(TFrom value, TParam parameter);
    public abstract TFrom ConvertBack(TTo value, TParam parameter);
}

public abstract class GenericCollectionParamConverter<TFrom, TTo, TParam> : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((IEnumerable<TFrom>)value).Select(x => Convert(x, (TParam)parameter));
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((IEnumerable<TTo>)value).Select(x => ConvertBack(x, (TParam)parameter));
    }

    public abstract TTo Convert(TFrom value, TParam parameter);
    public abstract TFrom ConvertBack(TTo value, TParam parameter);
}
