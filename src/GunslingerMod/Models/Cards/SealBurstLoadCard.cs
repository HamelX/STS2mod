using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealBurstLoad() : CardModel(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        var loads = IsUpgraded ? 2 : 1;
        var loadedNewSeal = false;
        for (var i = 0; i < loads; i++)
        {
            var before = cylinder.CountSealLoaded();
            cylinder.TryLoadOrIncrementSeal();
            loadedNewSeal |= cylinder.CountSealLoaded() > before;
        }

        if (loadedNewSeal)
            await SealShotHelper.GrantTemporaryToHand(this);

        for (var i = 0; i < loads; i++)
            cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
    }
}
