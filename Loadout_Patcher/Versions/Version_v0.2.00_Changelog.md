# Changelog - Patcher GUI

## Current version: v0.2.00
Commit ID: edcee76e3b73f8dd37d3c509ca0bdf27905a52f0

## Previous version: v0.10
Commit ID: 538a43546ca2c08dbb44ed66c73543925f01593b

### This is the first major update after the initial release

#### What's new?

___

1. #### Instant patching

- Checkbox added in options tab
- Full functionality implemented
- Guaranteed instant patching when Loadout is already open
- Can be combined with autostarting Loadout on startup of the patcher GUI

___

2. #### Autostarting Loadout on startup

- Checkbox added in options tab
- Full functionality implemented
- Reattempts for a few seconds due to delay when starting Loadout automatically
- Autostarting Loadout by the OS takes up to 1 second
- Can be combined with SSELauncher preference

___

3. #### SSELauncher preference

- Checkbox added in options tab
- Full functionality implemented
- Conditional SSELauncher preference: autostarts only if the user started Loadout via SSE manually before

___

4. #### SSE shortcut creation

- Checkbox added in options tab
- Shortcut generation inspired by SSE's generated shortcut for Loadout
- Added shortcut hotkey Ctrl+Shift+L (L for Loadout)
- Full functionality implemented

___

5. #### SSE shortcut deletion

- SSEShortcut will still be created but deleted instantly if the checkbox for SSE shortcut creation wasn't checked

___

6. #### In multiplayer tab, simulation of the server/session list

- Table created as DataGrid
- Complete design in .axaml
- Refresh button cooldown
- Reloading refresh button and current time of the day once a second
- Join button disabling after a few clicks
- Deep join button logic to prevent abusive use
- Join button enabling after refresh button press
- Separate animation on refresh button cooldown
- Console output
- Current time of the day in format HH:mm:ss
- Removing and adding favorite sessions
- Removing and adding favorite maps
- Simulation tasks such as random ping, server availability, player counter
- Example data with all attributes
- Added MultiplayerSession class for saving only the IP address and server name of the favorite server
- Save data functionality of MultiplayerSession
- Two different TabControl tabs for all servers and favorite servers
- Different conditional colors for the ping by using green, yellow and red colors
- Two new ObservableObject classes for countdown and game servers

___

7. #### Added resource finder for the new star icons in multiplayer tab

___

8. #### Highlighting Loadout process when it's open in multiple cases

- After patching a map in the map patching tab
- After joining a (simulated) multiplayer session in the multiplayer tab
- Generally after switching to the patch for game access tab

___

9. #### Two different custom cursors

- Primary cursor: "Dragonbone Greatsword" from The Elder Scrolls V: Dawnguard

> [Click to open Dragonbone Greatsword image](https://static.wikia.nocookie.net/elderscrolls/images/2/25/Dragontwohander.png)

The sword was chosen as cursor because of 2 main colors that make it stand out in dark areas as well as bright areas. It is also pointed at one end.

- Secondary cursor, appearance in any DataGrid: "The Juice" from Loadout

> [Click to open The Juice image](https://static.wikia.nocookie.net/loadout/images/3/35/The_Juice.png)

___

10. #### Cursors positioned and assigned with bitmap and cursor classes

___

11. #### Added an easter egg as image

___

12. #### Started inter-process communication setup (Linux)

___

13. #### In map patching tab, added dev map checkbox with full functionality

___

14. #### In minigame tab, added variable image size for each field depending on row-column-counter

___

15. #### Bug fixes

- Directory separator fixed (Linux)
- In minigame tab, empty space between fields removed when row-column-counter is set to below average
- Patching the starting map in lower case. Mix-up between map name and alternative map name fixed.

___

16. #### Other minor changes

- Options reset button updated
- Filters reset button updated
- Overall save data management
- And more

___

# Thank you for using the patcher GUI

## License

This project is licensed under the [Eclipse Public License v2.0](https://www.eclipse.org/legal/epl-2.0/).  
© 2025 Rasagiline ([GitHub](https://github.com/Rasagiline)) — Loadout_Patcher