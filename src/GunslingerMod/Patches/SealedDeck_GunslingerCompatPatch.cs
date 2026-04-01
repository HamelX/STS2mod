using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using GunslingerMod.Models.Characters;
using GunslingerMod.Framework.Compatibility;

namespace GunslingerMod.Patches;

/// <summary>
/// Compatibility patch for the core modifier MODIFIER.SEALED_DECK.
///
/// Core SealedDeck generates 30 distinct reward cards using CardFactory.CreateForReward.
/// CardFactory enforces a distinct blacklist, so a small mod card pool can exhaust before 30 cards.
/// When that happens, core throws and the selection UI never appears.
///
/// For Gunslinger, we clamp the generated candidate count to the number of possible cards,
/// and clamp the pick count as well.
/// </summary>
[HarmonyPatch]
[HarmonyPatchCategory(GunslingerPatchCategories.CompatibilityProduction)]
public static class SealedDeck_GunslingerCompatPatch
{
    [HarmonyTargetMethod]
    private static System.Reflection.MethodBase TargetMethod()
        => AccessTools.Method(typeof(SealedDeck), "ChooseCards", new[] { typeof(Player) });

    [HarmonyPrefix]
    private static bool Prefix(Player player, ref System.Threading.Tasks.Task __result)
    {
        // Only override for our character.
        if (player.Character is not Gunslinger)
            return true;

        __result = ChooseCardsSafe(player);
        return false;
    }

    private static async System.Threading.Tasks.Task ChooseCardsSafe(Player player)
    {
        // Mirror core options as closely as possible.
        var options = new CardCreationOptions(
                new[] { player.Character.CardPool },
                CardCreationSource.Other,
                CardRarityOddsType.RegularEncounter)
            .WithFlags(CardCreationFlags.NoUpgradeRoll | CardCreationFlags.ForceRarityOddsChange);

        // Clamp to prevent CardFactory exhaustion (it blacklists already-generated cards).
        var possible = options.GetPossibleCards(player).ToList();
        var candidateCount = System.Math.Min(30, possible.Count);
        var pickCount = System.Math.Min(10, candidateCount);

        Godot.GD.Print($"[Gunslinger] SEALED_DECK compat: possible={possible.Count}, candidates={candidateCount}, pick={pickCount}");

        var rewards = CardFactory.CreateForReward(player, candidateCount, options).ToList();
        rewards = rewards.OrderBy(r => r.Card.Rarity).ThenBy(r => r.Card.Title).ToList();

        var prefs = new CardSelectorPrefs(new LocString("modifiers", "SEALED_DECK.selectionPrompt"), pickCount)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };

        var chosen = (await CardSelectCmd.FromSimpleGridForRewards(
            new BlockingPlayerChoiceContext(),
            rewards,
            player,
            prefs)).ToList();

        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(chosen, PileType.Deck), 1.2f);

        // Keep core behavior.
        foreach (var p in player.RunState.Players)
            p.RelicGrabBag.Remove<PandorasBox>();
        player.RunState.SharedRelicGrabBag.Remove<PandorasBox>();
    }
}
