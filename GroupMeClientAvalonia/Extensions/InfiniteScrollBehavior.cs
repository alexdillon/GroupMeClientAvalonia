using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace GroupMeClientAvalonia.Extensions
{
    public class InfiniteScrollBehavior : Behavior<ListBox>
    {
        private double verticalHeightMax = 0.0;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private ICommand reachedTopCommand;
        private bool isNotAtBottom;
        private bool autoScrollToBottom;
        private bool isLockedToBottom;

        /// <summary>
        /// Gets an Avalonia Property for the command to execute when scrolled to the top of the list.
        /// </summary>
        public static readonly DirectProperty<InfiniteScrollBehavior, ICommand> ReachedTopCommandProperty =
            AvaloniaProperty.RegisterDirect<InfiniteScrollBehavior, ICommand>(
                nameof(ReachedTopCommand),
                isb => isb.ReachedTopCommand, 
                (isb, command) => isb.ReachedTopCommand = command);

        /// <summary>
        /// Gets an Avalonia Property indicating whether the list is not scrolled to the bottom.
        /// </summary>
        public static readonly DirectProperty<InfiniteScrollBehavior, bool> IsNotAtBottomProperty =
          AvaloniaProperty.RegisterDirect<InfiniteScrollBehavior, bool>(
              nameof(IsNotAtBottom),
              isb => isb.IsNotAtBottom);

        /// <summary>
        /// Gets an Avalonia Property indicating whether the list should automatically scroll to the bottom.
        /// </summary>
        public static readonly DirectProperty<InfiniteScrollBehavior, bool> AutoScrollToBottomProperty =
          AvaloniaProperty.RegisterDirect<InfiniteScrollBehavior, bool>(
              nameof(AutoScrollToBottom),
              isb => isb.AutoScrollToBottom);

        /// <summary>
        /// Gets an Avalonia Property indicating whether automatic scrolling to the bottom is currently engaged.
        /// </summary>
        public static readonly DirectProperty<InfiniteScrollBehavior, bool> LockedToBottomProperty =
          AvaloniaProperty.RegisterDirect<InfiniteScrollBehavior, bool>(
              nameof(LockedToBottom),
              isb => isb.LockedToBottom);

        /// <summary>
        /// Gets or sets the command to execute when the list is scrolled to the top.
        /// </summary>
        public ICommand ReachedTopCommand
        {
            get => this.reachedTopCommand;
            set => this.SetAndRaise(ReachedTopCommandProperty, ref this.reachedTopCommand, value);
        }

        /// <summary>
        /// Gets a value indicating whether the list is not scrolled to the bottom.
        /// </summary>
        public bool IsNotAtBottom
        {
            get => this.isNotAtBottom;
            private set => this.SetAndRaise(IsNotAtBottomProperty, ref this.isNotAtBottom, value);
        }

        /// <summary>
        /// Gets a value indicating whether the list should automatically scroll to the bottom.
        /// </summary>
        public bool AutoScrollToBottom
        {
            get => this.autoScrollToBottom;
            private set => this.SetAndRaise(AutoScrollToBottomProperty, ref this.autoScrollToBottom, value);
        }

        /// <summary>
        /// Gets a value indicating whether the list is currently locked to the bottom.
        /// </summary>
        public bool LockedToBottom
        {
            get => this.isLockedToBottom;
            private set => this.SetAndRaise(LockedToBottomProperty, ref this.isLockedToBottom, value);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            Observable.FromEventPattern(this.AssociatedObject, nameof(this.AssociatedObject.LayoutUpdated))
                .Take(1)
                .Subscribe(_ =>
                {
                    var scrollViewer = this.AssociatedObject.Scroll as ScrollViewer;

                    scrollViewer.GetObservable(ScrollViewer.VerticalScrollBarMaximumProperty)
                    .Subscribe(x => this.verticalHeightMax = x)
                    .DisposeWith(this.disposables);

                    scrollViewer.GetObservable(ScrollViewer.ExtentProperty)
                    .ForEachAsync(size =>
                    {
                        if (size.Height == 0)
                        {

                        }
                    });

                    scrollViewer.GetObservable(ScrollViewer.OffsetProperty)
                    .ForEachAsync(offset =>
                    {
                        if (scrollViewer.Extent.Height == 0)
                        {
                            this.LockedToBottom = scrollViewer.Bounds.Height == 0;
                        }

                        if (offset.Y <= Double.Epsilon)
                        {
                            Console.WriteLine("At Top");
                            
                            if (this.ReachedTopCommand.CanExecute(null))
                            {
                                this.ReachedTopCommand.Execute(null);
                            }
                        }

                        var delta = Math.Abs(this.verticalHeightMax - offset.Y);

                        if (delta <= Double.Epsilon)
                        {
                            Console.WriteLine("At Bottom");
                            this.IsNotAtBottom = false;
                        }
                        else
                        {
                            this.IsNotAtBottom = true;
                        }
                    })
                    .DisposeWith(this.disposables);
                });
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            try
            {
                this.disposables.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}
