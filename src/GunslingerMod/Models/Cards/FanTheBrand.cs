using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class FanTheBrand() : CardModel(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RicochetPower>(Owner.Creature, IsUpgraded ? 4 : 3, Owner.Creature, this);

        var ricochetTargets = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Count(c => c.IsAlive && c.CurrentHp > 0) ?? 0;
        if (ricochetTargets > 0)
            await PowerCmd.Apply<ImprintPower>(Owner.Creature, ricochetTargets, Owner.Creature, this);
    }
}
