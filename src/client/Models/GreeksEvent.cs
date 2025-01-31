﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary;

namespace client.Models
{
    public class GreeksData
    {        
        public Option Option { get; set; }
        public GreeksDataWrapper Greeks { get; set; }
        public double Charm { get; set; }
        public double SkewSensi { get; set; }
        public double CallConvexSensi { get; set; }
        public double PutConvexSensi { get; set; }
    }

    public class GreeksEvent : PubSubEvent<GreeksData>
    {
    }
}
