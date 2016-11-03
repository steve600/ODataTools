using Microsoft.Practices.Unity;
using ODataTools.Infrastructure.Constants;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ODataTools.Core.Base
{
    public abstract class ViewModelPopupBase : ViewModelBase
    {
        #region CTOR

        /// <summary>
        /// CTOR
        /// </summary>
        protected ViewModelPopupBase(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.InitializeCommands();
        }

        #endregion CTOR

        #region Commands

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            this.ClosePopupCommand = new DelegateCommand(this.ClosePopup, this.CanClosePopup);
        }

        public ICommand ClosePopupCommand { get; private set; }

        private bool CanClosePopup()
        {
            return true;
        }

        private void ClosePopup()
        {
            if (this.RegionManager != null)
            {
                var view = this.RegionManager.Regions[RegionNames.DialogPopupRegion].ActiveViews.FirstOrDefault();
                if (view != null)
                {
                    this.RegionManager.Regions[RegionNames.DialogPopupRegion].Remove(view);
                }
            }
        }

        #endregion Commands

        #region Properties

        /// <summary>
        /// Title of the windows
        /// </summary>
        //public abstract string Title { get; }

        /// <summary>
        /// Size to content
        /// </summary>
        public virtual SizeToContent PopupSizeToContent
        {
            get
            {
                return SizeToContent.WidthAndHeight;
            }
        }

        /// <summary>
        /// Resize mode
        /// </summary>
        public virtual ResizeMode PopupResizeMode
        {
            get
            {
                return ResizeMode.NoResize;
            }
        }

        #endregion Properties
    }
}
