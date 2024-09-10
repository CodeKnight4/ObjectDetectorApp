using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetector.View
{
    public class EmptyDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect) { }
    }
}
