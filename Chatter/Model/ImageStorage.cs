using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    class ImageStorage
    {
        [PrimaryKey]
        public string id
        {
            get;
            set;
        }
        public string image
        {
            get;
            set;
        }
        public string username
        {
            get;
            set;
        }
        public string location
        {
            get;
            set;
        }
    }
}
