using MIWE.Core.Interfaces;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MIWE.Core.Models;
using System.Threading.Tasks;

namespace MIWE.Core
{
    public class PluginRunner : IPluginRunner
    {
        private static ILogger _logger;
        private static object _lock;
        public PluginRunner(ILogger<PluginRunner> logger)
        {
            _logger = logger;
        }

        static Assembly MyResolveEventHandler(object source, ResolveEventArgs e)
        {
            lock (_lock)
            {
                try
                {
                    Console.WriteLine("Resolving {0}", e.Name);
                    return Assembly.Load(e.Name);
                }
                catch (Exception err)
                {
                    _logger.Log(LogLevel.Error, err, "Plugin resolve event handler failed");
                    throw;
                }
            }
        }

        public bool Run(string crawlPath, CancellationToken? cancellationToken = null)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);

                crawlPath = FindCrawlDLL<ICrawl>(crawlPath);

                var loader = PluginLoader.CreateFromAssemblyFile(
                            crawlPath,
                            sharedTypes: new[] { typeof(ICrawl) });
                var pluginType = loader
                    .LoadDefaultAssembly()
                        .GetTypes()
                        .Where(t => typeof(ICrawl).IsAssignableFrom(t) && !t.IsAbstract)
                        .FirstOrDefault();

                // This assumes the implementation of IPlugin has a parameterless constructor
                ICrawl plugin = (ICrawl)Activator.CreateInstance(pluginType);

                bool result = plugin.ScrapeData(cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Plugin run failed");
                return false;
            }
        }

        private string FindCrawlDLL<TInstance>(string crawlPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(crawlPath);
            var files = directoryInfo.GetFiles("*.dll");
            foreach (var file in files)
            {
                var nextAssembly = Assembly.LoadFrom(file.FullName);
                if (nextAssembly.GetTypes().Any(n => !n.IsInterface && typeof(TInstance).IsAssignableFrom(n)))
                {
                    crawlPath = file.FullName;
                    break;
                }
            }

            return crawlPath;
        }

        public async Task<bool> Run(PluginRunningParameters pluginRunningParameters, CancellationToken? cancellationToken)
        {
            try
            {
                ICrawl crawlPlugin = CreateInstanceOfPlugin<ICrawl>(pluginRunningParameters.CrawlerPluginPath);

                crawlPlugin.ScrapeData(cancellationToken);
                bool result = false;
                if (pluginRunningParameters.IsProcessorAssigned())
                {
                    var productsData = crawlPlugin.GetData();
                    IProcess processPlugin = CreateInstanceOfPlugin<IProcess>(pluginRunningParameters.ProcessorPluginPath);
                    result = await processPlugin.ProcessData(pluginRunningParameters.MerchantName, productsData, pluginRunningParameters.ProcessorSaveAction);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Plugin run failed");
                return false;
            }
        }

        public TInstance CreateInstanceOfPlugin<TInstance>(string pluginPath)
        {
            try
            {
                pluginPath = FindCrawlDLL<TInstance>(pluginPath);
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);

                var loader = PluginLoader.CreateFromAssemblyFile(
                            pluginPath,
                            sharedTypes: new[] { typeof(TInstance) });
                var pluginType = loader
                    .LoadDefaultAssembly()
                        .GetTypes()
                        .Where(t => typeof(TInstance).IsAssignableFrom(t) && !t.IsAbstract)
                        .FirstOrDefault();

                // This assumes the implementation of IProcess has a parameterless constructor
                TInstance plugin = (TInstance)Activator.CreateInstance(pluginType);

                Console.WriteLine($"Created plugin instance.");

                return plugin;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Plugin instantiation failed");
                throw;
            }
        }

        public void test()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);

                var loaders = new List<PluginLoader>();

                // create plugin loaders
                var pluginsDir = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
                foreach (var dir in DirSearch(pluginsDir))
                {
                    var pluginDll = Path.GetFileName(dir);

                    var loader = PluginLoader.CreateFromAssemblyFile(
                        dir,
                        sharedTypes: new[] { typeof(ICrawl) });
                    loaders.Add(loader);

                }

                // Create an instance of plugin types
                foreach (var loader in loaders)
                {
                    foreach (var pluginType in loader
                        .LoadDefaultAssembly()
                        .GetTypes()
                        .Where(t => typeof(ICrawl).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        // This assumes the implementation of IPlugin has a parameterless constructor
                        ICrawl plugin = (ICrawl)Activator.CreateInstance(pluginType);

                        Console.WriteLine($"Created plugin instance.");
                       
                        plugin.ScrapeData();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private  List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            foreach (string f in Directory.GetFiles(sDir).Where(n => n.EndsWith(".dll")))
            {
                files.Add(f);
            }
            foreach (string d in Directory.GetDirectories(sDir))
            {
                files.AddRange(DirSearch(d));
            }

            return files;
        }
    } 
}
