using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Helpers.Extensions
{
    public enum WindowBackdropStyle
    {
        Acrylic = 0,
        Mica = 1,
    }

    public static class WindowAcrylicMicaExtensions
    {
        public static void SetBackdropStyle(this Window window,WindowBackdropStyle style)
        {
            AcrylicMicaHelper helper = new AcrylicMicaHelper(window);
            _ = style switch
            {
                WindowBackdropStyle.Acrylic => helper.TrySetAcrylicBackdrop(),
                WindowBackdropStyle.Mica => helper.TrySetMicaBackdrop(),
                _ => throw new InvalidOperationException("Value does not fall in expected range.")
            };
        }
    }
}
