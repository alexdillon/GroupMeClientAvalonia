using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using GroupMeClientApi;
using GroupMeClientApi.Models.Attachments;
using Microsoft.Win32;

namespace GroupMeClientAvalonia.ViewModels.Controls
{
    /// <summary>
    /// <see cref="ViewImageControlViewModel"/> provides a ViewModel for the <see cref="Views.Controls.ViewImageControl"/> control.
    /// </summary>
    public class ViewImageControlViewModel : GalaSoft.MvvmLight.ViewModelBase, IDisposable
    {
        private IBitmap imageAttachmentStream;
        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewImageControlViewModel"/> class.
        /// </summary>
        /// <param name="attachment">The attachment display.</param>
        /// <param name="downloader">The downloader to use.</param>
        public ViewImageControlViewModel(ImageAttachment attachment, ImageDownloader downloader)
        {
            this.ImageAttachment = attachment;
            this.ImageDownloader = downloader;

            this.SaveImage = new RelayCommand(this.SaveImageAction);
            this.CopyImage = new RelayCommand(this.CopyImageAction);

            this.IsLoading = true;
            _ = this.LoadImageAttachment();
        }

        /// <summary>
        /// Gets the action to be performed when the save image button is clicked.
        /// </summary>
        public ICommand SaveImage { get; }

        /// <summary>
        /// Gets the action to be performed when the copy image button is clicked.
        /// </summary>
        public ICommand CopyImage { get; }

        /// <summary>
        /// Gets the attached image.
        /// </summary>
        public IBitmap ImageStream
        {
            get => this.imageAttachmentStream;
            private set => this.Set(() => this.ImageStream, ref this.imageAttachmentStream, value);
        }

        /// <summary>
        /// Gets a value indicating whether the loading animation should be displayed.
        /// </summary>
        public bool IsLoading
        {
            get => this.isLoading;
            private set => this.Set(() => this.IsLoading, ref this.isLoading, value);
        }

        private ImageAttachment ImageAttachment { get; }

        private ImageDownloader ImageDownloader { get; }

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            (this.imageAttachmentStream as IDisposable)?.Dispose();
        }

        private async Task LoadImageAttachment()
        {
            var image = await this.ImageDownloader.DownloadPostImageAsync($"{this.ImageAttachment.Url}");

            if (image == null)
            {
                return;
            }

            this.ImageStream = Utilities.ImageUtils.BytesToImageSource(image);
            this.IsLoading = false;
        }

        private void SaveImageAction()
        {
            var saveFileDialog = new SaveFileDialog();

            var imageUrlWithoutLongId = this.ImageAttachment.Url.Substring(0, this.ImageAttachment.Url.LastIndexOf('.'));
            var extension = System.IO.Path.GetExtension(imageUrlWithoutLongId);

            saveFileDialog.DefaultExtension = extension;
            saveFileDialog.Filters.Add(new FileDialogFilter() { Name = "Image", Extensions = { extension } });

            //TODO: Add Window back in. Broken when upgrading to Avalonia 0.90
            var fileName = saveFileDialog.ShowAsync(null).Result;
            if (!string.IsNullOrEmpty(fileName))
            {
                //using (var fs = File.OpenWrite(fileName))
                //{
                //    this.ImageStream.Seek(0, SeekOrigin.Begin);
                //    this.ImageStream.CopyTo(fs);
                //}
            }
        }

        private void CopyImageAction()
        {
            //var ms = new MemoryStream();
            //this.ImageStream.Seek(0, SeekOrigin.Begin);
            //this.ImageStream.CopyTo(ms);

            //var image = Utilities.ImageUtils.BytesToImageSource(ms.ToArray());

            // TODO
            //System.Windows.Clipboard.SetImage(image as BitmapSource);
        }
    }
}
