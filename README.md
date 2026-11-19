# Gen3 Save Toolkit (Pokémon Emerald / Ruby / Sapphire)

A tiny command-line tool that edits Gen 3 Pokémon saves headlessly, using
[PKHeX.Core](https://github.com/kwsch/PKHeX) as the save engine — no GUI, no clicking.

It does two things:

1. **Fills every item pouch** — Items, Key Items, Poké Balls, TMs/HMs, Berries and
   PC Items — giving every valid item in every slot at a chosen quantity (default 999,
   the maximum the games display correctly).
2. **Rebuilds the party** with six level-100 legendaries, with OT/TID/SID/language
   copied from your own save so they behave like yours.

## Why

On a real GBA (or a handheld emulator with no cheat menu), GameShark/CodeBreaker codes
have nowhere to run. A save editor sidesteps that entirely: the values live in the
save file, so they survive every reload and every emulator.

## Usage

```bash
dotnet run -- <path-to-save.srm> [quantity]
```

Example:

```bash
dotnet run -- "Pokemon - Emerald Version (U).srm" 999
```

Back up your save first. The tool overwrites the file in place (it does verify by
reloading afterwards).

## Requirements

- .NET 10 SDK
- `PKHeX.Core` NuGet package (restored automatically)

## How it works

- `SaveUtil.GetSaveFile(path)` → `SAV3` object
- `sav.Inventory.Pouches` → `pouch.GiveAllItems(inv, qty)` for each pouch
- `sav.BlankPKM` → set species/level/OT/IDs → `sav.SetPartySlotAtIndex(pk, i)`
- party count is set through the internal setter (reflection), since `SAV3.PartyCount`
  has no public setter
- `sav.Write()` writes the file back, checksums included

## Caveats

- Quantity is capped at 999 in this build; the field is 2 bytes but higher values
  display as garbage and can glitch menus.
- Legendaries are *legal-flagged* by PKHeX (no real event origin). Offline games do
  not check, so they work fine.
- Only Gen 3 saves (Emerald / Ruby / Sapphire / FireRed / LeafGreen) — this is a
  `SAV3`-specific tool.

## License

GPL-3.0, matching PKHeX.Core, which this project links against.
PKHeX is © Kurt (kwsch) and contributors.
