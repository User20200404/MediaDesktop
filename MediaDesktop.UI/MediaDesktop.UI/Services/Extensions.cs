using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;
using IniParser.Parser;
using System.IO;
using IniParser;
namespace MediaDesktop.UI.Services
{
    public static class Extensions
    {
        public static void WriteFileMerged(this FileIniDataParser parser, string path, IniData iniData)
        {
            IniData data;
            if (File.Exists(path))
            {
                data = parser.ReadFile(path);
                data.Merge(iniData);
            }
            else
            {
                data = iniData;
            }
            parser.WriteFile(path, data);
        }

        public static string GetValueOrDefault(this IniData iniData, string sectionName, string keyName, string defaultValue = null)
        {
            if(iniData.Sections.ContainsSection(sectionName))
            {
                if (iniData.Sections[sectionName].ContainsKey(keyName))
                {
                    return iniData.Sections[sectionName][keyName]; 
                }
            }

            return defaultValue;
        }

        public static int GetIntValueOrDefault(this IniData iniData, string sectionName, string keyName, int defaultValue = 0)
        {
            string value_string = GetValueOrDefault(iniData, sectionName, keyName, "0");
            int.TryParse(value_string, out int result);
            return result;
        }
    }
}
