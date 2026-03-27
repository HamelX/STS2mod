using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

// ?뺣? ?ш꺽(暎얍칳弱꾣뭴): 鍮꾧린蹂?怨듦꺽 移대뱶 (?곸젏/蹂댁긽 ? ?덉젙?붿슜)
public sealed class PrecisionShot() : CardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || !BulletResolver.HasAliveOpponents(Owner.Creature))
            return;

        var target = BulletResolver.ResolveAliveTarget(Owner.Creature, cardPlay.Target);
        if (target == null)
            return;

        var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
            return;

        // Precision baseline: +2 Imprint. Upgrade sharpens setup tempo (+1 additional Imprint).
        await PowerCmd.Apply<ImprintPower>(Owner.Creature, IsUpgraded ? 3 : 2, Owner.Creature, this);

        var baseDamage = BulletResolver.GetBaseDamage(ammoType, sealLevel) + 3m;
        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, baseDamage);
    }
}
