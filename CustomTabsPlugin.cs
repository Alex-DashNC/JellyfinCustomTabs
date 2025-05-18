// Plugin structure based on Jellyfin plugin architecture
// Main Plugin Class - CustomTabsPlugin.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.CustomTabs
{
    /// <summary>
    /// The main plugin class for Custom Tabs.
    /// </summary>
    public class CustomTabsPlugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        private readonly ILogger<CustomTabsPlugin> _logger;
        private readonly IServerConfigurationManager _configManager;
        private readonly IXmlSerializer _xmlSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTabsPlugin"/> class.
        /// </summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configurationManager">The server configuration manager.</param>
        public CustomTabsPlugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer,
            ILogger<CustomTabsPlugin> logger,
            IServerConfigurationManager configurationManager)
            : base(applicationPaths, xmlSerializer)
        {
            _logger = logger;
            _configManager = configurationManager;
            _xmlSerializer = xmlSerializer;
            
            Instance = this;
        }

        /// <summary>
        /// Gets the current plugin instance.
        /// </summary>
        public static CustomTabsPlugin? Instance { get; private set; }

        /// <inheritdoc />
        public override string Name => "Custom Tabs";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("1d46a759-264a-4e58-aa39-a733a9129731");

        /// <inheritdoc />
        public override string Description => "Create custom tabs in the Jellyfin web interface through a GUI";

        /// <summary>
        /// Gets the plugin's configuration.
        /// </summary>
        /// <returns>The configuration.</returns>
        public PluginConfiguration GetPluginConfiguration()
        {
            return Configuration;
        }

        /// <summary>
        /// Sets the plugin's configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void UpdateConfiguration(PluginConfiguration configuration)
        {
            Configuration = configuration;
            SaveConfiguration();
            
            // Update the web client config file
            UpdateWebClientConfiguration();
        }

        /// <summary>
        /// Updates the web client configuration file with the custom tabs.
        /// </summary>
        private void UpdateWebClientConfiguration()
        {
            try
            {
                // Get path to web client config file
                string webClientConfigPath = Path.Combine(
                    _configManager.ApplicationPaths.ProgramDataPath,
                    "config", 
                    "config.json");
                
                // Create config directory if it doesn't exist
                string configDir = Path.GetDirectoryName(webClientConfigPath);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                
                // Read existing config or create a new one
                Dictionary<string, object> config;
                if (File.Exists(webClientConfigPath))
                {
                    string configJson = File.ReadAllText(webClientConfigPath);
                    try
                    {
                        config = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(configJson)
                                 ?? new Dictionary<string, object>();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing web client config");
                        config = new Dictionary<string, object>();
                    }
                }
                else
                {
                    config = new Dictionary<string, object>();
                }
                
                // Convert custom tabs to the format expected by the web client
                var customTabs = Configuration.CustomTabs.Select(tab => new
                {
                    name = tab.Name,
                    url = tab.Url,
                    icon = tab.Icon,
                    openInNewTab = tab.OpenInNewTab
                }).ToList();
                
                // Update the customTabs property
                config["customTabs"] = customTabs;
                
                // Write the updated config back to the file
                string updatedConfig = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                File.WriteAllText(webClientConfigPath, updatedConfig);
                
                _logger.LogInformation("Updated web client config with custom tabs");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating web client configuration");
            }
        }

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = Name,
                    EmbeddedResourcePath = $"{GetType().Namespace}.Configuration.configPage.html"
                }
            };
        }
    }
}