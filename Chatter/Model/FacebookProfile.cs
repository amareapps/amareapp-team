using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Chatter.Model
{
    class FacebookProfile
    {
        public string Name { get; set; }
        public Picture Picture { get; set; }
        public string Locale { get; set; }
        public string Gender { get; set; }
        public string Id { get; set; }
    }

    public class Picture
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public bool IsSilhouette { get; set; }
        public string Url { get; set; }
    }

    public class Cover
    {
        public string Id { get; set; }
        public int OffsetY { get; set; }
        public string Source { get; set; }
    }

    public class AgeRange
    {
        public int Min { get; set; }
    }
}
