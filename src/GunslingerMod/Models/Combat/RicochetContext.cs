using System.Threading;

namespace GunslingerMod.Models.Combat;

public static class RicochetContext
{
    private static readonly AsyncLocal<RicochetInfo?> _current = new();

    public sealed class RicochetInfo
    {
        public decimal Damage { get; }

        public RicochetInfo(decimal damage)
        {
            Damage = damage;
        }
    }

    public static RicochetInfo? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }

    public static bool IsRicocheting => _current.Value != null;
}
