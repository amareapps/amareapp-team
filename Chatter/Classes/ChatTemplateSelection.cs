
using Android.OS;
using Chatter.Model;
using Chatter.View.Cells;
using Xamarin.Forms;

namespace ChatUIXForms.Helpers
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate outgoingDataTemplate;
        DataTemplate incomingDataTemplateImage;
        DataTemplate outgoingDataTemplateImage;
        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
            this.incomingDataTemplateImage = new DataTemplate(typeof(IncomingViewCellImage));
            this.outgoingDataTemplateImage = new DataTemplate(typeof(OutgoingViewCellImage));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as ChatModel;
            if (messageVm == null)
                return null;
            if(messageVm.message.Contains("chatter-7b8e4"))
                return (messageVm.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"", "")) ? outgoingDataTemplateImage : incomingDataTemplateImage;
            
            return (messageVm.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"","")) ? outgoingDataTemplate : incomingDataTemplate;
        }
    }
}