using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

// 遊됱씤 媛쒕갑(弱곩뜲鰲ｆ붂): 媛???믪? ?덈꺼 遊됱씤?꾩쓣 諛쒖궗 ?쎌떎濡??대룞
public sealed class SealOpen() : CardModel(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    private const int MaxAmmo = 6;

    protected override void OnUpgrade()
    {
        // Behavior change handled in OnPlay.
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var bestIdx = -1;
        var bestLvl = -1;
        for (var i = 0; i < CylinderPower.MaxRounds; i++)
        {
            if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal)
            {
                var lvl = cylinder.GetSealLevel(i);
                if (lvl > bestLvl)
                {
                    bestLvl = lvl;
                    bestIdx = i;
                }
            }
        }

        if (bestIdx < 0)
            return;

        cylinder.SwapChambers(bestIdx, cylinder.ChamberIndex);

        // Upgrade: when you "open" the seal, immediately empower the round you pulled to the top.
        if (IsUpgraded && cylinder.GetAmmoType(cylinder.ChamberIndex) == CylinderPower.AmmoType.Seal)
            cylinder.IncrementSealLevel(cylinder.ChamberIndex, 2);

        var count = cylinder.CountLoaded();
        if (count > MaxAmmo)
            count = MaxAmmo;
        await PowerCmd.SetAmount<CylinderPower>(Owner.Creature, count, Owner.Creature, this);
    }
}
