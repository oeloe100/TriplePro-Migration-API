﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class MetaGroup
    {
        [JsonProperty("id")]
        public int? Id { get; set; }
    }
}
