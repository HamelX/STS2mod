using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Unlocks;
using GunslingerMod.Models.Cards;

namespace GunslingerMod.Models.CardPools;

public sealed class GunslingerCardPool : CardPoolModel
{
    public override string Title => "gunslinger";

    public override string EnergyColorName => "gunslinger";

    public override string CardFrameMaterialPath => "card_frame_purple";

    public override Color DeckEntryCardColor => new("4D4D4D");

    public override Color EnergyOutlineColor => new("2A2A2A");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
            ModelDb.Card<Shoot>(),
            ModelDb.Card<DefendGunslinger>(),
            ModelDb.Card<Reload>(),
            ModelDb.Card<TakeCover>(),
            ModelDb.Card<Evasion>(),
            ModelDb.Card<EchoNote>(),
            ModelDb.Card<QuickRack>(),
            ModelDb.Card<HotChamber>(),
            ModelDb.Card<Panning>(),
            ModelDb.Card<SprayFire>(),
            ModelDb.Card<SealLoad>(),
            ModelDb.Card<SigilGuard>(),
            ModelDb.Card<EtchedTracer>(),
            ModelDb.Card<ReadTheMark>(),
            ModelDb.Card<TracerLoad>(),
            ModelDb.Card<SteadyAim>(),
            ModelDb.Card<ImprintSqueeze>(),
            ModelDb.Card<ImprintCompression>(),
            ModelDb.Card<FanTheBrand>(),
            ModelDb.Card<RicochetShot>(),
            ModelDb.Card<ReboundNet>(),
            ModelDb.Card<ImprintManifestRicochet>(),
            ModelDb.Card<TracerConversion>(),
            ModelDb.Card<BallisticCompiler>(),
            ModelDb.Card<ChainBurst>(),
            ModelDb.Card<WalkingFire>(),
            ModelDb.Card<BlankFire>(),
            ModelDb.Card<CrossfireRhythm>(),
            ModelDb.Card<TagBurst>(),
            ModelDb.Card<RicochetSeal>(),
            ModelDb.Card<CasingCount>(),
            ModelDb.Card<SealTension>(),
            ModelDb.Card<HuntReload>(),
            ModelDb.Card<SealSearch>(),
            ModelDb.Card<SealAmplify>(),
            ModelDb.Card<EmptyTheMagazine>(),
            ModelDb.Card<OverclockDrum>(),
            ModelDb.Card<OverclockCharge>(),
            ModelDb.Card<ExecutionShot>(),
            ModelDb.Card<ImprintIgnition>(),
            ModelDb.Card<SealRite>(),
            ModelDb.Card<SealReleaseKai>(),
            ModelDb.Card<SealRampage>(),
            ModelDb.Card<SealInsight>(),
            ModelDb.Card<SealBarrier>(),
            ModelDb.Card<FinalVolley>(),
            ModelDb.Card<SealImprint>(),
            ModelDb.Card<DeadAngle>()
        ];
    }

    protected override IEnumerable<CardModel> FilterThroughEpochs(UnlockState unlockState, IEnumerable<CardModel> cards)
    {
        return cards.ToList();
    }
}
