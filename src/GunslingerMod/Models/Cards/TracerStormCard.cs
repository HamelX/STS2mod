using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class TracerStorm() : CardModel(2, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        var loads = IsUpgraded ? 5 : 3;
        var loaded = 0;
        for (var i = 0; i < loads; i++)
        {
            if (cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer))
                loaded++;
        }

        if (loaded > 0)
            await BulletResolver.RegisterTracerShots(choiceContext, Owner.Creature, this, loaded);

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
    }
}
