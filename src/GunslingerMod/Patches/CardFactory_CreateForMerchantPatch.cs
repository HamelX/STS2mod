using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using GunslingerMod.Models.Characters;

namespace GunslingerMod.Patches;

/// <summary>
/// Prevent merchant black-screen crash for Gunslinger when shop rarity roll has no cards for that type.
/// Falls back to the highest available rarity for the requested CardType.
/// </summary>
[HarmonyPatch(typeof(CardFactory), nameof(CardFactory.CreateForMerchant),
    new Type[] { typeof(Player), typeof(IEnumerable<CardModel>), typeof(CardType) })]
public static class CardFactory_CreateForMerchantPatch
{
    public static bool Prefix(Player player, IEnumerable<CardModel> options, CardType type, ref CardCreationResult __result)
    {
        if (player?.Character is not Gunslinger)
            return true;

        // Mirror core filters (basic exclusion + mod hook). We intentionally avoid private FilterForPlayerCount.
        options = Hook.ModifyMerchantCardPool(player.RunState, player, options);
        var source = options.Where(c => c.Rarity != CardRarity.Basic).ToArray();

        // Rarity roll (shop odds), then clamp to highest available rarity for this card type.
        var rolled = Hook.ModifyMerchantCardRarity(player.RunState, player,
            player.PlayerOdds.CardRarity.RollWithoutChangingFutureOdds(CardRarityOddsType.Shop));

        List<CardModel> list = source.Where(c => c.Rarity == rolled && c.Type == type).ToList();
        if (list.Count == 0)
        {
            // Choose the highest available rarity for this type (Rare -> Uncommon -> Common).
            var rarityOrder = new[] { CardRarity.Rare, CardRarity.Uncommon, CardRarity.Common };
            var fallbackAll = source.Where(c => c.Type == type).ToList();
            if (fallbackAll.Count == 0)
                throw new InvalidOperationException("Gunslinger merchant pool has no cards for type: " + type);

            foreach (var r in rarityOrder)
            {
                list = fallbackAll.Where(c => c.Rarity == r).ToList();
                if (list.Count > 0)
                    break;
            }
        }

        var chosen = player.PlayerRng.Shops.NextItem(list);
        if (chosen == null)
            throw new InvalidOperationException("Gunslinger merchant pool returned no card for type: " + type);
        var cardModel = player.RunState.CreateCard(chosen, player);
        __result = new CardCreationResult(cardModel);
        return false; // Skip original
    }
}
