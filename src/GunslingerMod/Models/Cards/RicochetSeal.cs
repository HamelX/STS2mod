using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class RicochetSeal() : CardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
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

        var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, cardPlay.Target, this, ammoType, sealLevel, damage);
        await PowerCmd.Apply<ImprintPower>(Owner.Creature, 1, Owner.Creature, this);

        if (ammoType == CylinderPower.AmmoType.Seal)
            await PowerCmd.Apply<RicochetPower>(Owner.Creature, IsUpgraded ? 2 : 1, Owner.Creature, this);
    }
}