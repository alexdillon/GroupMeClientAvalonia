using GroupMeClientApi;
using GroupMeClientApi.Models;
using GroupMeClientPlugin.GroupChat;
using System;
using System.Collections.Generic;
using System.Text;

namespace GroupMeClientAvalonia.ViewModels
{
    public class SearchViewModel
    {
        public SearchViewModel(GroupMeClient groupMeClient, object cacheContext)
        {
        }

        public IGroupChatCachePlugin ActivatePluginOnLoad { get; internal set; }
        public IMessageContainer ActivatePluginForGroupOnLoad { get; internal set; }
    }
}
