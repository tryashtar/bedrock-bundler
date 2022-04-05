using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShulkerBundle;

public class InterpPictureBox : PictureBox
{
    private InterpolationMode _InterpolationMode;
    public InterpolationMode InterpolationMode
    {
        get => _InterpolationMode;
        set { _InterpolationMode = value; this.Refresh(); }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.InterpolationMode = InterpolationMode;
        base.OnPaint(e);
    }
}
