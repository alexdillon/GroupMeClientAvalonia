using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using GalaSoft.MvvmLight;
using GroupMeClientApi;
using GroupMeClientApi.Models;

namespace GroupMeClientAvalonia.ViewModels.Controls
{
    /// <summary>
    /// <see cref="AvatarControlViewModel"/> provides the ViewModel for a control to display a GroupMe Avatar.
    /// </summary>
    public class AvatarControlViewModel : ViewModelBase
    {
        private bool isRounded;
        private IBitmap avatar;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvatarControlViewModel"/> class.
        /// </summary>
        /// <param name="avatarSource">The avatar that should be displayed.</param>
        /// <param name="imageDownloader">The downloader used to retreive the avatar.</param>
        public AvatarControlViewModel(IAvatarSource avatarSource, ImageDownloader imageDownloader)
        {
            this.AvatarSource = avatarSource;
            this.ImageDownloader = imageDownloader;

            _ = this.LoadAvatarAsync();
        }

        /// <summary>
        /// Gets the <see cref="IAvatarSource"/> this control is displaying.
        /// </summary>
        public IAvatarSource AvatarSource { get; }

        /// <summary>
        /// Gets the <see cref="ImageDownloader"/> that should be used to retreive avatars.
        /// </summary>
        public ImageDownloader ImageDownloader { get; }

        /// <summary>
        /// Gets a value indicating whether the avatar should be cropped to be circular.
        /// </summary>
        public bool IsRounded
        {
            get => this.isRounded;
            private set => this.Set(() => this.IsRounded, ref this.isRounded, value);
        }

        /// <summary>
        /// Gets the image that should be used for square avatars.
        /// If the avatar shouldn't be rectangular, null is returned.
        /// </summary>
        public IBitmap Avatar
        {
            get => this.avatar;
            private set => this.Set(() => this.Avatar, ref this.avatar, value);
        }

        /// <summary>
        /// Asychronously downloads the avatar image from GroupMe.
        /// </summary>
        /// <returns>A <see cref="Task"/> with the download status.</returns>
        public async Task LoadAvatarAsync()
        {
            var isGroup = !this.AvatarSource.IsRoundedAvatar;
            byte[] image = await this.ImageDownloader.DownloadAvatarImageAsync(this.AvatarSource.ImageOrAvatarUrl, isGroup);

            var bitmapImage = Utilities.ImageUtils.BytesToImageSource(image);

            this.Avatar = bitmapImage;
            this.IsRounded = this.AvatarSource.IsRoundedAvatar;
        }
    }
}
