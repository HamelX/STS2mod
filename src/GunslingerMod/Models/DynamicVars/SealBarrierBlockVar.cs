using System;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.DynamicVars;

public sealed class SealBarrierBlockVar : BlockVar
{
    public SealBarrierBlockVar() : base(5m, ValueProp.Move)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var cylinder = card.Owner?.Creature?.GetPower<CylinderPower>();
        var bestLevel = 0;

        if (cylinder != null && cylinder.CountSealLoaded() > 0)
        {
            for (var i = 0; i < CylinderPower.MaxRounds; i++)
            {
                if (cylinder.GetAmmoType(i) != CylinderPower.AmmoType.Seal)
                    continue;
                bestLevel = Math.Max(bestLevel, cylinder.GetSealLevel(i));
            }
        }

        BaseValue = 5m + (bestLevel * 2m);
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}
