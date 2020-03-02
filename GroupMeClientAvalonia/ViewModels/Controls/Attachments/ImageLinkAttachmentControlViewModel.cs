using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using GroupMeClientApi;

namespace GroupMeClientAvalonia.ViewModels.Controls.Attachments
{
    /// <summary>
    /// <see cref="ImageLinkAttachmentControlViewModel"/> provides a ViewModel for the <see cref="Views.Controls.Attachments.ImageLinkAttachmentControl"/> control.
    /// </summary>
    public class ImageLinkAttachmentControlViewModel : LinkAttachmentBaseViewModel
    {
        private IBitmap imageAttachmentStream;
        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLinkAttachmentControlViewModel"/> class.
        /// </summary>
        /// <param name="url">The URL of the image to display.</param>
        /// <param name="imageDownloader">The downloader to use when retreiving data.</param>
        /// <param name="navigateToUrl">The URL of the image to open in a web browser when the user clicks on it.</param>
        public ImageLinkAttachmentControlViewModel(string url, ImageDownloader imageDownloader, string navigateToUrl = null)
            : base(imageDownloader)
        {
            this.Url = url;
            this.NavigateToUrl = navigateToUrl;

            this.Clicked = new RelayCommand(this.ClickedAction);

            this.IsLoading = true;
            _ = this.LoadImageAttachment();
        }

        /// <summary>
        /// Gets the command to be performed when the image is clicked.
        /// </summary>
        public ICommand Clicked { get; }

        /// <summary>
        /// Gets the attached image.
        /// </summary>
        public IBitmap ImageAttachmentStream
        {
            get { return this.imageAttachmentStream; }
            internal set { this.Set(() => this.ImageAttachmentStream, ref this.imageAttachmentStream, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the loading animation should be displayed.
        /// </summary>
        public bool IsLoading
        {
            get => this.isLoading;
            private set => this.Set(() => this.IsLoading, ref this.isLoading, value);
        }

        private string NavigateToUrl { get; }

        /// <inheritdoc/>
        public override void Dispose()
        {
            // Not needed - no unmanaged resources
        }

        /// <inheritdoc/>
        protected override void MetadataDownloadCompleted()
        {
        }

        private async Task LoadImageAttachment()
        {
            var image = await this.ImageDownloader.DownloadPostImageAsync(this.Url);

            if (image == null)
            {
                return;
            }

            this.ImageAttachmentStream = Utilities.ImageUtils.BytesToImageSource(image);
            this.IsLoading = false;
        }

        private void ClickedAction()
        {
            var navigateUrl = !string.IsNullOrEmpty(this.NavigateToUrl) ? this.NavigateToUrl : this.Url;
            Extensions.WebBrowserHelper.OpenUrl(navigateUrl);
        }
    }
}
