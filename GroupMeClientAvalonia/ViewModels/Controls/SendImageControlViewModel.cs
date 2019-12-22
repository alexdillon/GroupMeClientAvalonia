using Avalonia.Input;
using Avalonia.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using System;
using System.IO;
using System.Windows.Input;

namespace GroupMeClientAvalonia.ViewModels.Controls
{
    /// <summary>
    /// <see cref="SendImageControlViewModel"/> provides a ViewModel for the <see cref="Views.Controls.SendImageControl"/> control.
    /// </summary>
    public class SendImageControlViewModel : GalaSoft.MvvmLight.ViewModelBase, IDisposable
    {
        private string typedMessageContents;
        private bool isSending;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendImageControlViewModel"/> class.
        /// </summary>
        public SendImageControlViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the command to be performed when the message is ready to send.
        /// </summary>
        public ICommand SendMessage { get; set; }

        /// <summary>
        /// Gets or sets the image to preview.
        /// </summary>
        public IBitmap Image { get; set; }

        /// <summary>
        /// The raw encoded image data corresponding to <see cref="Image"/>.
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// Gets or sets the message the user has composed to send.
        /// </summary>
        public string TypedMessageContents
        {
            get => this.typedMessageContents;
            set => this.Set(() => this.TypedMessageContents, ref this.typedMessageContents, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sending animation should be displayed.
        /// </summary>
        public bool IsSending
        {
            get => this.isSending;
            set => this.Set(() => this.IsSending, ref this.isSending, value);
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            this.Image.Dispose();
        }
    }
}
