using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using GunslingerMod.Models.Characters;
using GunslingerMod.Nodes;

namespace GunslingerMod.Patches;

[HarmonyPatch(typeof(NCombatUi), "Activate")]
public static class NCombatUi_GunslingerCylinderPatch
{
    private const string CylinderUiScenePath = "res://scenes/ui/gunslinger_cylinder.tscn";
    private const string CylinderUiName = "GunslingerCylinderUi";

    public static void Postfix(NCombatUi __instance, CombatState state)
    {
        GD.Print("[Gunslinger] NCombatUi.Activate postfix running");

        var me = LocalContext.GetMe(state);

        var scene = ResourceLoader.Load<PackedScene>(CylinderUiScenePath);
        if (scene == null)
        {
            GD.Print($"[Gunslinger] Failed to load scene: {CylinderUiScenePath}");
            return;
        }

        // Create a cylinder UI for every Gunslinger player (including remote teammates) so others can see it.
        // Debug: log layer info for background + any CanvasLayers in combat UI
        try
        {
            var room = NCombatRoom.Instance;
            var bg = room?.GetNodeOrNull<Control>("%BgContainer");
            if (bg != null)
            {
                var bgLayer = bg.GetParent() as CanvasLayer;
                GD.Print($"[Gunslinger] BgContainer ZIndex={bg.ZIndex}, CanvasLayer={(bgLayer != null ? bgLayer.Layer.ToString() : "<none>")}");
            }

            var uiRoot = room?.Ui as Node;
            if (uiRoot != null)
            {
                foreach (var n in uiRoot.GetChildren())
                {
                    if (n is CanvasLayer cl)
                        GD.Print($"[Gunslinger] Combat UI CanvasLayer: {cl.Name} layer={cl.Layer}");
                }
            }
        }
        catch (Exception e)
        {
            GD.Print($"[Gunslinger] Layer debug exception: {e}");
        }

        foreach (var p in state.Players)
        {
            if (p.Character is not Gunslinger)
                continue;

            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(p.Creature);
            if (creatureNode == null)
                continue;

            // Remove previous instance (if any) to avoid duplicates.
            var existing = creatureNode.GetNodeOrNull<Node>(CylinderUiName);
            existing?.QueueFree();

            var layout = scene.Instantiate<Control>();
            layout.Name = "CylinderLayout";

            var ui = new NGunslingerCylinderUi();
            ui.Name = CylinderUiName;
            var isLocal = me != null && p.NetId == me.NetId;
            ui.Initialize(p, creatureNode, layout, isLocal: isLocal);
            creatureNode.AddChild(ui);
        }

        GD.Print("[Gunslinger] Cylinder UI instantiated for Gunslinger players");
    }
}
