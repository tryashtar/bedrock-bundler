using ShulkerBundle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShulkerBundleWPF;
class PackResolver : GenericCollectionParamConverter<PackReference, Pack, Minecraft>
{
    public override Pack Convert(PackReference value, Minecraft parameter)
    {
        return parameter.GetResourcePack(value);
    }

    public override PackReference ConvertBack(Pack value, Minecraft parameter)
    {
        return null;
    }
}
