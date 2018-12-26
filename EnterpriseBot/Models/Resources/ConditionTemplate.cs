using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnterpriseBot.Models.Resources
{
    public static class ConditionTemplate
    {
        private static Dictionary<string, ConditionType> templateMapping;

        static ConditionTemplate()
        {
            // Read the regexs from data file.
            templateMapping = new Dictionary<string, ConditionType>();
            var dir = Path.GetDirectoryName(typeof(ConditionModel).Assembly.Location);
            var resDir = Path.Combine(dir, @"Models\Resources\ConditionTemplate.txt");
            StreamReader sr = new StreamReader(resDir, Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("#"))
                {
                    // Language tag, should skip and continue.
                    continue;
                }

                string[] parts = line.Split("\t");
                if (parts[0].Length > 0 && parts[1].Length > 0)
                {
                    ConditionType conditionType = Enum.Parse<ConditionType>(parts[0], true);
                    templateMapping.Add(parts[1], conditionType);
                }
            }
        }

        public static Dictionary<ConditionType, string> GetConditionTypes(string content)
        {
            // return all parameter types that matches this content
            // for example, when and where will match ask location and ask time
            Dictionary<ConditionType, string> types = new Dictionary<ConditionType, string>();
            if (string.IsNullOrEmpty(content))
            {
                return types;
            }

            foreach (string key in templateMapping.Keys)
            {
                Regex regex = new Regex(key);
                if (regex.IsMatch(content))
                {
                    types.Add(templateMapping[key], regex.Split(content)[1]);
                }
            }

            return types;
        }
    }
}
