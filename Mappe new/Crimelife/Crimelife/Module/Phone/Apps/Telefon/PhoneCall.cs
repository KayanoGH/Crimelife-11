using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    class PhoneCall
    {
        public int Id { get; set; }
        public DbPlayer Player1 { get; set; }
        public DbPlayer Player2 { get; set; }

        public PhoneCall() { }
    }
}
