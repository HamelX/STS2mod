using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using GunslingerMod.Models.Characters;

namespace GunslingerMod.Patches;

/// <summary>
/// Ancient Presence can grant Large Capsule.
/// Core LargeCapsule expects a Basic+Strike and Basic+Defend card in the character pool.
/// Gunslinger's current starter/basic setup can omit Basic+Strike, causing First(...) to throw
/// and the event flow to soft-lock.
///
/// For Gunslinger only, mirror core behavior but resolve fallback cards safely.
/// </summary>
[HarmonyPatch(typeof(LargeCapsule), nameof(LargeCapsule.AfterObtained))]
public static class LargeCapsule_GunslingerCompatPatch
{
    [HarmonyPrefix]
    private static bool Prefix(LargeCapsule __instance, ref Task __result)
    {
        if (__instance.Owner?.Character is not Gunslinger)
            return true;

        __result = AfterObtainedSafe(__instance);
        return false;
    }

    private static async Task AfterObtainedSafe(LargeCapsule relic)
    {
        for (var i = 0; i < relic.DynamicVars["Relics"].IntValue; i++)
        {
            var next = RelicFactory.PullNextRelicFromFront(relic.Owner).ToMutable();
            await RelicCmd.Obtain(next, relic.Owner);
        }

        var strike = ResolveStrikeLike(relic.Owner.Character);
        var defend = ResolveDefendLike(relic.Owner.Character);

        var adds = new System.Collections.Generic.List<CardPileAddResult>(2);
        adds.Add(await CardPileCmd.Add(relic.Owner.RunState.CreateCard(strike, relic.Owner), PileType.Deck));
        adds.Add(await CardPileCmd.Add(relic.Owner.RunState.CreateCard(defend, relic.Owner), PileType.Deck));
        CardCmd.PreviewCardPileAdd(adds, 2f);
    }

    private static CardModel ResolveStrikeLike(CharacterModel character)
    {
        var pool = character.CardPool.AllCards;

        return pool.FirstOrDefault(c => c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Strike))
               ?? pool.FirstOrDefault(c => c.Rarity == CardRarity.Basic && c.Type == CardType.Attack)
               ?? character.StartingDeck.First(c => c.Type == CardType.Attack);
    }

    private static CardModel ResolveDefendLike(CharacterModel character)
    {
        var pool = character.CardPool.AllCards;

        return pool.FirstOrDefault(c => c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Defend))
               ?? pool.FirstOrDefault(c => c.Rarity == CardRarity.Basic && c.Type == CardType.Skill)
               ?? character.StartingDeck.First(c => c.Tags.Contains(CardTag.Defend) || c.Type == CardType.Skill);
    }
}
