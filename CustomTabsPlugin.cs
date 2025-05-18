// Plugin structure based on Jellyfin plugin architecture
// Main Plugin Class - CustomTabsPlugin.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Session;
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
        private readonly ISessionManager _sessionManager;
        private readonly IXmlSerializer _xmlSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTabsPlugin"/> class.
        /// </summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configurationManager">The server configuration manager.</param>
        /// <param name="sessionManager">The session manager.</param>
        public CustomTabsPlugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer,
            ILogger<CustomTabsPlugin> logger,
            IServerConfigurationManager configurationManager,
            ISessionManager sessionManager)
            : base(applicationPaths, xmlSerializer)
        {
            _logger = logger;
            _configManager = configurationManager;
            _sessionManager = sessionManager;
            _xmlSerializer = xmlSerializer;
            
            Instance = this;
            
            // Update the shared tabs file when the plugin starts
            UpdateSharedTabsFile();
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
        public async Task UpdateConfiguration(PluginConfiguration configuration)
        {
            Configuration = configuration;
            SaveConfiguration();
            
            // Update the shared tabs file
            UpdateSharedTabsFile();
            
            // Send message to clients to refresh
            try {
                await _sessionManager.SendMessageToSessions(new SessionMessageType[] 
                { 
                    SessionMessageType.ForceKeepAlive
                }, 
                "Custom tabs configuration has been updated. Please refresh your browser to see changes.", 
                CancellationToken.None);
                
                _logger.LogInformation("Sent refresh message to clients");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error sending refresh message to clients");
            }
        }
        
        /// <summary>
        /// Updates the shared tabs file with the current configuration.
        /// </summary>
        private void UpdateSharedTabsFile()
        {
            try
            {
                // Format tabs for web client
                var webClientTabs = Configuration.CustomTabs.Select(tab => new
                {
                    name = tab.Name,
                    url = tab.Url,
                    icon = tab.Icon,
                    openInNewTab = tab.OpenInNewTab
                }).ToList();
                
                // Convert to JSON
                string tabsJson = System.Text.Json.JsonSerializer.Serialize(webClientTabs, 
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                
                // Determine path for shared tabs file
                string scriptsPath = Path.Combine(_configManager.ApplicationPaths.WebPath, "scripts", "customtabs");
                Directory.CreateDirectory(scriptsPath);
                
                string sharedTabsPath = Path.Combine(scriptsPath, "shared-tabs.json");
                
                // Write to the file
                File.WriteAllText(sharedTabsPath, tabsJson);
                
                _logger.LogInformation("Updated shared tabs file with {Count} tabs", webClientTabs.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shared tabs file");
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