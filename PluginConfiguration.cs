// PluginConfiguration.cs

using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.CustomTabs
{
    /// <summary>
    /// Class PluginConfiguration
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            CustomTabs = new List<CustomTab>();
        }

        /// <summary>
        /// Gets or sets the list of custom tabs.
        /// </summary>
        public List<CustomTab> CustomTabs { get; set; }
    }

    /// <summary>
    /// Class representing a custom tab.
    /// </summary>
    public class CustomTab
    {
        /// <summary>
        /// Gets or sets the name of the tab.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL of the tab.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the icon to use for the tab.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to open the tab in a new window.
        /// </summary>
        public bool OpenInNewTab { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTab"/> class.
        /// </summary>
        public CustomTab()
        {
            Name = string.Empty;
            Url = string.Empty;
            Icon = "link";  // Default Material UI icon
            OpenInNewTab = false;
        }
    }
}