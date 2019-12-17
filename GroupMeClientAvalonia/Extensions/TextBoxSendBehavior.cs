using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System;
using System.Windows.Input;

namespace GroupMeClientAvalonia.Extensions
{
    /// <summary>
    /// <see cref="TextBoxSendBehavior"/> provides support for typing MultiLine messages.
    /// Keyboard send triggers are supported.
    /// </summary>
    public class TextBoxSendBehavior : Behavior<TextBox>
    {
        private ICommand sendCommand;

        /// <summary>
        /// Gets an Avalonia Property for the command to execute when sending is invoked.
        /// </summary>
        public static readonly DirectProperty<TextBoxSendBehavior, ICommand> SendCommandProperty =
            AvaloniaProperty.RegisterDirect<TextBoxSendBehavior, ICommand>(
                nameof(SendCommand),
                tsb => tsb.SendCommand,
                (tsb, command) => tsb.SendCommand = command);

        /// <summary>
        /// Gets or sets the command to execute when send behavior is invoked.
        /// </summary>
        public ICommand SendCommand
        {
            get => this.sendCommand;
            set => this.SetAndRaise(SendCommandProperty, ref this.sendCommand, value);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.KeyDown += this.AssociatedObject_KeyDown;

            // If AcceptsReturn is true, the TextBox will automatically consume KeyDown events and mark them as handled.
            this.AssociatedObject.AcceptsReturn = false;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            try
            {
                this.AssociatedObject.KeyDown -= this.AssociatedObject_KeyDown;
            }
            catch (Exception)
            {
            }
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            var controlPressed = e.KeyModifiers.HasFlag(KeyModifiers.Control);
            var shiftPressed = e.KeyModifiers.HasFlag(KeyModifiers.Shift);

            if ((e.Key == Key.Enter || e.Key == Key.Return) &&
                (!controlPressed && !shiftPressed))
            {
                e.Handled = true;
                this.SendCommand?.Execute(null);
            }
            else if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                var beforeCaret = this.AssociatedObject.Text.Substring(0, this.AssociatedObject.CaretIndex);
                var afterCaret = this.AssociatedObject.Text.Substring(this.AssociatedObject.CaretIndex);
                var newString = $"{beforeCaret}{this.AssociatedObject.NewLine}{afterCaret}";

                this.AssociatedObject.Text = newString;
                this.AssociatedObject.CaretIndex++;
            }
        }
    }
}
