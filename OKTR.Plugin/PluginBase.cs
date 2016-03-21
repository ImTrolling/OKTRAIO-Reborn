using System;
using System.Linq;
using EloBuddy;

namespace OKTR.Plugin
{
    public class PluginAttribute : Attribute
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public Champion[] SupportedChampions { get; set; }

        public PluginAttribute(string name, string author, params Champion[] supportedChampions)
        {
            Name = name;
            Author = author;
            SupportedChampions = supportedChampions;
        }

    }
    public abstract class PluginBase
    {
        public PluginAttribute GetAttribute()
        {
            return (PluginAttribute) GetType().GetCustomAttributes(typeof(PluginAttribute), true).FirstOrDefault();
        }
        public abstract void Initialize();
    }
}