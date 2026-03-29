using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.DynamicVars;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

// 遊됱씤??弱곩뜲凉? 2諛??μ쟾
public sealed class SealLoad() : CardModel(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    private const int MaxAmmo = 6;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SealLoadProtectionAmountVar()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        // If seal slots are already full, convert this into seal level growth instead.
        var loadedNewSeal = false;
        if (cylinder.CountSealLoaded() >= CylinderPower.MaxSealRounds)
        {
            cylinder.IncrementSealLevels(1);
        }
        else
        {
            // Load 1 seal bullet (global seal cap remains 2)
            for (var i = 0; i < 1; i++)
            {
                loadedNewSeal |= cylinder.TryLoadNext(CylinderPower.AmmoType.Seal);
            }
        }

        if (loadedNewSeal)
            await SealShotHelper.GrantTemporaryToHand(this);

        // Immediate survivability while setting up seal play.
        await CreatureCmd.GainBlock(Owner.Creature, IsUpgraded ? 8m : 5m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move, cardPlay);

        var rite = Owner.Creature.GetPower<SealRitePower>();
        if (rite != null)
            await TryAutoReleaseAtThreshold(choiceContext, cylinder);


        var count = cylinder.CountLoaded();
        if (count > MaxAmmo)
            count = MaxAmmo;
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, count, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Behavior change handled in OnPlay.
    }

    private async Task TryAutoReleaseAtThreshold(PlayerChoiceContext choiceContext, CylinderPower cylinder)
    {
        var sealIndex = SealShotHelper.FindHighestLevelSealIndex(cylinder);
        if (sealIndex < 0)
            return;

        var level = cylinder.GetSealLevel(sealIndex);
        if (level < 3)
            return;

        if (!BulletResolver.HasAliveOpponents(Owner.Creature))
            return;

        if (sealIndex != cylinder.ChamberIndex)
            cylinder.SwapChambers(sealIndex, cylinder.ChamberIndex);

        var target = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).FirstOrDefault(c => c.IsAlive && c.CurrentHp > 0);
        if (target == null)
            return;

        var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);
        if (!didFire)
            return;

        var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, target, this, ammoType, sealLevel, damage);
    }

}
