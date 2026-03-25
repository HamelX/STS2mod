using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.DynamicVars;

public sealed class SealAmplifyAmountVar : DynamicVar
{
    public SealAmplifyAmountVar() : base("SealAmplifyAmount", 0m)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var value = card.IsUpgraded ? 4m : 3m;
        PreviewValue = value;
        EnchantedValue = value;
    }
}
