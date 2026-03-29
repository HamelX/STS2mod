using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Powers;

public sealed class CylinderPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldReceiveCombatHooks => true;

    // Ammo by type (at most one type per chamber)
    public int NormalMask { get; private set; }
    public int EnhancedMask { get; private set; }
    public int PenetratorMask { get; private set; }
    public int TracerMask { get; private set; }
    public int SealMask { get; private set; }

    // Seal levels: 1 byte per chamber (0..255) packed into a 64-bit value.
    public ulong SealLevelsPacked { get; private set; }

    // Temporary seal bullets loaded beyond the normal cap (removed at end of turn)
    public int TempSealMask { get; private set; }

    // Convenience: any loaded chamber
    public int LoadedMask => NormalMask | EnhancedMask | PenetratorMask | TracerMask | SealMask | TempSealMask;

    public int ChamberIndex { get; private set; }

    public enum AmmoType
    {
        None = 0,
        Normal = 1,
        Enhanced = 2,
        Penetrator = 3,
        Tracer = 4,
        Seal = 5,
    }

    public const int MaxRounds = 6;
    public const int MaxSealRounds = 1;

    public const int SealThresholdExtraHit = 255;
    public const int SealThresholdUnblockable = 7;
    public const int SealThresholdOverpenetration = 255;

    public void ResetChambers()
    {
        NormalMask = 0;
        EnhancedMask = 0;
        PenetratorMask = 0;
        TracerMask = 0;
        SealMask = 0;
        SealLevelsPacked = 0;
        TempSealMask = 0;
        ChamberIndex = 0;
    }

    public int CountLoaded()
    {
        var count = 0;
        var mask = LoadedMask;
        while (mask != 0)
        {
            count += mask & 1;
            mask >>= 1;
        }
        return count;
    }

    public int CountEmpty() => MaxRounds - CountLoaded();

    public void ClearAll()
    {
        for (var i = 0; i < MaxRounds; i++)
            ClearChamber(i);
    }

    public int CountSealLoaded() => CountBits(SealMask | TempSealMask);

    public bool IsTempSeal(int chamberIndex)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return false;
        return (TempSealMask & (1 << chamberIndex)) != 0;
    }

    private static int CountBits(int mask)
    {
        var count = 0;
        while (mask != 0)
        {
            count += mask & 1;
            mask >>= 1;
        }
        return count;
    }

    public bool IsLoaded(int chamberIndex)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return false;
        return (LoadedMask & (1 << chamberIndex)) != 0;
    }

    public AmmoType GetAmmoType(int chamberIndex)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return AmmoType.None;
        var bit = 1 << chamberIndex;
        if ((SealMask & bit) != 0 || (TempSealMask & bit) != 0) return AmmoType.Seal;
        if ((PenetratorMask & bit) != 0) return AmmoType.Penetrator;
        if ((TracerMask & bit) != 0) return AmmoType.Tracer;
        if ((EnhancedMask & bit) != 0) return AmmoType.Enhanced;
        if ((NormalMask & bit) != 0) return AmmoType.Normal;
        return AmmoType.None;
    }

    public byte GetSealLevel(int chamberIndex)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return 0;
        var shift = chamberIndex * 8;
        return (byte)((SealLevelsPacked >> shift) & 0xFF);
    }

    private void SetSealLevel(int chamberIndex, byte level)
    {
        var shift = chamberIndex * 8;
        var mask = 0xFFUL << shift;
        SealLevelsPacked = (SealLevelsPacked & ~mask) | ((ulong)level << shift);
    }

    public void IncrementSealLevel(int chamberIndex, byte amount = 1)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return;
        if (GetAmmoType(chamberIndex) != AmmoType.Seal)
            return;
        var lvl = GetSealLevel(chamberIndex);
        var next = (int)lvl + amount;
        if (next > 255) next = 255;
        SetSealLevel(chamberIndex, (byte)next);
    }

    public void ReduceSealLevel(int chamberIndex, byte amount = 1)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return;
        if (GetAmmoType(chamberIndex) != AmmoType.Seal)
            return;

        var lvl = GetSealLevel(chamberIndex);
        var next = lvl - amount;
        if (next < 0)
            next = 0;
        SetSealLevel(chamberIndex, (byte)next);
    }

    private void ClearChamber(int chamberIndex)
    {
        var bit = 1 << chamberIndex;
        NormalMask &= ~bit;
        EnhancedMask &= ~bit;
        PenetratorMask &= ~bit;
        TracerMask &= ~bit;
        SealMask &= ~bit;
        TempSealMask &= ~bit;
        SetSealLevel(chamberIndex, 0);
    }

    public void ClearChamberAt(int chamberIndex)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return;

        ClearChamber(chamberIndex);
    }

    private void LoadDirect(int chamberIndex, AmmoType type, byte sealLevel = 0, bool isTempSeal = false)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds || type == AmmoType.None)
            return;

        ClearChamber(chamberIndex);

        var bit = 1 << chamberIndex;
        switch (type)
        {
            case AmmoType.Normal:
                NormalMask |= bit;
                break;
            case AmmoType.Enhanced:
                EnhancedMask |= bit;
                break;
            case AmmoType.Penetrator:
                PenetratorMask |= bit;
                break;
            case AmmoType.Tracer:
                TracerMask |= bit;
                break;
            case AmmoType.Seal:
                if (isTempSeal)
                    TempSealMask |= bit;
                else
                    SealMask |= bit;
                SetSealLevel(chamberIndex, sealLevel);
                break;
        }
    }

    public void FillAllWithSeal(byte level)
    {
        for (var i = 0; i < MaxRounds; i++)
            LoadDirect(i, AmmoType.Seal, level, isTempSeal: false);
    }

    public bool TryLoadNext(AmmoType type)
    {
        if (type == AmmoType.Seal && CountSealLoaded() >= MaxSealRounds)
            return false;

        for (var offset = 0; offset < MaxRounds; offset++)
        {
            var i = (ChamberIndex + offset) % MaxRounds;
            if (!IsLoaded(i))
                return TryLoadInto(i, type);
        }
        return false;
    }

    public bool TryLoadOrIncrementSeal(byte incrementAmount = 1)
    {
        if (TryLoadNext(AmmoType.Seal))
            return true;

        if (incrementAmount <= 0)
            return false;

        for (var i = 0; i < MaxRounds; i++)
        {
            if (GetAmmoType(i) != AmmoType.Seal)
                continue;

            IncrementSealLevel(i, incrementAmount);
            return true;
        }

        return false;
    }

    public bool TryLoadInto(int chamberIndex, AmmoType type)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return false;
        if (IsLoaded(chamberIndex))
            return false;
        if (type == AmmoType.Seal && CountSealLoaded() >= MaxSealRounds)
            return false;

        LoadDirect(chamberIndex, type);
        return true;
    }

    public bool TryLoadTempSeal()
    {
        for (var offset = 0; offset < MaxRounds; offset++)
        {
            var i = (ChamberIndex + offset) % MaxRounds;
            if (!IsLoaded(i))
            {
                LoadDirect(i, AmmoType.Seal, 0, isTempSeal: true);
                return true;
            }
        }
        return false;
    }

    public bool TryLoadTempSealInto(int chamberIndex, byte level = 0)
    {
        if (chamberIndex < 0 || chamberIndex >= MaxRounds)
            return false;
        if (IsLoaded(chamberIndex))
            return false;

        LoadDirect(chamberIndex, AmmoType.Seal, level, isTempSeal: true);
        return true;
    }

    public bool TryConsumeCurrent(out AmmoType type, out byte sealLevel)
    {
        type = AmmoType.None;
        sealLevel = 0;
        if (!IsLoaded(ChamberIndex))
            return false;

        type = GetAmmoType(ChamberIndex);
        if (type == AmmoType.Seal)
            sealLevel = GetSealLevel(ChamberIndex);


        ClearChamber(ChamberIndex);
        return true;
    }

    public void SwapChambers(int a, int b)
    {
        if (a < 0 || a >= MaxRounds || b < 0 || b >= MaxRounds || a == b)
            return;

        var typeA = GetAmmoType(a);
        var typeB = GetAmmoType(b);
        var sealA = GetSealLevel(a);
        var sealB = GetSealLevel(b);
        var tempSealA = typeA == AmmoType.Seal && IsTempSeal(a);
        var tempSealB = typeB == AmmoType.Seal && IsTempSeal(b);

        ClearChamber(a);
        ClearChamber(b);

        if (typeA != AmmoType.None)
            LoadDirect(b, typeA, sealA, tempSealA);
        if (typeB != AmmoType.None)
            LoadDirect(a, typeB, sealB, tempSealB);
    }

    public void IncrementSealLevels(byte amount = 1)
    {
        if (amount <= 0) return;
        for (var i = 0; i < MaxRounds; i++)
        {
            if (GetAmmoType(i) == AmmoType.Seal)
            {
                var lvl = GetSealLevel(i);
                var next = (int)lvl + amount;
                if (next > 255) next = 255;
                SetSealLevel(i, (byte)next);
            }
        }
    }

    public void AdvanceChamber(int maxRounds = MaxRounds)
    {
        if (maxRounds <= 0)
            return;

        ChamberIndex = (ChamberIndex + 1) % maxRounds;
    }

    public void RetreatChamber(int maxRounds = MaxRounds)
    {
        if (maxRounds <= 0)
            return;

        ChamberIndex = (ChamberIndex - 1 + maxRounds) % maxRounds;
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (Owner != null && Owner.Side == side)
        {
            Amount = CountLoaded();
        }
        return Task.CompletedTask;
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner != null && Owner.Side == side)
        {
            for (var i = 0; i < MaxRounds; i++)
            {
                if (IsTempSeal(i))
                    ClearChamber(i);
            }

            EnforceSealLimit();
            Amount = CountLoaded();
        }
        return Task.CompletedTask;
    }

    private void EnforceSealLimit()
    {
        var sealIndices = new List<int>();
        for (var i = 0; i < MaxRounds; i++)
        {
            if (GetAmmoType(i) == AmmoType.Seal)
                sealIndices.Add(i);
        }

        if (sealIndices.Count <= MaxSealRounds)
            return;

        sealIndices.Sort((a, b) =>
        {
            var la = GetSealLevel(a);
            var lb = GetSealLevel(b);
            var cmp = lb.CompareTo(la);
            if (cmp != 0) return cmp;
            return a.CompareTo(b);
        });

        for (var i = MaxSealRounds; i < sealIndices.Count; i++)
            ClearChamber(sealIndices[i]);
    }
}
