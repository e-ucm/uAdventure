# uAdventure

uAdventure is a Unity based framework for the creation of adventure games and educational games,
originally authored by the e-UCM Learning Group. This repository hosts the core engine plus the
Geo, Analytics, QR, Simva and Unity companion modules packaged as a single UPM package.

## Installing

Add the package to your project's `Packages/manifest.json`:

```json
"es.e-ucm.uadventure": "file:C:/path/to/UAdventurePackage"
```

## Modules

- `Runtime/uAdventure` — core engine (Scripts, Scenes, Prefabs, Animations, Shaders, Resources, Plugins).
- `Runtime/uAdventureGeo` — location-based adventures (MapzenGo mapping, GML import).
- `Runtime/uAdventureAnalytics` — in-game analytics reporting.
- `Runtime/uAdventureQR` — QR code generation.
- `Runtime/uAdventureSimva` — Simva platform integration (relies on `es.e-ucm.simva-unity-plugin` and `es.e-ucm.xasu`).
- `Runtime/uAdventureUnity` — auxiliary Unity helpers.

Editor-only code lives under `Editor/` mirroring the same module names.

## License

TBD