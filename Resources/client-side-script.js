// client-side-script.js - This will be injected into the web client

// Register script as a plugin for Jellyfin web client
(function() {
    'use strict';
    
    // Plugin ID should match the server-side plugin
    const pluginId = '1d46a759-264a-4e58-aa39-a733a9129731';
    const pluginName = 'Custom Tabs';

    // Register the plugin
    if (window.Emby && window.Emby.PluginManager) {
        window.Emby.PluginManager.register({
            id: pluginId,
            name: pluginName,
            type: 'skin',
            version: '1.0.0'
        });
    }

    // Function to load custom tabs from the server
    async function loadCustomTabs() {
        try {
            // Load tabs from the shared file on the server
            const response = await fetch('scripts/customtabs/shared-tabs.json?nocache=' + new Date().getTime());
            if (!response.ok) {
                return [];
            }
            
            const data = await response.json();
            return data || [];
        } catch (ex) {
            console.error('Error loading shared custom tabs', ex);
            return [];
        }
    }

    // Function to add custom tabs to navigation menu
    async function addCustomTabs() {
        const customTabs = await loadCustomTabs();
        if (!customTabs || !customTabs.length) {
            return;
        }

        // Wait for the navigation menu to be available
        const checkNavInterval = setInterval(() => {
            const navDrawer = document.querySelector('.mainDrawer');
            const navDrawerItems = document.querySelector('.mainDrawer-scrollContainer');
            
            if (navDrawer && navDrawerItems) {
                clearInterval(checkNavInterval);
                
                // Remove any existing custom tabs
                const existingCustomTabs = navDrawerItems.querySelectorAll('.customTab');
                existingCustomTabs.forEach(tab => tab.parentNode.removeChild(tab));
                
                // Add custom tabs
                customTabs.forEach(tab => {
                    const navItem = document.createElement('a');
                    navItem.className = 'navMenuOption customTab';
                    navItem.href = tab.url;
                    if (tab.openInNewTab) {
                        navItem.target = '_blank';
                    }
                    
                    const icon = document.createElement('span');
                    icon.className = 'material-icons navMenuOptionIcon';
                    icon.textContent = tab.icon || 'link';
                    
                    const text = document.createElement('span');
                    text.className = 'navMenuOptionText';
                    text.textContent = tab.name;
                    
                    navItem.appendChild(icon);
                    navItem.appendChild(text);
                    
                    navDrawerItems.appendChild(navItem);
                });
            }
        }, 500);
    }

    // Initial setup - add tabs when page loads
    window.addEventListener('load', () => {
        addCustomTabs();
    });

    // Listen for navigation events to re-add tabs when the UI reloads
    window.addEventListener('viewshow', () => {
        addCustomTabs();
    });

    // Listen for periodic refresh - Check every 5 minutes for updates to tabs
    setInterval(() => {
        addCustomTabs();
    }, 5 * 60 * 1000); // 5 minutes
    
    // Listen for custom refresh messages from the server
    if (window.ApiClient) {
        window.ApiClient.on('message', (message) => {
            if (message && message.MessageType === 'ForceKeepAlive' &&
                message.Data.includes('Custom tabs configuration has been updated')) {
                // Reload tabs immediately
                addCustomTabs();
            }
        });
    }
})();
