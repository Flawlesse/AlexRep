using System;

namespace STW_Service
{
    public class ParsableAttribute : Attribute
    {
        public string Alias { get; set; }

        public ParsableAttribute()
        {
            Alias = "add";
        }

        public ParsableAttribute(string val)
        {
            Alias = val;
        }
    }
}
