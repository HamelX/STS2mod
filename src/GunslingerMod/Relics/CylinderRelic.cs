using MegaCrit.Sts2.Core.Commands;
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
        if (side != Owner.Creature.Side)
            return;

        var cylinder = Owner.Creature.GetPower<CylinderPower>()
                       ?? await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, null);
        if (cylinder == null)
            return;

        var loadCount = combatState.RoundNumber == 1 ? 2 : (cylinder.CountLoaded() == 0 ? 1 : 0);
        if (loadCount <= 0)
            return;

        var loadedAny = false;
        for (var i = 0; i < loadCount; i++)
            loadedAny |= cylinder.TryLoadNext(CylinderPower.AmmoType.Normal);

        if (!loadedAny)
            return;

        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, cylinder.CountLoaded(), Owner.Creature, null);

        Flash();
    }
}
