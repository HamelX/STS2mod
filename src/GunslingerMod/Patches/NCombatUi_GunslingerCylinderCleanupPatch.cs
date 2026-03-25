using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace GunslingerMod.Patches;

// Ensure cylinder UI only exists during combat.
[HarmonyPatch(typeof(NCombatUi), "Deactivate")]
public static class NCombatUi_GunslingerCylinderCleanupPatch
{
    public static void Postfix()
    {
        try
        {
            var room = NCombatRoom.Instance;
            if (room == null)
                return;

            foreach (var creatureNode in room.CreatureNodes)
            {
                if (creatureNode == null || !GodotObject.IsInstanceValid(creatureNode))
                    continue;

                var ui = creatureNode.GetNodeOrNull<Node>("GunslingerCylinderUi");
                ui?.QueueFree();
            }
        }
        catch (System.Exception e)
        {
            GD.Print($"[Gunslinger] Cleanup patch exception: {e}");
        }
    }
}
