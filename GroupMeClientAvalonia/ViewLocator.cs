using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GalaSoft.MvvmLight;
using GroupMeClientAvalonia.ViewModels;

namespace GroupMeClientAvalonia
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);
            if (type != null)
            {
                return (Control)Activator.CreateInstance(type);
            }
            
            type = Type.GetType(name.Substring(0, name.LastIndexOf("View")));
            if (type != null)
            {
                return (Control)Activator.CreateInstance(type);
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}