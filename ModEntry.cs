using System;
using Godot;
using GunslingerMod.Framework.Bootstrap;
using MegaCrit.Sts2.Core.Modding;

[ModInitializer("Initialize")]
public class ModEntry
{
    public static void Initialize()
    {
        try
        {
            GD.Print("[Gunslinger] ModEntry.Initialize start");
            GunslingerBootstrap.Initialize();
            GD.Print("[Gunslinger] ModEntry.Initialize complete");
        }
        catch (Exception ex)
        {
            GD.Print($"[Gunslinger] ModEntry.Initialize exception: {ex}");
        }
    }
}
