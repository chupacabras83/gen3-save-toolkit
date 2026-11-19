using PKHeX.Core;
using System.Reflection;

var path = args[0];
var sav = (SAV3E)SaveUtil.GetSaveFile(path)!;
Console.WriteLine($"loaded {sav.GetType().Name} OT={sav.OT} party={sav.PartyCount}");

var inv = sav.Inventory;
int qty = 999;
foreach (var pouch in inv.Pouches)
    pouch.GiveAllItems(inv, qty);
Console.WriteLine($"items filled at x{qty}");

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

var prop = typeof(SAV3).GetProperty("PartyCount")!;
var setter = prop.GetSetMethod(true);
if (setter != null) { setter.Invoke(sav, new object[] { 6 }); Console.WriteLine("party count set to 6"); }
else Console.WriteLine("no PartyCount setter found");

File.WriteAllBytes(path, sav.Write().ToArray());
Console.WriteLine("written");

var check = (SAV3E)SaveUtil.GetSaveFile(path)!;
Console.WriteLine($"verify: party={check.PartyCount} -> " + string.Join(", ",
    check.PartyData.Select(p => $"{p.Nickname} L{p.CurrentLevel}")));
Console.WriteLine($"verify: Items pouch count = {check.Inventory.GetPouch(InventoryType.Items).Count}");
