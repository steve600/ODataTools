using MahApps.Metro.Controls;
using Prism.Interactivity;
using System.Windows;

namespace ODataTools.Core.TriggerActions
{
    public class PopupMetroWindowAction : PopupWindowAction
    {
        protected override Window CreateWindow()
        {
            //return base.CreateWindow();
            return new MetroWindow();
        }
    }
}
