﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.Options
{
    public class CloudinaryOptions
    {
        public string? ApiKey {  get; set; }
        public string? ApiSecret { get; set; }
        public string? CloudName { get; set; }
    }
}
