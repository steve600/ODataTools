using MahApps.Metro.Controls;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;

namespace ODataTools.Shell.Views
{
    /// <summary>
    /// Interaktionslogik für ApplicationSettings.xaml
    /// </summary>
    public partial class ApplicationSettings : Flyout, IFlyoutView
    {
        public ApplicationSettings()
        {
            InitializeComponent();
        }

        public string FlyoutName
        {
            get
            {
                return FlyoutNames.ApplicationSettingsFlyout;
            }
        }
    }
}
