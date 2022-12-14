using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShulkerBundle.Converters;

[ValueConversion(typeof(string), typeof(ImageSource))]
public class ImageConverter : GenericConverter<string, ImageSource>
{
    public override ImageSource Convert(string value)
    {
        if (value == null)
            return null;
        if (!File.Exists(value))
            return null;
        return BitmapFrame.Create(new Uri(value), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
    }

    public override string ConvertBack(ImageSource value)
    {
        throw new InvalidOperationException();
    }
}
