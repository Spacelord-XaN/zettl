# zettl

A small cross-platform tool for managing and pasting named text fragments.

## Usage

```
zettl --edit             Open the fragment editor
zettl --paste [name]     Copy a fragment to the clipboard
```

### Editor (`zettl --edit`)

Opens a GUI window with a two-panel layout:

- **Left** – list of all saved fragments
- **Right** – preview of the selected fragment's text

Toolbar actions:

| Button | Action |
|--------|--------|
| ➕ Add | Open the editor to create a new fragment |
| ✏️ Edit | Edit the selected fragment (or double-click) |
| 🗑 Delete | Delete the selected fragment |

Changes are saved automatically. Press **Escape** to close the editor or cancel a dialog.

### Paste (`zettl --paste [name]`)

Copies the text of the named fragment to the system clipboard. Name matching is case-insensitive.

```sh
zettl --paste greeting
```

If no name is given, a selection window opens with a fuzzy-search box. Type to filter, use **↑ / ↓** to move through results, **Enter** to confirm, **Escape** to cancel.

If no clipboard tool is available the text is written to stdout instead, so piping works too:

```sh
zettl --paste greeting | xdotool type --clearmodifiers --file -
```

## Storage

Fragments are stored in a plain XML file:

| Platform | Path |
|----------|------|
| Linux    | `~/.config/zettl/fragments.xml` |
| macOS    | `~/Library/Application Support/zettl/fragments.xml` |
| Windows  | `%APPDATA%\zettl\fragments.xml` |

The file is created automatically on first save.

### Format

```xml
<?xml version="1.0" encoding="utf-8"?>
<Zettl>
  <Fragment name="greeting">Hello, World!</Fragment>
</Zettl>
```

## Building

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
dotnet build
dotnet run -- --edit
```

## Dependencies

- [Avalonia UI](https://avaloniaui.net/) — cross-platform UI framework
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) — MVVM helpers

No other third-party packages.

## License

MIT
