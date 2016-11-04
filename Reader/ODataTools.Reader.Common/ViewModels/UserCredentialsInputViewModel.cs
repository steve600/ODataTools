using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.Reader.Common.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ODataTools.Reader.Common.ViewModels
{
    public class UserCredentialsInputViewModel : ViewModelBase, IInteractionRequestAware
    {
        private UserCredentialsConfirmation confirmation = null;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public UserCredentialsInputViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.AcceptCommand = new DelegateCommand(this.AcceptUserCredentials, AcceptUserCredentialsCanExecute)
                .ObservesProperty(() => this.UserName)
                .ObservesProperty(() => this.Password);

            this.CancelCommand = new DelegateCommand(this.CancelInteraction);
            this.PasswordChangedCommand = new DelegateCommand<object>(this.PasswordChanged);
        }

        #region Commands

        public ICommand AcceptCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public ICommand PasswordChangedCommand { get; private set; }

        #endregion Commands

        public void AcceptUserCredentials()
        {
            if (this.confirmation != null)
            {
                this.confirmation.UserCredentials = new NetworkCredential(this.UserName, this.Password);
                this.confirmation.Confirmed = true;
            }

            this.FinishInteraction();
        }

        public bool AcceptUserCredentialsCanExecute()
        {
            return (!String.IsNullOrEmpty(this.UserName) && this.Password != null);
        }

        /// <summary>
        /// Cancel interaction
        /// </summary>
        public void CancelInteraction()
        {
            if (this.confirmation != null)
            {
                this.confirmation.UserCredentials = null;
                this.confirmation.Confirmed = false;
            }

            this.FinishInteraction();
        }

        /// <summary>
        /// Password changed
        /// </summary>
        /// <param name="password"></param>
        private void PasswordChanged(object sender)
        {
            if (sender != null && sender is PasswordBox)
            {
                this.Password = ((PasswordBox)sender).SecurePassword;
            }
        }

        #region Properties

        private string userName = string.Empty;

        /// <summary>
        /// User name
        /// </summary>
        public string UserName
        {
            get { return this.userName; }
            set { this.SetProperty(ref this.userName, value); }
        }

        private SecureString password;

        /// <summary>
        /// Password
        /// </summary>
        public SecureString Password
        {
            get { return this.password; }
            private set { this.SetProperty(ref this.password, value); }
        }

        #endregion Properties

        #region IInteractionRequestAware

        // Both the FinishInteraction and Notification properties will be set by the PopupWindowAction
        // when the popup is shown.
        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get
            {
                return this.confirmation;
            }
            set
            {
                if (value is UserCredentialsConfirmation)
                {
                    // To keep the code simple, this is the only property where we are raising the PropertyChanged event,
                    // as it's required to update the bindings when this property is populated.
                    // Usually you would want to raise this event for other properties too.
                    this.confirmation = value as UserCredentialsConfirmation;
                    this.OnPropertyChanged(() => this.Notification);
                }
            }
        }

        #endregion IInteractionRequestAware
    }
}
