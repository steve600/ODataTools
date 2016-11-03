using MahApps.Metro.Controls;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
