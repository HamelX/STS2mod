using System;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using GunslingerMod.Models.Powers;

namespace GunslingerMod.Nodes;

public partial class NGunslingerCylinderUi : Control
{
    private const int MaxRounds = 6;
    private const float LocalScaleFactor = 0.25f;
    private const float RemoteScaleFactor = 0.20f;

    private Player? _player;
    private bool _isLocal;
    private NCreature? _creatureNode;
    private Control? _layoutRoot;
    private bool _pivotReady;
    private float _currentRotationRad;
    private float _targetRotationRad;
    private int _lastChamberIndex = -1;
    private readonly List<TextureRect> _bullets = new();
    private readonly List<Control> _hoverAnchors = new();
    private readonly HashSet<int> _hoverActive = new();

    public static event Action<int>? ChamberClicked;

    private Texture2D? _bulletDefaultTex;
    private Texture2D? _bulletSealTex;

    public void Initialize(Player player, NCreature creatureNode, Control layoutRoot, bool isLocal)
    {
        _player = player;
        _creatureNode = creatureNode;
        _layoutRoot = layoutRoot;
        _isLocal = isLocal;

        if (_layoutRoot.GetParent() != this)
            AddChild(_layoutRoot);
    }

    public override void _Ready()
    {
        GD.Print("[Gunslinger] NGunslingerCylinderUi _Ready");

        TopLevel = false;
        ZAsRelative = true;
        ZIndex = 1;

        // Let mouse events pass through this top-level container to chamber nodes.
        MouseFilter = MouseFilterEnum.Ignore;

        var scale = _isLocal ? LocalScaleFactor : RemoteScaleFactor;
        Scale = new Vector2(scale, scale);
        if (!_isLocal)
            Modulate = new Color(1f, 1f, 1f, 0.85f);

        Node root = _layoutRoot ?? this;
        if (_layoutRoot != null)
        {
            // Ensure layout doesn't swallow mouse input.
            _layoutRoot.MouseFilter = MouseFilterEnum.Ignore;

            _layoutRoot.PivotOffset = _layoutRoot.Size / 2f;
            _pivotReady = true;
        }

        var cylinder = root.GetNodeOrNull<TextureRect>("Cylinder");
        var tex = ResourceLoader.Load<Texture2D>("res://images/ui/gunslinger_cylinder.png");
        if (cylinder != null && tex != null)
        {
            cylinder.Texture = tex;
        }
        else
        {
            GD.Print($"[Gunslinger] Cylinder node or texture missing (node={cylinder != null}, tex={tex != null})");
        }

        // debug label removed

        // Chamber index mapping (0..5): index 0 is 12 o'clock (firing chamber).
        // With _layoutRoot rotating CCW (negative degrees) as ChamberIndex increases,
        // the *next* chamber arriving at 12 o'clock is the one that was at top-right.
        // So indices proceed clockwise in the unrotated layout.
        _bullets.Add(root.GetNode<TextureRect>("BulletTop"));         // 0 (12 o'clock)
        _bullets.Add(root.GetNode<TextureRect>("BulletTopRight"));    // 1
        _bullets.Add(root.GetNode<TextureRect>("BulletBottomRight")); // 2
        _bullets.Add(root.GetNode<TextureRect>("BulletBottom"));      // 3
        _bullets.Add(root.GetNode<TextureRect>("BulletBottomLeft"));  // 4
        _bullets.Add(root.GetNode<TextureRect>("BulletTopLeft"));     // 5

        // Bullet textures (default + seal)
        _bulletDefaultTex = _bullets.Count > 0 ? _bullets[0].Texture : null;
        _bulletSealTex = ResourceLoader.Load<Texture2D>("res://images/ui/gunslinger_bullet_seal.png");

        // Hover tooltips for chambers.
        // IMPORTANT: NHoverTipSet positions using owner.Size * owner.Scale (not global scale).
        // Since our cylinder UI is scaled via parent Scale, the bullet nodes' local Scale stays 1
        // and tooltip alignment math can be wildly off-screen.
        // Fix: create an unscaled TopLevel anchor per chamber and use that as the hover-tip owner.
        for (var i = 0; i < _bullets.Count; i++)
        {
            var anchor = new Control
            {
                Name = $"HoverAnchor{i}",
                TopLevel = true,
                MouseFilter = MouseFilterEnum.Ignore,
                Size = new Vector2(40, 40)
            };
            AddChild(anchor);
            _hoverAnchors.Add(anchor);

            var chamberIndex = i;
            var bulletNode = _bullets[i];
            bulletNode.MouseFilter = MouseFilterEnum.Stop;
            bulletNode.MouseDefaultCursorShape = CursorShape.PointingHand;

            bulletNode.MouseEntered += () =>
            {
                _hoverActive.Add(chamberIndex);
                ShowChamberHoverTip(_hoverAnchors[chamberIndex], chamberIndex);
            };
            bulletNode.MouseExited += () =>
            {
                _hoverActive.Remove(chamberIndex);
                HideChamberHoverTip(_hoverAnchors[chamberIndex]);
            };

            bulletNode.GuiInput += (InputEvent e) =>
            {
                if (e is InputEventMouseMotion)
                {
                    if (!_hoverActive.Contains(chamberIndex))
                    {
                        _hoverActive.Add(chamberIndex);
                        ShowChamberHoverTip(_hoverAnchors[chamberIndex], chamberIndex);
                    }
                }
                else if (e is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
                {
                    ChamberClicked?.Invoke(chamberIndex);
                }
            };
        }

        UpdateUi(0);
    }

    private static void HideChamberHoverTip(Control owner)
    {
        NHoverTipSet.Remove(owner);
    }

    private void ShowChamberHoverTip(Control owner, int chamberIndex)
    {
        GD.Print($"[Gunslinger] Hover chamber {chamberIndex + 1}");
        // Clear any existing tip for this owner first.
        NHoverTipSet.Remove(owner);

        var power = _player?.Creature?.GetPower<CylinderPower>();
        var loaded = power?.IsLoaded(chamberIndex) ?? false;
        var isFiringChamber = (power?.ChamberIndex ?? 0) == chamberIndex;

        var titleLoc = new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", "CHAMBER.title");
        titleLoc.Add("n", (chamberIndex + 1).ToString());

        string desc;
        if (!loaded)
        {
            var descLoc = new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", "CHAMBER.desc_empty");
            desc = descLoc.GetFormattedText();
        }
        else
        {
            var ammoType = power?.GetAmmoType(chamberIndex) ?? CylinderPower.AmmoType.None;
            var sealLevel = ammoType == CylinderPower.AmmoType.Seal ? (power?.GetSealLevel(chamberIndex) ?? (byte)0) : (byte)0;

            var dmg = ammoType switch
            {
                CylinderPower.AmmoType.Enhanced => 10,
                CylinderPower.AmmoType.Tracer => 4,
                CylinderPower.AmmoType.Seal => 8 + sealLevel,
                _ => 6
            };

            var ammoNameKey = ammoType switch
            {
                CylinderPower.AmmoType.Normal => "AMMO.normal",
                CylinderPower.AmmoType.Enhanced => "AMMO.enhanced",
                CylinderPower.AmmoType.Penetrator => "AMMO.penetrator",
                CylinderPower.AmmoType.Tracer => "AMMO.tracer",
                CylinderPower.AmmoType.Seal => "AMMO.seal",
                _ => "AMMO.unknown"
            };

            var ammoNameLoc = new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", ammoNameKey);
            var sb = new System.Text.StringBuilder();
            sb.Append(ammoNameLoc.GetFormattedText());
            sb.Append("\n");
            var dmgLoc = new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", "CHAMBER.damage");
            dmgLoc.Add("d", dmg);
            sb.Append(dmgLoc.GetFormattedText());

            if (ammoType == CylinderPower.AmmoType.Penetrator)
                sb.Append("\n").Append(new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", "AMMO.penetrator_note").GetFormattedText());

            if (ammoType == CylinderPower.AmmoType.Tracer)
                sb.Append("\n").Append(new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", "AMMO.tracer_note").GetFormattedText());

            if (ammoType == CylinderPower.AmmoType.Seal)
            {
                sb.Append("\n").Append(new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", "AMMO.seal_level").GetFormattedText().Replace("{l}", sealLevel.ToString()));
                sb.Append("\n");
                sb.Append(sealLevel >= CylinderPower.SealThresholdUnblockable ? "[●] " : "[ ] ")
                    .Append(new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", "AMMO.seal_t8").GetFormattedText());
            }

            desc = sb.ToString();
        }

        if (isFiringChamber)
        {
            var firingLoc = new MegaCrit.Sts2.Core.Localization.LocString("static_hover_tips", "CHAMBER.firing_note");
            desc = desc + "\n" + firingLoc.GetFormattedText();
        }

        var tip = new HoverTip(titleLoc, desc);

        var viewportW = GetViewport().GetVisibleRect().Size.X;
        var alignment = owner.GlobalPosition.X > viewportW * 0.55f ? HoverTipAlignment.Left : HoverTipAlignment.Right;
        // Some game states set NHoverTipSet.shouldBlockHoverTips=true (e.g., while other UI wants to suppress tips).
        // Our cylinder chamber tips are purely informational, so we temporarily bypass this block.
        var prevBlock = NHoverTipSet.shouldBlockHoverTips;
        NHoverTipSet.shouldBlockHoverTips = false;
        var tipSet = NHoverTipSet.CreateAndShow(owner, tip, alignment);
        NHoverTipSet.shouldBlockHoverTips = prevBlock;

        tipSet.SetFollowOwner();
    }

    public override void _Process(double delta)
    {
        // Ensure this UI never leaks outside combat (e.g., map screen).
        if (!CombatManager.Instance.IsInProgress)
        {
            if (IsInsideTree())
                QueueFree();
            return;
        }

        // Hide while map screen is open (combat state may still be in progress).
        if (NMapScreen.Instance?.IsOpen == true)
        {
            Visible = false;
            return;
        }
        Visible = true;

        if (_player?.Creature == null || _creatureNode == null)
            return;

        var power = _player.Creature.GetPower<CylinderPower>();
        var amount = power?.Amount ?? 0;
        amount = Math.Clamp(amount, 0, MaxRounds);

        var headPos = _creatureNode.GetTopOfHitbox();

        // Place beside the character, but flip side based on screen half to reduce overlap in multiplayer.
        var viewportW = GetViewport().GetVisibleRect().Size.X;
        var toLeft = headPos.X > viewportW * 0.55f;
        var offsetX = toLeft ? -260f : 120f;
        var offsetY = _isLocal ? -120f : -150f;
        GlobalPosition = headPos + new Vector2(offsetX, offsetY);

        // Keep unscaled hover anchors glued to the (scaled/rotated) bullet nodes.
        for (var i = 0; i < _hoverAnchors.Count && i < _bullets.Count; i++)
        {
            var b = _bullets[i];
            var a = _hoverAnchors[i];
            // Center the anchor on the bullet.
            a.GlobalPosition = b.GlobalPosition + (b.Size * 0.5f) - (a.Size * 0.5f);
        }

        if (_layoutRoot != null)
        {
            if (!_pivotReady && _layoutRoot.Size != Vector2.Zero)
            {
                _layoutRoot.PivotOffset = _layoutRoot.Size / 2f;
                _pivotReady = true;
            }

            var chamberIndex = power?.ChamberIndex ?? 0;

            // 12 o'clock is the firing chamber. When ChamberIndex advances by 1, the cylinder should rotate
            // so that the next chamber arrives at 12 o'clock.
            // Godot angles are in radians for Rotation, and Mathf.LerpAngle expects radians.
            _targetRotationRad = Mathf.DegToRad(-60f * chamberIndex);

            // If chamber changed, snap the current rotation start from actual node rotation (prevents drift).
            if (_lastChamberIndex != chamberIndex)
            {
                _currentRotationRad = _layoutRoot.Rotation;
                _lastChamberIndex = chamberIndex;
            }

            // Smooth rotate toward target (radians)
            _currentRotationRad = Mathf.LerpAngle(_currentRotationRad, _targetRotationRad, (float)(10.0 * delta));
            _layoutRoot.Rotation = _currentRotationRad;

            // Snap when very close (avoids "never quite arrives")
            if (Mathf.Abs(Mathf.AngleDifference(_currentRotationRad, _targetRotationRad)) < 0.005f)
                _layoutRoot.Rotation = _targetRotationRad;
        }

        UpdateUi(amount);
    }

    private void UpdateUi(int amount)
    {
        var power = _player?.Creature?.GetPower<CylinderPower>();
        for (var i = 0; i < _bullets.Count; i++)
        {
            var filled = power?.IsLoaded(i) ?? (i < amount);
            var ammoType = power?.GetAmmoType(i) ?? CylinderPower.AmmoType.None;

            if (_bulletDefaultTex != null)
            {
                var tex = (ammoType == CylinderPower.AmmoType.Seal && _bulletSealTex != null)
                    ? _bulletSealTex
                    : _bulletDefaultTex;
                _bullets[i].Texture = tex;
            }

            // IMPORTANT: keep bullet nodes Visible even when empty so they can still receive hover events.
            // We hide the sprite visually using alpha=0 instead of Visible=false.
            _bullets[i].Visible = true;
            _bullets[i].Modulate = filled
                ? new Color(1f, 1f, 1f, 0.9f)
                : new Color(1f, 1f, 1f, 0f);
        }
    }
}
