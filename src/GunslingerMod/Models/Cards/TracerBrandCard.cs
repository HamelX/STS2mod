using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class TracerBrand() : CardModel(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        cylinder.TryLoadNext(CylinderPower.AmmoType.Tracer);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        var ricochetTargets = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Count(c => c.IsAlive && c.CurrentHp > 0) ?? 0;
        if (ricochetTargets > 0)
            await PowerCmd.Apply<ImprintPower>(Owner.Creature, ricochetTargets * (IsUpgraded ? 2 : 1), Owner.Creature, this);
    }
}
