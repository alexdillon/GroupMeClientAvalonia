using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using GalaSoft.MvvmLight.Command;

namespace GroupMeClient.Extensions
{
    /// <summary>
    /// <see cref="ListBoxWithPosition"/> provides a ListBox control that reports on scroll position.
    /// </summary>
    public class ListBoxWithPosition : ListBox
    {
        /// <summary>
        /// Gets a Dependency Property indicating whether the ListBox is scrolled to the bottom.
        /// </summary>
        public static readonly AvaloniaProperty IsNotAtBottomProperty =
            AvaloniaProperty.Register<ListBoxWithPosition, bool>(
                "IsNotAtBottom",
                inherits: true);

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxWithPosition"/> class.
        /// </summary>
        public ListBoxWithPosition()
        {
            this.AttachedToVisualTree += this.ListBoxWithPosition_Loaded;
            this.ScrollToEnd = new RelayCommand(this.DoScrollToEnd);
        }

        /// <summary>
        /// Gets a value indicating whether the ListBox is scrolled to the bottom.
        /// </summary>
        public bool IsNotAtBottom => (bool)this.GetValue(IsNotAtBottomProperty);

        /// <summary>
        /// Gets a command that can be executed to scroll this <see cref="ListBox"/> to the bottom.
        /// </summary>
        public ICommand ScrollToEnd { get; }

        private ScrollViewer ScrollViewer { get; set; }

        private bool ShouldSnapToBottom { get; set; } = false;

        private CompositeDisposable Disposables { get; set; } = new CompositeDisposable();

        private double MaxVerticalHeight { get; set; }

        private void ListBoxWithPosition_Loaded(object sender, VisualTreeAttachmentEventArgs e)
        {
            (sender as ListBox).GetObservable(ListBox.ScrollProperty)
                .OfType<ScrollViewer>()
                .Take(1)
                .Subscribe(sv => {

                    this.Disposables?.Dispose();
                    this.Disposables = new CompositeDisposable();

                    sv.GetObservable(ScrollViewer.VerticalScrollBarMaximumProperty)
                        .Subscribe(newMax => this.MaxVerticalHeight = newMax)
                        .DisposeWith(this.Disposables);

                    sv.GetObservable(ScrollViewer.OffsetProperty)
                    .Subscribe(offset =>
                    {
                        var delta = Math.Abs(this.MaxVerticalHeight - offset.Y);
                        var atBottom = (delta <= double.Epsilon);
                        this.ScrollViewer_ScrollChanged(atBottom);
                    }).DisposeWith(this.Disposables);
                });
        }

        private void ScrollViewer_ScrollChanged(bool atBottom)
        {
            // When DPI scaling is enabled, pixel values may be floating point. Round down to integers to
            // prevent floating-point roundoff error when comparing values.
            this.SetValue(IsNotAtBottomProperty, !atBottom);

            if (this.ShouldSnapToBottom)
            {
                if (atBottom)
                {
                    this.ShouldSnapToBottom = false;
                }
                else
                {
                    this.ScrollViewer.Offset = new Vector(0, this.ScrollViewer.Height);
                    //this.ScrollViewer.ScrollToBottom();
                }
            }
        }

        private void DoScrollToEnd()
        {
            this.ShouldSnapToBottom = true;
            this.ScrollViewer.Offset = new Vector(0, this.ScrollViewer.Height);
            //this.ScrollViewer.ScrollToBottom();
        }
    }
}
