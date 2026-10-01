# Synchronized Tree Growth

A Stardew Valley mod that makes your farm's forestry much more predictable and visually satisfying. Instead of trees growing at random, independent rates, this mod synchronizes them so that all unfertilized trees on your farm advance to their next growth stage together on the exact same day.

## Features

* **Unified Forest Growth:** All unfertilized standard trees (Oak, Maple, Pine, Mahogany) share a single daily growth roll. When one grows, they all grow. 
* **Vanilla-Friendly Winter Rules:** Seamlessly respects standard game mechanics. Unfertilized trees will naturally pause their growth during the Winter season.
* **Full Tree Fertilizer Compatibility:** Tree Fertilizer works exactly as intended. Fertilized trees bypass the synchronization and grow one stage every single night, year-round, handled safely by the base game.
* **Lightweight & Safe:** Uses smart state-snapshotting to prevent double-growth bugs and minimizes performance impact by letting the base game handle edge cases.

## Installation

1. Install the latest version of [SMAPI](https://smapi.io/).
2. Download the latest release from the [Releases](../../releases) page (or Nexus Mods, if applicable).
3. Unzip the downloaded file and place the `SynchronizedTreeGrowth` folder into your `Stardew Valley/Mods` directory.
4. Launch the game using SMAPI.

## Compatibility

* Requires **Stardew Valley 1.6+**
* Requires **SMAPI 4.0.0+**
* Works in both single-player and multiplayer (install on the host machine).
* Safely ignores Fruit Trees, custom trees lacking standard properties, and fully grown trees.

## For Developers (Compiling from Source)

If you want to modify the code or adjust the internal growth chance (currently set to `0.99`), you can easily compile the mod yourself:

1. Clone this repository.
2. Ensure you have the [.NET 8.0 SDK](https://dotnet.microsoft.com/download) installed.
3. Open the `.sln` or `.csproj` in your preferred IDE (Visual Studio, Rider, VS Code) or run `dotnet build` from the command line.
4. The compiled mod will automatically deploy to your `Mods` folder if your system environment variables are set up for SMAPI, or you can manually copy the output from the `bin` folder.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
