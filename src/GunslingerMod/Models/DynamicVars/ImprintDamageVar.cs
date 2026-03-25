using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.DynamicVars;

public sealed class ImprintDamageVar : DynamicVar
{
    public ImprintDamageVar() : base("ImprintDamage", 0m)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var imprint = card.Owner?.Creature?.GetPower<ImprintPower>();
        var hits = imprint?.Amount ?? 0;
        var damagePerHit = card.IsUpgraded ? 2 : 1;
        BaseValue = hits * damagePerHit;
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}
