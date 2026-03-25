using System;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.DynamicVars;

public sealed class SprayFireTotalDamageVar : DamageVar
{
    public SprayFireTotalDamageVar() : base(0m, ValueProp.Move)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var cylinder = card.Owner?.Creature?.GetPower<CylinderPower>();
        if (cylinder == null)
        {
            BaseValue = 0m;
            base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
            return;
        }

        decimal totalDamage = 0m;
        var multiplier = card.IsUpgraded ? 0.85m : 0.75m;

        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (!cylinder.IsLoaded(i))
                continue;

            var ammoType = cylinder.GetAmmoType(i);
            var sealLevel = cylinder.GetSealLevel(i);

            var shotDamage = BulletResolver.GetBaseDamage(ammoType, sealLevel);

            totalDamage += shotDamage;

            if (ammoType == CylinderPower.AmmoType.Seal && sealLevel >= CylinderPower.SealThresholdExtraHit)
                totalDamage += shotDamage;
        }

        BaseValue = totalDamage;
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}
