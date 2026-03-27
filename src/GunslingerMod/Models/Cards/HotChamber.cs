using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class HotChamber() : CardModel(1, CardType.Skill, CardRarity.Common, TargetType.None), IImprintConsumerCard
{
    protected override bool IsPlayable
    {
        get
        {
            if (IsUpgraded)
                return true;
            return (Owner?.Creature?.GetPower<ImprintPower>()?.Amount ?? 0) >= 1;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        if (!IsUpgraded)
        {
            if ((Owner.Creature.GetPower<ImprintPower>()?.Amount ?? 0) < 1)
                return;

            await PowerCmd.Apply<ImprintPower>(Owner.Creature, -1, Owner.Creature, this);
        }

        for (var i = 0; i < 2; i++)
            TryLoadTracerWithFallback(cylinder);

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if ((Owner.Creature.GetPower<TracerFiredThisTurnPower>()?.Amount ?? 0) > 0)
            await CardPileCmd.Draw(choiceContext, 2, Owner);
        else
            await CardPileCmd.Draw(choiceContext, 1, Owner);
    }

    private static bool TryLoadTracerWithFallback(CylinderPower cylinder)
    {
        if (cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer))
            return true;

        // Fallback should be narrow: only retune the current firing chamber when no empty slot exists.
        var idx = cylinder.ChamberIndex;
        var ammo = cylinder.GetAmmoType(idx);
        if (ammo is CylinderPower.AmmoType.Normal or CylinderPower.AmmoType.Enhanced or CylinderPower.AmmoType.Penetrator)
        {
            cylinder.ClearChamberAt(idx);
            return cylinder.TryLoadInto(idx, CylinderPower.AmmoType.Tracer);
        }

        return false;
    }
}
