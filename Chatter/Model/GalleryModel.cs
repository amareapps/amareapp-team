using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    class GalleryModel
    {
       [PrimaryKey,AutoIncrement]
        public int id { get; set; }
        public string user_id { get; set; }
        public string is_dp { get; set; }
        public string image { get; set; }
        public string timestamp { get; set; }
    }
}
