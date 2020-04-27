
using Chatter.Model;
using Chatter.View.Cells;
using Xamarin.Forms;

namespace ChatUIXForms.Helpers
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate outgoingDataTemplate;

        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as ChatModel;
            if (messageVm == null)
                return null;
            return (messageVm.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"","")) ? outgoingDataTemplate : incomingDataTemplate;
        }
    }
}