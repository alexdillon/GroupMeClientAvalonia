using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GalaSoft.MvvmLight;

namespace GroupMeClientAvalonia
{
    /// <summary>
    /// <see cref="ViewLocator"/> provides a universal Data Template to map between ViewModels and Views.
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        /// <inheritdoc/>
        public bool SupportsRecycling => false;

        /// <inheritdoc/>
        public IControl Build(object data)
        {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);
            if (type != null)
            {
                var control = (Control)Activator.CreateInstance(type);
                return control;
            }

            type = Type.GetType(name.Substring(0, name.LastIndexOf("View")));
            if (type != null)
            {
                var control = (Control)Activator.CreateInstance(type);
                return control;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        /// <inheritdoc/>
        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}