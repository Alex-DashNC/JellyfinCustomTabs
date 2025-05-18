# Jellyfin Custom Tabs Plugin

A Jellyfin plugin that allows users to create and manage custom tabs in the Jellyfin web interface through a GUI.

## Features

- Add custom tabs to the Jellyfin web interface through an easy-to-use GUI
- Configure tab name, URL, icon, and whether to open in a new tab
- Choose from a selection of Material UI icons
- No need to manually edit configuration files

## Installation

### Automatic Installation (Recommended)

1. In your Jellyfin dashboard, go to the "Dashboard" section
2. Navigate to "Plugins" and then "Catalog"
3. Find "Custom Tabs" in the list of available plugins
4. Click "Install" to install the plugin
5. Restart Jellyfin when prompted

### Manual Installation

1. Download the latest release from the [releases page](https://github.com/yourusername/jellyfin-custom-tabs/releases)
2. Extract the downloaded file
3. Copy the DLL file to your Jellyfin plugins directory:
   - Windows: `%ProgramData%\Jellyfin\Server\plugins`
   - Linux: `/var/lib/jellyfin/plugins` or `/usr/share/jellyfin/plugins`
   - Docker: Use a volume mount to `/jellyfin/plugins`
4. Restart Jellyfin

## Usage

1. Go to the Jellyfin dashboard
2. Navigate to "Plugins"
3. Select "Custom Tabs" from the list of installed plugins
4. Click "Add Custom Tab" to create a new tab
5. Enter a name, URL, select an icon, and choose whether to open in a new tab
6. Save your changes
7. Refresh your Jellyfin web client to see the new tabs

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

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Jellyfin team for creating a great open-source media server
- All contributors to this project
