using System.Runtime.InteropServices;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using GunslingerMod.Models.CardPools;
using GunslingerMod.Models.Cards;
using GunslingerMod.Models.RelicPools;
using GunslingerMod.Relics;

namespace GunslingerMod.Models.Characters;

public sealed class Gunslinger : CharacterModel
{
    public const string energyColorName = "gunslinger";

    public override CharacterGender Gender => CharacterGender.Masculine;

    protected override CharacterModel? UnlocksAfterRunAs => null;

    public override Color NameColor => new("4D4D4D");

    public override int StartingHp => 77;

    public override int StartingGold => 99;

    public override CardPoolModel CardPool => ModelDb.CardPool<GunslingerCardPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<IroncladPotionPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GunslingerRelicPool>();

    public override IEnumerable<CardModel> StartingDeck =>
    [
        // 10-card starter deck:
        // Shoot x4, Defend x4, Reload x2.
        ModelDb.Card<Shoot>(),
        ModelDb.Card<Shoot>(),
        ModelDb.Card<Shoot>(),
        ModelDb.Card<Shoot>(),
        ModelDb.Card<DefendGunslinger>(),
        ModelDb.Card<DefendGunslinger>(),
        ModelDb.Card<DefendGunslinger>(),
        ModelDb.Card<DefendGunslinger>(),
        ModelDb.Card<Reload>(),
        ModelDb.Card<Reload>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<CylinderRelic>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override Color EnergyLabelOutlineColor => new("1F1F1F");

    public override Color DialogueColor => new("2C2C2C");

    public override Color MapDrawingColor => new("4D4D4D");

    public override Color RemoteTargetingLineColor => new("6B6B6B");

    public override Color RemoteTargetingLineOutline => new("1F1F1F");

    public override List<string> GetArchitectAttackVfx()
    {
        var num = 3;
        var list = new List<string>(num);
        CollectionsMarshal.SetCount(list, num);
        var span = CollectionsMarshal.AsSpan(list);
        var num2 = 0;
        span[num2] = "vfx/vfx_attack_slash";
        num2++;
        span[num2] = "vfx/vfx_attack_blunt";
        num2++;
        span[num2] = "vfx/vfx_heavy_blunt";
        return list;
    }
}
