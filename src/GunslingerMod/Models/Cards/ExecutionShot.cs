using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class ExecutionShot() : CardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var didFire = cylinder.TryConsumeCurrent(out var ammoType, out var sealLevel);
        cylinder.AdvanceChamber();
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, 3, Owner.Creature, this);

        var baseDamage = BulletResolver.GetBaseDamage(ammoType, sealLevel);

        if (cardPlay.Target.CurrentHp * 2 <= cardPlay.Target.MaxHp)
            baseDamage *= IsUpgraded ? 3m : 2m;

        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, cardPlay.Target, this, ammoType, sealLevel, baseDamage);
    }
}
