using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Cards;

namespace GunslingerMod.Models.Powers;

public sealed class ReloadLockPower : PowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner?.Creature != Owner)
            return true;

        return card is not Reload;
    }

    public override Task AfterSideTurnStart(CombatSide side, MegaCrit.Sts2.Core.Combat.CombatState combatState)
    {
        if (side == Owner.Side)
            return MegaCrit.Sts2.Core.Commands.PowerCmd.SetAmount<ReloadLockPower>(Owner, 0, Owner, null);

        return Task.CompletedTask;
    }
}
