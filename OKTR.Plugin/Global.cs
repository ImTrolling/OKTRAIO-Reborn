using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Events;

namespace OKTR.Plugin
{
    public static class Global
    {
        internal static List<PluginBase> Plugins = new List<PluginBase>();
        private static bool _setEvents;

        public static void RegisterPlugin<T>()
        {
            if (typeof (T).IsAssignableFrom(typeof (PluginBase)) && !typeof(T).IsAbstract)
            {
                RegisterPlugin((PluginBase) Activator.CreateInstance(typeof(T)));
            }
        }

        public static void RegisterPlugin(this PluginBase pluginBase)
        {
           if(pluginBase == null) return;
           Plugins.Add(pluginBase);
            if (!_setEvents) SetEvents();
        }

        private static void SetEvents()
        {
            Loading.OnLoadingComplete += a => Initialize();
            _setEvents = true;
        }

        public static void Initialize()
        {
            Console.WriteLine("Loading OKTR Core....");
            OKTR_Core.InitCore();
            Console.WriteLine("Loaded OKTR Core!");
            Console.WriteLine("");
            Console.WriteLine("Loading {0} Plugins....", Plugins.Count);
            Plugins.ForEach(LoadPlugin);
            Console.WriteLine("Loaded All Plugins!");
        }

        private static void LoadPlugin(PluginBase pluginBase)
        {
            var attribute = pluginBase.GetAttribute();
            if (attribute != null)
            {
               Console.WriteLine("Found Plugin: {0}", pluginBase.GetType().Name);
                if (attribute.SupportedChampions.Contains(Player.Instance.Hero))
                {
                    if (Plugins.Count(x => x.GetAttribute().SupportedChampions.Contains(Player.Instance.Hero)) > 1)
                    {
                        Console.WriteLine("Many Plugin found for current champion, returning for now");
                        //TODO: Plugin Selector
                        return;
                    }  
                    pluginBase.Initialize();
                } 
            }
        }
    }

}