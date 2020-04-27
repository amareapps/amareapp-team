using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using SQLite;

namespace Chatter.Model
{
    class SearchRefenceModel
    {
        [PrimaryKey]
        public string user_id
        {
            get; set;
        }
        public string maximum_distance
        {
            get;
            set;
        }
        public string age_range
        {
            get;
            set;
        }
    }
}
