using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;
using GunslingerMod.Models.DynamicVars;

namespace GunslingerMod.Models.Cards;

public sealed class Panning() : CardModel(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PanningTotalDamageVar()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var preferredTarget = cardPlay.Target;
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || cylinder.Amount <= 0)
            return;

        for (var i = 0; i < 6; i++)
        {
            if (!HasAliveOpponents())
                break;

            var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
            await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

            if (!didFire)
                continue;

            var shotTarget = ResolveAliveTarget(preferredTarget);
            if (shotTarget == null)
                break;

            var shotDamage = BulletResolver.GetBaseDamage(ammoType, sealLevel);

            // Per-shot resolution contract:
            // fire once -> apply damage -> re-check combat end before continuing.
            await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, shotTarget, this, ammoType, sealLevel, shotDamage);

            if (!HasAliveOpponents())
                break;
        }

        bool HasAliveOpponents()
            => BulletResolver.HasAliveOpponents(Owner.Creature);

        MegaCrit.Sts2.Core.Entities.Creatures.Creature? ResolveAliveTarget(MegaCrit.Sts2.Core.Entities.Creatures.Creature? preferred)
        {
            return BulletResolver.ResolveAliveTarget(Owner.Creature, preferred);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
