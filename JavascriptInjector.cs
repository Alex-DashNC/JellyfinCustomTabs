// JavascriptInjector.cs

using System;
using System.IO;
using System.Reflection;
using System.Text;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.IO;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.CustomTabs
{
    /// <summary>
    /// Injects custom JavaScript into the web client.
    /// </summary>
    public class JavascriptInjector : IServerEntryPoint
    {
        private readonly IServerConfigurationManager _configManager;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<JavascriptInjector> _logger;

        private string _scriptsPath;
        private bool _webFilesExist;

        /// <summary>
        /// Initializes a new instance of the <see cref="JavascriptInjector"/> class.
        /// </summary>
        /// <param name="configManager">Instance of <see cref="IServerConfigurationManager"/>.</param>
        /// <param name="fileSystem">Instance of <see cref="IFileSystem"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger{JavascriptInjector}"/>.</param>
        public JavascriptInjector(
            IServerConfigurationManager configManager,
            IFileSystem fileSystem,
            ILogger<JavascriptInjector> logger)
        {
            _configManager = configManager;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Run()
        {
            try
            {
                // Determine scripts path in the web directory
                _scriptsPath = Path.Combine(_configManager.ApplicationPaths.WebPath, "scripts", "customtabs");

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(_scriptsPath);

                // Deploy client script
                DeployClientScript();
                
                // Create an initial shared tabs file if it doesn't exist
                string sharedTabsPath = Path.Combine(_scriptsPath, "shared-tabs.json");
                if (!File.Exists(sharedTabsPath))
                {
                    File.WriteAllText(sharedTabsPath, "[]");
                }

                _webFilesExist = true;

                _logger.LogInformation("Custom Tabs Plugin - JavaScript injected successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deploying custom tabs client script");
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the resources.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_webFilesExist)
                {
                    try
                    {
                        // Clean up deployed files but preserve the shared tabs data
                        string sharedTabsPath = Path.Combine(_scriptsPath, "shared-tabs.json");
                        string sharedTabsData = "[]";
                        if (File.Exists(sharedTabsPath))
                        {
                            sharedTabsData = File.ReadAllText(sharedTabsPath);
                        }
                        
                        // Remove script files
                        if (File.Exists(Path.Combine(_scriptsPath, "customTabs.js")))
                        {
                            File.Delete(Path.Combine(_scriptsPath, "customTabs.js"));
                        }
                        
                        if (File.Exists(Path.Combine(_scriptsPath, "customTabs.html")))
                        {
                            File.Delete(Path.Combine(_scriptsPath, "customTabs.html"));
                        }
                        
                        // Restore the shared tabs
                        File.WriteAllText(sharedTabsPath, sharedTabsData);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error cleaning up custom tabs client scripts");
                    }
                }
            }
        }

        private void DeployClientScript()
        {
            var assembly = GetType().Assembly;
            var resourceName = "Jellyfin.Plugin.CustomTabs.Resources.client-side-script.js";

            // Extract and save the client script
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var scriptContent = reader.ReadToEnd();
                        var filePath = Path.Combine(_scriptsPath, "customTabs.js");
                        File.WriteAllText(filePath, scriptContent);
                    }
                }
                else
                {
                    _logger.LogError("Could not find embedded resource: {ResourceName}", resourceName);
                }
            }

            // Create a HTML injection script that will be loaded in the web client
            var injectionScript = @"
<script>
    // Load Custom Tabs script
    var script = document.createElement('script');
    script.src = 'scripts/customtabs/customTabs.js';
    document.head.appendChild(script);
</script>
";
            var injectionFilePath = Path.Combine(_scriptsPath, "customTabs.html");
            File.WriteAllText(injectionFilePath, injectionScript);
        }
    }
}
