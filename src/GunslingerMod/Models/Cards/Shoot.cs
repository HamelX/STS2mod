using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
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

        if (!BulletResolver.HasAliveOpponents(Owner.Creature))
            return;

        var shotTarget = BulletResolver.ResolveAliveTarget(Owner.Creature, preferredTarget);
        if (shotTarget == null)
            return;

        // Shoot is a "trigger pull": even if the current chamber is empty, the cylinder should still rotate.
        var didFire = BulletResolver.TryConsumeCurrentWithSealSkip(cylinder, this, out var ammoType, out var sealLevel);
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, this);

        if (!didFire)
        {
            // Dry-fire smoothing: pulling the trigger on an empty chamber refunds tempo via draw.
            // If No Draw is active, clear it so this compensation draw can always happen.
            var noDraw = Owner.Creature.GetPower<NoDrawPower>();
            if (noDraw != null)
                await PowerCmd.Remove<NoDrawPower>(Owner.Creature);

            await CardPileCmd.Draw(choiceContext, 1, Owner);

            return;
        }

        var baseDamage = Math.Max(0m, BulletResolver.GetBaseDamage(ammoType, sealLevel));
        if (IsUpgraded)
            baseDamage += 3m;

        await BulletResolver.FireAtTarget(choiceContext, Owner.Creature, shotTarget, this, ammoType, sealLevel, baseDamage);

        if ((Owner.Creature.GetPower<RicochetPower>()?.Amount ?? 0) > 0 || (Owner.Creature.GetPower<RicochetImprintPower>()?.Amount ?? 0) > 0)
            await PowerCmd.Apply<ImprintPower>(Owner.Creature, 1, Owner.Creature, this);
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
