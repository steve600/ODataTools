﻿using Prism.Mvvm;
using System.Windows.Media;

namespace ODataTools.Shell.Model
{
    public class AccentColor : BindableBase
    {
        private string name;

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { this.SetProperty<string>(ref this.name, value); }
        }

        private Brush colorBrush;

        /// <summary>
        /// The color brush
        /// </summary>
        public Brush ColorBrush
        {
            get { return colorBrush; }
            set { this.SetProperty<Brush>(ref this.colorBrush, value); }
        }
    }
}
