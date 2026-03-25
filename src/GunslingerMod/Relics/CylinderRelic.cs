using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Combat;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Relics;

public sealed class CylinderRelic : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        var relic = this;
        if (side != relic.Owner.Creature.Side)
            return;

        if (combatState.RoundNumber == 1)
        {
            var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, null);
            if (cylinder != null)
            {
                // User-intended behavior: start combat with an empty cylinder.
                cylinder.ResetChambers();
                await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, null);
            }
        }

        var penalty = Owner.Creature.GetPower<NeutralBlockPenaltyPower>();
        if (penalty == null || penalty.Amount <= 0)
            await PowerCmd.Apply<NeutralBlockPenaltyPower>(Owner.Creature, 1, Owner.Creature, null);

        var player = combatState.Players.FirstOrDefault(p => p.Creature == Owner.Creature);
        if (player != null)
        {
            var canonical = ModelDb.Card<GunslingerMod.Models.Cards.Reload>();
            var reload = combatState.CreateCard(canonical, player);
            reload.AddKeyword(CardKeyword.Exhaust);
            reload.AddKeyword(CardKeyword.Ethereal);
            await CardPileCmd.AddGeneratedCardToCombat(reload, PileType.Hand, addedByPlayer: true);
        }

        relic.Flash();
    }
}
