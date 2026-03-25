using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Audio;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class Reload() : CardModel(0, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override void OnUpgrade()
    {
    }

    private const int MaxAmmo = 6;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        if (cylinder.CountLoaded() == 0)
            cylinder.ResetChambers();

        var ammoTypeToLoad = CylinderPower.AmmoType.Normal;

        // Load 2 rounds with a quick "insert, insert" pacing so it feels like placing bullets into chambers.
        for (var i = 0; i < 2; i++)
        {
            // TryLoadNext loads the next empty chamber in firing order.
            var beforeMask = cylinder.LoadedMask;
            var loaded = cylinder.TryLoadNext(ammoTypeToLoad);

            if (loaded)
            {
                // Find which chamber changed (best-effort) for future SFX hooks.
                var diff = beforeMask ^ cylinder.LoadedMask;
                var chamberIndex = diff != 0 ? System.Numerics.BitOperations.TrailingZeroCount(diff) : -1;
                if (chamberIndex >= 0 && chamberIndex < 6)
                    GunslingerSfxHooks.AmmoInserted(chamberIndex);
            }

            var count = cylinder.CountLoaded();
            if (count > MaxAmmo)
                count = MaxAmmo;
            await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, count, Owner.Creature, this);

            // Quick pacing between inserts (skip delay after the last one)
            if (i == 0)
                await Cmd.Wait(0.07f);
        }

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
}
