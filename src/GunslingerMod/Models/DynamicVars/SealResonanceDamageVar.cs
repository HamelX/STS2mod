using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.DynamicVars;

public sealed class SealResonanceDamageVar : DamageVar
{
    public SealResonanceDamageVar() : base(0m, ValueProp.Move)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var cylinder = card.Owner?.Creature?.GetPower<CylinderPower>();
        var bestLvl = 0;
        if (cylinder != null)
        {
            for (var i = 0; i < CylinderPower.MaxRounds; i++)
            {
                if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal)
                {
                    var lvl = cylinder.GetSealLevel(i);
                    if (lvl > bestLvl)
                        bestLvl = lvl;
                }
            }
        }
        BaseValue = bestLvl * 2m;
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}
