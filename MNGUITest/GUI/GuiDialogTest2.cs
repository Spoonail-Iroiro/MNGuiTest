using MNGui;
using MNGui.Layouts;
using MNGui.DialogBuilders;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

namespace MNGUITest.GUI;
public class GuiDialogTest2 : GuiDialog {
    public override string ToggleKeyCombinationCode => "demoscrollgui";

    public GuiDialogTest2(ICoreClientAPI capi) : base(capi) {
        SetupDialog();
    }

    private void SetupDialog() {
        int insetWidth = 900;
        int insetHeight = 300;
        int insetDepth = 3;
        int rowHeight = 35;
        int rowCount = 40;

        // Auto-sized dialog at the center of the screen
        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

        // Bounds of main inset for scrolling content in the GUI
        ElementBounds insetBounds = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight, insetWidth, insetHeight);
        ElementBounds scrollbarBounds = insetBounds.RightCopy().WithFixedWidth(20);

        // Create child elements bounds for within the inset
        ElementBounds clipBounds = insetBounds.ForkContainingChild(GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding);
        ElementBounds containerBounds = insetBounds.ForkContainingChild(GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding);
        ElementBounds containerRowBounds = ElementBounds.Fixed(0, 0, insetWidth, rowHeight);

        // Dialog background bounds
        ElementBounds bgBounds = ElementBounds.Fill
            .WithFixedPadding(GuiStyle.ElementToDialogPadding)
            .WithSizing(ElementSizing.FitToChildren)
            .WithChildren(insetBounds, scrollbarBounds);

        // Create the dialog
        SingleComposer = capi.Gui.CreateCompo("demoScrollGui", dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar("Scroll Me!", OnTitleBarCloseClicked)
            .BeginChildElements()
                .AddInset(insetBounds, insetDepth)
                .BeginClip(clipBounds)
                    .AddContainer(containerBounds, "scroll-content")
                .EndClip()
                .AddVerticalScrollbar(OnNewScrollbarValue, scrollbarBounds, "scrollbar")
            .EndChildElements();

        // Add desired scrollable content to the container
        GuiElementContainer scrollArea = SingleComposer.GetContainer("scroll-content");
        for (int i = 0; i < rowCount; i++) {
            scrollArea.Add(new GuiElementStaticText(capi, $"- Example Row {i + 1} -", EnumTextOrientation.Center, containerRowBounds, CairoFont.WhiteSmallishText()));
            containerRowBounds = containerRowBounds.BelowCopy();
        }

        var boundsHie = DebugUtil.GetBoundsTree(SingleComposer.Bounds);
        capi.Logger.Event(boundsHie);

        // Compose the dialog
        SingleComposer.Compose();

        // After composing dialog, need to set the scrolling area heights to enable scroll behavior
        float scrollVisibleHeight = (float)clipBounds.fixedHeight;
        float scrollTotalHeight = rowHeight * rowCount;
        SingleComposer.GetScrollbar("scrollbar").SetHeights(scrollVisibleHeight, scrollTotalHeight);

    }

    private void OnNewScrollbarValue(float value) {
        ElementBounds bounds = SingleComposer.GetContainer("scroll-content").Bounds;
        bounds.fixedY = 5 - value;
        bounds.CalcWorldBounds();
    }

    private void OnTitleBarCloseClicked() {
        TryClose();
    }
}
