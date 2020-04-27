using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    class RecentMatchesModel
    {
        [PrimaryKey]
        public string user_id { get; set; }
        public string username { get; set; }
        public string image { get; set; }
        public string datetime { get; set; }
    }
}
