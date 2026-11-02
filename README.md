# ShowPlayerOnMap – The Long Dark

A small MelonLoader mod for **The Long Dark** that shows **your own position** on the map in **Survival mode**, the same direction arrow you know from Wintermute, and **centers the map on you** when you open it.

- Direction arrow at your position, moves and rotates with you
- Map centers on you every time you open it (you can still pan freely afterwards)
- No new items, nothing is written to your save, safe to remove at any time

> 🇩🇪 Deutsche Kurzfassung weiter unten.

## Requirements

| | Version |
|---|---|
| The Long Dark | 2.55 (tested) |
| MelonLoader | 0.7.x (tested with 0.7.3) |

No other mods are required.

## Installation

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader) 0.7.x for The Long Dark and start the game once.
2. Download `ShowPlayerOnMap.dll` from the [Releases](https://github.com/KillerGanja/TLD-ShowPlayerOnMap/releases).
3. Put it into the `Mods` folder of your game, e.g. `…/steamapps/common/TheLongDark/Mods/`.
4. Start the game, load a Survival save and open the map.

**Linux / Steam Deck (Proton):** add `WINEDLLOVERRIDES="version=n,b" %command%` to the launch options so MelonLoader is loaded.

## Compatibility

- Tested together with Auto-Fill Map on Explore, MapIconTracker, Compass, Interloper HUD Pro, FreeLook, GunZoom, Skip Intro Reborn, Weight Fix, DeveloperConsole and ModSettings.
- Survival only. Wintermute (story mode) already shows the player arrow, and its episodes run in separate executables with their own `Mods` folders.

## How it works

The game already tracks your position on the map in Survival (`Panel_Map.m_PlayerIcon` is active and is moved and rotated every frame). It is invisible only because its sprite refers to the atlas **"Skills"**, which is **empty** in Survival. The sprite `ico_map_directional` exists in the **"Base Atlas"** that all other map icons use. The mod points the player icon to that atlas, raises its draw depth, and calls `Panel_Map.CenterOnPoint()` with your world position when the map is opened.

This is also why the console command `map_enablegps` seems to do nothing in Survival.

## Building from source

Requires the .NET SDK (6.0 or newer) and a game installation with MelonLoader that has been started once, so that `MelonLoader/Il2CppAssemblies` exists.

```bash
git clone https://github.com/KillerGanja/TLD-ShowPlayerOnMap.git
cd TLD-ShowPlayerOnMap
dotnet build -c Release -p:TLDPath="/path/to/TheLongDark"
# result: bin/Release/net6.0/ShowPlayerOnMap.dll
```

Without `-p:TLDPath` the default Steam path is used (`~/.local/share/Steam/steamapps/common/TheLongDark` on Linux, `C:\Program Files (x86)\Steam\steamapps\common\TheLongDark` on Windows).

## Troubleshooting

Check `MelonLoader/Latest.log` in the game folder. On success you will see:

```text
[ShowPlayerOnMap] loaded - shows your position on the map
[ShowPlayerOnMap] Player icon: atlas 'Skills' -> 'Base Atlas', sprite 'ico_map_directional'
```

If a game update breaks the mod, please open an [issue](https://github.com/KillerGanja/TLD-ShowPlayerOnMap/issues) with your game version and the log.

## License

[MIT](LICENSE)

---

## 🇩🇪 Kurzfassung

**ShowPlayerOnMap** zeigt im **Survival-Modus** deine eigene Position als Richtungspfeil auf der Karte, so wie in Wintermute, und **zentriert die Karte beim Öffnen auf dich**. Der Mod fügt keine Gegenstände hinzu, schreibt nichts in den Spielstand und kann jederzeit wieder entfernt werden.

**Installation:** MelonLoader 0.7.x installieren und das Spiel einmal starten. Dann `ShowPlayerOnMap.dll` aus den [Releases](https://github.com/KillerGanja/TLD-ShowPlayerOnMap/releases) in den `Mods`-Ordner des Spiels legen. Unter Linux/Proton als Startoption `WINEDLLOVERRIDES="version=n,b" %command%` eintragen.

**Ursache:** Das Spiel führt das Spielersymbol auch in Survival mit, es verweist aber auf den dort leeren Atlas „Skills“. Der Mod nimmt stattdessen dasselbe Bild aus dem „Base Atlas“.

Getestet mit The Long Dark 2.55 und MelonLoader 0.7.3.
