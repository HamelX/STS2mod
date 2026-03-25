using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.DynamicVars;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class SealResonance() : CardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SealResonanceDamageVar()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null || !BulletResolver.HasAliveOpponents(Owner.Creature))
            return;

        var target = BulletResolver.ResolveAliveTarget(Owner.Creature, cardPlay.Target);
        if (target == null)
            return;

        var bestLvl = 0;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal)
            {
                var lvl = cylinder.GetSealLevel(i);
                if (lvl > bestLvl)
                    bestLvl = lvl;
            }
        }

        var damage = (decimal)bestLvl * 2m;
        if (damage > 0)
            await CreatureCmd.Damage(choiceContext, target, damage, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
