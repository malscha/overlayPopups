using Overlay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Overlay.Render
{
    public interface IRenderer
    {
        void RenderCard(Card card, Canvas canvas);
        void RenderScene(Scene scene, Configuration config, Canvas canvas);
        void ClearCanvas(Canvas canvas);
    }
}