# Jellyfin Custom Tabs Plugin

A Jellyfin plugin that allows administrators to create and manage custom tabs in the Jellyfin web interface through a GUI. Custom tabs are visible to all users of the server.

## Features

- Add custom tabs to the Jellyfin web interface through an easy-to-use GUI
- Configure tab name, URL, icon, and whether to open in a new tab
- Choose from a selection of Material UI icons
- No need to manually edit configuration files
- Server-side implementation - tabs are shared among all users
- Administrators can manage tabs for all users in one place

## Screenshots

![Custom Tabs Configuration](screenshots/config-page.jpg)
![Custom Tabs in Jellyfin](screenshots/custom-tabs.jpg)

## Installation

### Manual Installation from GitHub Repo

1. In your Jellyfin dashboard, go to the "Dashboard" section
2. Navigate to "Plugins" and then click on "Repositories"
3. Add this repository URL: `https://github.com/Alex-DashNC/jellyfin-custom-tabs`
4. Go to "Catalog" and find "Custom Tabs" in the list
5. Click "Install" to install the plugin
6. Restart Jellyfin when prompted

## Usage

1. Go to the Jellyfin dashboard
2. Navigate to "Plugins"
3. Select "Custom Tabs" from the list of installed plugins
4. Click "Add Custom Tab" to create a new tab
5. Enter a name, URL, select an icon, and choose whether to open in a new tab
6. Save your changes
7. Refresh your Jellyfin web client to see the new tabs

## FAQ

### Why don't I see my custom tabs?
The tabs are stored in your browser's local storage. Make sure you:
- Refreshed the page after saving your tabs
- Are using the same browser you used to create the tabs
- Have JavaScript enabled in your browser

### Who can see the custom tabs?
All users accessing your Jellyfin server will see the custom tabs you've configured.

### Can different users have different tabs?
No, the custom tabs are defined at the server level and are shared by all users. This plugin is designed to provide consistent navigation options for all users of your Jellyfin server.

### Can I limit some tabs to specific users?
Currently the plugin doesn't support user-specific tabs. All configured tabs will be visible to all users.

## Development

### Prerequisites

- .NET 6.0 SDK or higher
- A development environment set up for Jellyfin plugin development

### Building the Plugin

1. Clone the repository:
```
git clone https://github.com/yourusername/jellyfin-custom-tabs.git
```

2. Build the plugin:
```
cd jellyfin-custom-tabs
dotnet build
```

3. The compiled plugin dll will be located in the `bin` directory

## Project Structure

- **CustomTabsPlugin.cs**: Main plugin class
- **PluginConfiguration.cs**: Configuration structure for storing custom tabs
- **JavascriptInjector.cs**: Injects client-side code into the Jellyfin web UI
- **Resources/client-side-script.js**: Client-side script that adds tabs to the UI
- **Configuration/configPage.html**: Dashboard configuration page

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Jellyfin team for creating a great open-source media server
- All contributors to this project