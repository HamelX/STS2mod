using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using GunslingerMod.Models.DynamicVars;
using GunslingerMod.Models.Powers;
using System.Linq;

namespace GunslingerMod.Models.Cards;

// 봉인 증폭
public sealed class SealAmplify() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    private const int SelectionTimeoutMs = 8000;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SealAmplifyAmountVar()
    ];

    protected override bool IsPlayable
    {
        get
        {
            var cylinder = Owner?.Creature?.GetPower<CylinderPower>();
            return cylinder != null && cylinder.CountSealLoaded() > 0;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cylinder = await PowerCmd.Apply<CylinderPower>(Owner.Creature, 1, Owner.Creature, this);
        if (cylinder == null)
            return;

        var inc = (byte)(IsUpgraded ? 4 : 3);

        if (IsUpgraded)
        {
            // Upgrade: all loaded seal bullets gain +4
            cylinder.IncrementSealLevels(inc);
        }
        else
        {
            // Base: choose a seal bullet to amplify (if multiple exist).
            var sealIndices = new List<int>();
            for (var i = 0; i < CylinderPower.MaxRounds; i++)
            {
                if (cylinder.GetAmmoType(i) == CylinderPower.AmmoType.Seal)
                    sealIndices.Add(i);
            }

            if (sealIndices.Count == 1)
            {
                cylinder.IncrementSealLevel(sealIndices[0], inc);
            }
            else if (sealIndices.Count > 1)
            {
                var fallbackIndex = sealIndices[0];
                var chosen = await WaitForSealSelection(sealIndices, fallbackIndex);
                if (chosen >= 0)
                    cylinder.IncrementSealLevel(chosen, inc);
            }
        }
    }

    private static async Task<int> WaitForSealSelection(List<int> sealIndices, int fallbackIndex)
    {
        var validIndices = sealIndices.ToHashSet();
        var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        void Handler(int idx)
        {
            if (!validIndices.Contains(idx))
                return;
            tcs.TrySetResult(idx);
        }

        GunslingerMod.Nodes.NGunslingerCylinderUi.ChamberClicked += Handler;
        try
        {
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(SelectionTimeoutMs));
            return completed == tcs.Task ? await tcs.Task : fallbackIndex;
        }
        finally
        {
            GunslingerMod.Nodes.NGunslingerCylinderUi.ChamberClicked -= Handler;
        }
    }
}
