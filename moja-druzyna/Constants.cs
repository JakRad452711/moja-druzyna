using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moja_druzyna
{
    public static class Constants
    {
        public static JObject getConstants()
        {
            return ((JObject)JObject.Parse(System.IO.File.ReadAllText(@"constants.json")));
        }
    }
}
