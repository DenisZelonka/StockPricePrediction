using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace dotnet.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(long x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        [DataMember(Name = "x")]
        public Nullable<long> X = null;
        [DataMember(Name = "y")]
		public Nullable<float> Y = null;
    }
}