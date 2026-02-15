using MNGui.DialogBuilders;
using MNGui.Extensions;
using MNGui.GuiElements;
using MNGui.GuiElements.Layout;
using MNGui.Layouts;
using MNGui.Std;
using MNGui.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;

namespace MNGuiTest.Gui.Specific;

public class GuiDialogDebugWindow : GuiDialogGeneric {
    ContainerDialogController? controller;
    WidgetDebug? widgetDebug;
    long? tickId = null;
    GUIDebugModSystem? modSystem;

    public GuiDialogDebugWindow(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
        modSystem = capi.ModLoader.GetModSystem<GUIDebugModSystem>();
        if (modSystem == null) {
            capi.Logger.Warning($"{nameof(GUIDebugModSystem)} is required to work!");
        }
    }

    public void SetupDialog() {
        var elementStd = new ElementStd(capi);

        var builder = new ContainerDialogBuilder()
            .WithStandardClose(false);

        var topInsetLayoutBuilder = new InsetContainerLayoutBuilder(capi, "container-top")
            .WithSizeFitToChildren(BoxSide.Horizontal)
            .WithSizeFixed(BoxSide.Vertical, 500)
            .WithInitialLayout(WidgetDebug.InitialLayout(capi));
        var topInsetLayout = topInsetLayoutBuilder.Build();

        var layout = new VerticalLayout(capi, 10)
            .Add(
                topInsetLayout
            )
            .Add(
                new HorizontalLayout(capi, 5)
                    .Add(
                        new GuiElementSwitch(capi, null, ElementBounds.FixedSize(26, 26)),
                        "switch-excludeblank"
                    )
                    .Add(
                        elementStd.TextAutoBoxSize("Exclude dummy elements", font: CairoFont.WhiteSmallText())
                    )
            )
            .Add(
                new HorizontalLayout(capi, 5)
                    .Add(
                        new GuiElementSwitch(capi, null, ElementBounds.FixedSize(26, 26)),
                        "switch-updateonlywhenhotkey"
                    )
                    .Add(
                        elementStd.TextAutoBoxSize("Update only when Shift pressed", font: CairoFont.WhiteSmallText())
                    )
            )
            .Add(
                new HorizontalLayout(capi, 50)
                    .Add(
                        new MNGuiElementTextButton(capi, "Close", ElementBounds.FixedSize(100, 26)),
                        "button-close"
                    )
            );

        builder.SetChildLayout(layout);

        ClearComposers();
        SingleComposer = builder.Layout(capi, this, nameof(GuiDialogDebugWindow));

        controller = new(capi, SingleComposer, layout);

        widgetDebug = new WidgetDebug(capi, controller);
        widgetDebug.SetText("Empty");

        var closeButton = controller.GetElement<MNGuiElementTextButton>("button-close");
        closeButton.EventClicked = () => TryClose();

        tickId = capi.Event.RegisterGameTickListener(OnClientGameTick, 500);
    }

    public void OnClientGameTick(float dt) {
        if (this.modSystem == null) return;
        if (widgetDebug == null) return;

        var switchElem = controller?.GetElement<GuiElementSwitch>("switch-excludeblank");
        var excludeBlank = switchElem?.On ?? false;

        var switchOnlyWhenHotkey = controller?.GetElement<GuiElementSwitch>("switch-updateonlywhenhotkey");
        var enableOnlyWhenHotkey = switchOnlyWhenHotkey?.On ?? false;

        if (enableOnlyWhenHotkey && !capi.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft]) return;

        var cmdSB = new StringBuilder();
        cmdSB.AppendLine("Element Info:");

        var hoveredElements = modSystem.FindHoveredElements().ToList();

        foreach (var elem in hoveredElements) {
            if (excludeBlank && (elem is GuiElementDebugHorizontalLayout || elem is GuiElementDebugVerticalLayout || elem is GuiElementDummy)) {
                continue;
            }
            cmdSB.Append($"{elem.GetType().Name}");
            if (elem.Bounds != null) {
                var bounds = elem.Bounds;
                cmdSB.AppendLine($" ({bounds.horizontalSizing}, {bounds.verticalSizing})");
                cmdSB.AppendLine($"Pos: ({BoundsUtil.UnScaled(bounds.absX):F2}, {BoundsUtil.UnScaled(bounds.absY):F2})");
                cmdSB.AppendLine($"Size: ({bounds.UnscaledOuterWidth():F2}, {bounds.UnscaledOuterHeight():F2})");
            }
        }

        widgetDebug.SetText(cmdSB.ToString());
    }

    public override void OnGuiOpened() {
        SetupDialog();
        base.OnGuiOpened();
    }

    public override void OnGuiClosed() {
        base.OnGuiClosed();
        if (tickId != null) {
            capi.Event.UnregisterGameTickListener(tickId.Value);
        }
    }

    public override bool OnEscapePressed() {
        return false;
    }

    //public override bool PrefersUngrabbedMouse => false;

    public override EnumDialogType DialogType => EnumDialogType.HUD;

}

public class WidgetDebug {
    public readonly static string DynamicTextName = "dyntext-widgetdebug";
    ICoreClientAPI capi;
    ContainerDialogController controller;

    public WidgetDebug(ICoreClientAPI capi, ContainerDialogController controller) {
        this.capi = capi;
        this.controller = controller;
    }

    public static LayoutWithElementBounds InitialLayout(ICoreClientAPI capi) {
        var layout = new SingleLayout(
                new GuiElementDynamicText(capi, "Empty", CairoFont.WhiteDetailText(), ElementBounds.FixedSize(400, 300)),
                DynamicTextName
            );
        return layout;
    }

    public void SetText(string text) {
        try {
            var dynText = controller.GetElement<GuiElementDynamicText>(DynamicTextName);
            if (dynText == null) throw new InvalidOperationException($"Couldn't find dynamic text. Did you forget set layout with InitialLayout?");

            dynText.SetNewText(text, autoHeight: true);
            controller.OnBoundsUpdated();
        }
        catch (Exception ex) {
            capi.Logger.Warning(ex);
        }
    }
}
