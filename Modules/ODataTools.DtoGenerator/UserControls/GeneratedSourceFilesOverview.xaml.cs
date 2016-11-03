using ODataTools.Infrastructure.Model;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ODataTools.DtoGenerator.UserControls
{
    /// <summary>
    /// Interaktionslogik für GeneratedSourceFilesOverview.xaml
    /// </summary>
    public partial class GeneratedSourceFilesOverview : UserControl
    {
        public GeneratedSourceFilesOverview()
        {
            InitializeComponent();
        }

        public Dictionary<FileInfo, string> GeneratedSourceFiles
        {
            get { return (Dictionary<FileInfo, string>)GetValue(GeneratedSourceFilesProperty); }
            set { SetValue(GeneratedSourceFilesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GeneratedSourceFiles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeneratedSourceFilesProperty =
            DependencyProperty.Register("GeneratedSourceFiles", typeof(Dictionary<FileInfo, string>), typeof(GeneratedSourceFilesOverview), new PropertyMetadata(null, OnGeneratedSourceFilesChanged));

        /// <summary>
        /// Generated source files changed handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnGeneratedSourceFilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GeneratedSourceFilesOverview overview = (GeneratedSourceFilesOverview)d;

            if (e.NewValue != null && e.NewValue is Dictionary<FileInfo, string>)
            {
                overview.mainTabControl.Items.Clear();

                var sourceFiles = e.NewValue as Dictionary<FileInfo, string>;

                foreach (var f in sourceFiles)
                {
                    overview.mainTabControl.Items.Add(new TabContent(f.Key.Name, f.Value));
                }

                overview.mainTabControl.SelectedIndex = 0;
            }
            else
            {
                overview.mainTabControl.Items.Clear();
            }
        }
    }
}
