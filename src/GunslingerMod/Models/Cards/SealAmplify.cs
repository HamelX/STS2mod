using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.DynamicVars;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

// 봉인 증폭
public sealed class SealAmplify() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SealAmplifyAmountVar()
    ];

    protected override bool IsPlayable
    {
        get
        {
            var cylinder = Owner?.Creature?.GetPower<CylinderPower>();
            return cylinder != null && cylinder.CountSealLoaded() > 0;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        var increase = (byte)(IsUpgraded ? 5 : 3);
        cylinder.IncrementSealLevels(increase);
    }
}
