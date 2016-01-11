using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Infrastructure
{
    public static class SettingsExtensions
    {
        public static Dictionary<string, string> ToDictionary(this ApplicationSettingsBase settings)
        {
            return settings.Properties
                          .Cast<SettingsProperty>()
                          .ToDictionary(k => k.Name, v => v.DefaultValue
                                                           .ToString());
        }
    }
}
