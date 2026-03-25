using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using GunslingerMod.Models.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

public sealed class Shoot() : CardModel(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CurrentChamberDamageVar()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var preferredTarget = cardPlay.Target;

        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        if (Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Any(c => c.IsAlive) != true)
            return;

        var shotTarget = preferredTarget.IsAlive
            ? preferredTarget
            : Owner.Creature.CombatState?.HittableEnemies.FirstOrDefault(e => e.IsAlive);
        if (shotTarget == null)
            return;

        // Shoot is a "trigger pull": even if the current chamber is empty, the cylinder should still rotate.
        var didFire = cylinder.TryConsumeCurrent(out var ammoType, out var sealLevel);

        cylinder.AdvanceChamber();
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
        {
            // Dry-fire smoothing: pulling the trigger on an empty chamber refunds tempo via draw.
            await CardPileCmd.Draw(choiceContext, 1, Owner);
            return;
        }

        await PowerCmd.Apply<ImprintPower>(Owner.Creature, 1, Owner.Creature, this);

        var baseDamage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        if (IsUpgraded)
            baseDamage += 3m;

        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, shotTarget, this, ammoType, sealLevel, baseDamage);
    }

    private static decimal GetCurrentChamberDamage(CylinderPower.AmmoType ammoType, byte sealLevel, bool isUpgraded)
    {
        var damage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        if (isUpgraded)
            damage += 3m;
        return damage;
    }

    private sealed class CurrentChamberDamageVar : DamageVar
    {
        public CurrentChamberDamageVar() : base(0m, ValueProp.Move) { }

        public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, MegaCrit.Sts2.Core.Entities.Creatures.Creature? target, bool runGlobalHooks)
        {
            var cylinder = card.Owner?.Creature?.GetPower<CylinderPower>();
            if (cylinder == null)
            {
                BaseValue = 0m;
                PreviewValue = 0m;
                return;
            }

            decimal total = 0m;
            var idx = cylinder.ChamberIndex;

            if (cylinder.IsLoaded(idx))
            {
                var ammoType = cylinder.GetAmmoType(idx);
                var sealLevel = ammoType == CylinderPower.AmmoType.Seal ? cylinder.GetSealLevel(idx) : (byte)0;
                total += GetCurrentChamberDamage(ammoType, sealLevel, card.IsUpgraded);
            }

            BaseValue = total;
            base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
        }
    }
}
