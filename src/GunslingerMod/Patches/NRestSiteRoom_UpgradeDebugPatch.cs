using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using GunslingerMod.Models.Characters;

namespace GunslingerMod.Patches;

// Temporary diagnostic patch: when entering a rest site, log which cards are considered upgradable.
[HarmonyPatch(typeof(NRestSiteRoom), "_Ready")]
public static class NRestSiteRoom_UpgradeDebugPatch
{
    public static void Postfix(NRestSiteRoom __instance)
    {
        try
        {
            var runState = Traverse.Create(__instance).Field("_runState").GetValue<IRunState>();
            if (runState == null)
                return;

            var me = runState.Players.FirstOrDefault(LocalContext.IsMe);
            if (me == null)
                return;

            if (me.Character is not Gunslinger)
                return;

            var deckPile = PileType.Deck.GetPile(me);
            var total = deckPile.Cards.Count;
            var upgradable = deckPile.Cards.Count(c => c.IsUpgradable);
            GD.Print($"[Gunslinger] Rest site: deck cards={total}, upgradable={upgradable}");

            foreach (var c in deckPile.Cards.Take(20))
            {
                GD.Print($"[Gunslinger]  - {c.Id.Entry} upgraded={c.IsUpgraded} lvl={c.CurrentUpgradeLevel}/{c.MaxUpgradeLevel} upgradable={c.IsUpgradable}");
            }
        }
        catch (System.Exception e)
        {
            GD.Print($"[Gunslinger] Rest site upgrade debug patch error: {e}");
        }
    }
}
