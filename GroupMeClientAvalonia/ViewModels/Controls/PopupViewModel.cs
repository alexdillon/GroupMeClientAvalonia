using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace GroupMeClientAvalonia.ViewModels.Controls
{
    public class PopupViewModel : ViewModelBase
    {
        private ViewModelBase popupDialog;
        private bool showPopup;
        private ICommand closePopup;
        private ICommand easyClosePopup;

        public PopupViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the Popup Dialog that should be displayed.
        /// Null specifies that no popup is shown.
        /// </summary>
        public ViewModelBase PopupDialog
        {
            get 
            {
                return this.popupDialog;
            }

            set
            {
                this.Set(() => this.PopupDialog, ref this.popupDialog, value);
                this.ShowPopup = (this.PopupDialog != null);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the popup should be shown.
        /// </summary>
        public bool ShowPopup
        {
            get => this.showPopup;
            set => this.Set(() => this.ShowPopup, ref this.showPopup, value);
        }

        /// <summary>
        /// Gets or sets the action to be be performed when the big popup has been closed.
        /// </summary>
        public ICommand ClosePopup
        {
            get => this.closePopup;
            set => this.Set(() => this.ClosePopup, ref this.closePopup, value);
        }

        /// <summary>
        /// Gets or sets the action to be be performed when the big popup has been closed indirectly.
        /// This typically is from the user clicking in the gray area around the popup to dismiss it.
        /// </summary>
        public ICommand EasyClosePopup
        {
            get => this.easyClosePopup;
            set => this.Set(() => this.EasyClosePopup, ref this.easyClosePopup, value);
        }
    }
}
