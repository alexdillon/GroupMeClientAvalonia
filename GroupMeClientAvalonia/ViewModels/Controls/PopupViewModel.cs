using System.Windows.Input;
using Avalonia.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace GroupMeClientAvalonia.ViewModels.Controls
{
    /// <summary>
    /// <see cref="PopupViewModel"/> provides a ViewModel for the <see cref="Views.Controls.Popup"/> control.
    /// </summary>
    public class PopupViewModel : ViewModelBase
    {
        private ViewModelBase popupDialog;
        private bool showPopup;
        private ICommand closePopup;
        private ICommand easyClosePopup;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupViewModel"/> class.
        /// </summary>
        public PopupViewModel()
        {
            this.CheckEasyClose = new RelayCommand<object>(this.CheckEasyCloseHandler);
        }

        /// <summary>
        /// Gets or sets the Popup Dialog that should be displayed.
        /// Null specifies that no popup is shown.
        /// </summary>
        public ViewModelBase PopupDialog
        {
            get => this.popupDialog;

            set
            {
                this.Set(() => this.PopupDialog, ref this.popupDialog, value);
                this.ShowPopup = this.PopupDialog != null;
            }
        }

        /// <summary>
        /// Gets a command checking whether the input conditions for easy closing have been satisfied. If so,
        /// the <see cref="EasyClosePopup"/> command is executed.
        /// </summary>
        public ICommand CheckEasyClose { get; }

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

        private void CheckEasyCloseHandler(object e)
        {
            if (e is PointerPressedEventArgs pointerPressedEvent)
            {
                if (pointerPressedEvent.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
                {
                    this.EasyClosePopup?.Execute(null);
                }
            }
        }
    }
}
