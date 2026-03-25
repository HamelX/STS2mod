using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Models.Cards;

// 봉인 확인
public sealed class SealCheck() : CardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = Owner.Creature.GetPower<CylinderPower>();
        if (cylinder == null)
            return;

        var foundSeal = cylinder.GetAmmoType(cylinder.ChamberIndex) == CylinderPower.AmmoType.Seal;
        if (!foundSeal)
        {
            cylinder.AdvanceChamber();
            foundSeal = cylinder.GetAmmoType(cylinder.ChamberIndex) == CylinderPower.AmmoType.Seal;
        }

        if (!foundSeal)
            return;

        await CardPileCmd.Draw(choiceContext, 1, Owner);

        // Upgrade reward: if Seal Check connects, sharpen that live seal immediately.
        if (IsUpgraded)
            cylinder.IncrementSealLevel(cylinder.ChamberIndex, 1);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
