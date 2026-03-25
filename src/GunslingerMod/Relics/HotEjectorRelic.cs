using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Relics;

public sealed class HotEjectorRelic : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public static bool CanTriggerFor(Creature shooter)
    {
        var player = shooter.Player;
        if (player == null)
            return false;

        if (player.GetRelic<HotEjectorRelic>() == null)
            return false;

        return shooter.GetPower<FirstFireEnergyThisTurnPower>() == null;
    }

    public static async Task TriggerFor(Player player)
    {
        if (player.GetRelic<HotEjectorRelic>() is not { } relic)
            return;

        await MegaCrit.Sts2.Core.Commands.PlayerCmd.GainEnergy(1, player);
        await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<FirstFireEnergyThisTurnPower>(player.Creature, 1, player.Creature, null);
        relic.Flash();
    }
}
