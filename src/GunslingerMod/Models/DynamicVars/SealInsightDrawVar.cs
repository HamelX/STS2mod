using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Cards;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.DynamicVars;

public sealed class SealInsightDrawVar : DynamicVar
{
    public SealInsightDrawVar() : base("Draw", 0m)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var cylinder = card.Owner?.Creature?.GetPower<CylinderPower>();
        BaseValue = cylinder == null
            ? (card.IsUpgraded ? 2 : 1)
            : SealInsight.GetDrawCount(cylinder, card.IsUpgraded);
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
    }
}
