using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Classes
{
    public class ListManager
    {
        private IRefreshInbox _logger;
        public ListManager(IRefreshInbox logger)
        {
            _logger = logger;
        }

        public void RefreshInbox()
        {
            _logger.refreshInbox();
        }
    }
}
