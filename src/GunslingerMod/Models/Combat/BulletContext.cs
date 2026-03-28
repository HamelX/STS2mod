using System.Threading;
using GunslingerMod.Models.Cards;
using GunslingerMod.Models.Powers;
using MegaCrit.Sts2.Core.Models;

namespace GunslingerMod.Models.Combat;

/// <summary>
/// A thread-safe, async-safe context to flag when a bullet is currently dealing damage.
/// This allows global hooks (like Ricochet) to perfectly identify bullet damage 
/// without modifying the original card/relic source, which would break game UI.
/// </summary>
public static class BulletContext
{
    private static readonly AsyncLocal<BulletInfo?> _currentBullet = new();

    public static BulletInfo? Current
    {
        get => _currentBullet.Value;
        set => _currentBullet.Value = value;
    }

    public static bool IsFiring => _currentBullet.Value != null;

    /// <summary>
    /// Fallback bullet-source check for hook contexts where async-local bullet context may be lost.
    /// Keep this narrow so non-bullet attacks do not trigger bullet-only reactions.
    /// </summary>
    public static bool IsBulletCardSource(CardModel? cardSource)
    {
        return cardSource is Shoot
            or QuickRack
            or Panning
            or SprayFire
            or ChainBurst
            or WalkingFire
            or BlankFire
            or EtchedTracer
            or ExecutionShot
            or PrecisionShot
            or RicochetShot
            or RicochetFollowUp
            or TracerStrike
            or TagBurst
            or HuntTrigger
            or BankShot
            or CrossfireRhythm
            or FinalVolley
            or SealReleaseKai;
    }

    public class BulletInfo
    {
        public CylinderPower.AmmoType AmmoType { get; }
        public byte SealLevel { get; }
        public bool SuppressTracerTriggers { get; }
        public bool RicochetTriggered { get; set; }

        public BulletInfo(CylinderPower.AmmoType ammoType, byte sealLevel = 0, bool suppressTracerTriggers = false)
        {
            AmmoType = ammoType;
            SealLevel = sealLevel;
            SuppressTracerTriggers = suppressTracerTriggers;
            RicochetTriggered = false;
        }
    }
}
