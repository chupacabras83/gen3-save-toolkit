# Gen3 Save Toolkit (Pokémon Emerald / Ruby / Sapphire)

A tiny command-line tool that edits Gen 3 Pokémon saves headlessly, using
[PKHeX.Core](https://github.com/kwsch/PKHeX) as the save engine — no GUI, no clicking.

It does two things:

1. **Fills every item pouch** — Items, Key Items, Poké Balls, TMs/HMs, Berries and
   PC Items — giving every valid item in every slot at a chosen quantity.
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

Examples:

```bash
dotnet run -- "Pokemon - Emerald Version (U).srm"        # 999 (default)
dotnet run -- "Pokemon - Emerald Version (U).srm" 65535  # HaX, see caveats
```

Back up your save first. The tool overwrites the file in place (it verifies by
reloading afterwards).

## Quantity — there is no infinity

Gen 3 stores an item quantity in a 2-byte field, but the games only render three
digits, so **999 is the largest value that displays and behaves correctly**. There is
no infinite-item flag in the save data — an item stack is a number, not a flag.

- `999` (default) — reads as 999 in the bag and works everywhere.
- `65535` — technically storable ("HaX" in PKHeX terms). The bag will show garbage or
  wrap, and some menus glitch. Useful only for experiments.
- True infinite use would require patching the game's code so the quantity never
  decrements — that is a ROM hack, not a save edit.

## Requirements

- .NET 10 SDK
- `PKHeX.Core` NuGet package (restored automatically)

## How it works

- `SaveUtil.GetSaveFile(path)` → `SAV3` object
- `sav.Inventory.Pouches` → `pouch.GiveAllItems(inv, qty)` for each pouch
- `sav.BlankPKM` → species/level/OT/IDs → `sav.SetPartySlotAtIndex(pk, i)`
- party count via the internal setter (reflection) — `SAV3.PartyCount` has no public one
- `sav.Write()` writes the file back, checksums included

## `saves/`

Sample saves from testing this tool (Emerald and Modern Emerald), plus the matching
save states. They are personal save files published deliberately — if you fork this
repository, consider whether you want someone else's trainer data in your copy. They
are **not** required to use the tool.

## ROMs

No ROMs are included, and none will be. Pokémon ROMs are copyrighted and cannot be
redistributed here; supply your own legally obtained dump.

## Caveats

- Legendaries are *legal-flagged* by PKHeX (no real event origin). Offline games do
  not check, so they work fine.
- Gen 3 only (`SAV3` — Emerald / Ruby / Sapphire / FireRed / LeafGreen).
- Saves written by a ROM hack may not load in the vanilla game, and vice versa.

## License

GPL-3.0 — see [LICENSE](LICENSE), matching PKHeX.Core, which this project links
against. PKHeX is © Kurt (kwsch) and contributors.
