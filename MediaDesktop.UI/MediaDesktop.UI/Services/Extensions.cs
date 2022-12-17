using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;
using IniParser.Parser;
using System.IO;
using IniParser;
using System.Reflection;

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

        public static string GetStringValueOrDefault(this IniData iniData, string sectionName, string keyName, string defaultValue = null)
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

        public static T GetValueTypeValueOrDefault<T>(this IniData iniData, string sectionName, string keyName, T defaultValue = default(T)) where T : struct
        {
            string value_string = GetStringValueOrDefault(iniData, sectionName, keyName, "0");
            Type type = typeof(T);
            object value = type.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new string[] { value_string });
            return (T)value;
        }
    }
}
