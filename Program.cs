// Gen3 Save Toolkit - headless Pokemon Gen 3 save editor
// Copyright (C) 2026 chupacabras83
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using PKHeX.Core;

var path = args[0];
// quantity: default 999 (what the games display cleanly). Pass 65535 for HaX.
int qty = 999;
if (args.Length > 1 && int.TryParse(args[1], out var parsed))
    qty = Math.Clamp(parsed, 1, 65535);

var sav = (SAV3)SaveUtil.GetSaveFile(path)!;
Console.WriteLine($"loaded {sav.GetType().Name} OT={sav.OT} party={sav.PartyCount}");

var inv = sav.Inventory;
foreach (var pouch in inv.Pouches)
    pouch.GiveAllItems(inv, qty);
Console.WriteLine($"items filled at x{qty}");

// six level-100 legendaries: Mewtwo, Rayquaza, Groudon, Kyogre, Latios, Deoxys
ushort[] legendaries = { 150, 384, 383, 382, 381, 386 };
for (int i = 0; i < legendaries.Length; i++)
{
    var pk = sav.BlankPKM;
    pk.Species = legendaries[i];
    pk.CurrentLevel = 100;
    pk.OriginalTrainerName = sav.OT;
    pk.TID16 = sav.TID16;
    pk.SID16 = sav.SID16;
    pk.Language = sav.Language;
    pk.Version = sav.Version;
    pk.Ball = 4;
    pk.Nickname = SpeciesName.GetSpeciesNameGeneration(legendaries[i], (int)sav.Language, 3);
    pk.Heal();
    pk.RefreshChecksum();
    sav.SetPartySlotAtIndex(pk, i);
    Console.WriteLine($"  + {pk.Nickname} Lv{pk.CurrentLevel}");
}

// SAV3.PartyCount has no public setter
var prop = typeof(SAV3).GetProperty("PartyCount")!;
prop.GetSetMethod(true)?.Invoke(sav, new object[] { legendaries.Length });

File.WriteAllBytes(path, sav.Write().ToArray());
Console.WriteLine("written");

var check = (SAV3)SaveUtil.GetSaveFile(path)!;
Console.WriteLine($"verify: party={check.PartyCount} -> " + string.Join(", ",
    check.PartyData.Select(p => $"{p.Nickname} L{p.CurrentLevel}")));
Console.WriteLine($"verify: Items pouch count = {check.Inventory.GetPouch(InventoryType.Items).Count}");
