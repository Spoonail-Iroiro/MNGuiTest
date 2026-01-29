using System.Collections.Generic;
using MNGui;
using MNGui.GuiElements;
using Vintagestory.API.Client;
using MNGui.Extensions;
using System.Text;

namespace MNGuiTest.GUI;
public class GuiDialogSimplest : GuiDialogGeneric {
    public GuiDialogSimplest(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
        //SetupDialog();
    }

    public GuiComposer CreateCompoWithStandardLayout(int fixedHeight) {
        var dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
        dialogBounds.Name = "bounds-dialog";

        var bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
        bgBounds.Name = "bounds-bg";
        bgBounds.BothSizing = ElementSizing.FitToChildren;

        var clientAreaBounds = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight, 10, 10);
        clientAreaBounds.BothSizing = ElementSizing.FitToChildren;

        // DynamicText is simply unable to auto wrap properly. Use RichText, or just use it with one-line
        //var dynText = """
        //    My
        //    Special
        //    Dynamic
        //    Text
        //    """;
        //var elem1 = new GuiElementDynamicText(capi, dynText, CairoFont.WhiteSmallishText(), ElementBounds.FixedSize(900, 200));
        var elem1 = new GuiElementContainer(capi, ElementBounds.FixedSize(200, 400));

        var composer = capi.Gui.CreateCompo(DialogTitle, dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar(DialogTitle, () => TryClose())
            .BeginChildElements(bgBounds) // Begin bgBounds child
                .BeginChildElements(clientAreaBounds)
                    .AddInteractiveElement(elem1, "container-main")
                .EndChildElements()
            //.AddInteractiveElement(new MNGuiElementContainer(capi, containerBounds), "scroll-content")
            .EndChildElements();

        // Scroll bar setting

        //var container = Composer.GetElement("scroll-content") as OldMNGuiElementContainer;

        //ChildLayout.Layout(container);

        //container.Bounds.CalcWorldBounds();
        //ChildLayout.BeforeComposerCompose();

        //var boundsHie = DebugUtil.GetBoundsTree(Composer.Bounds);
        //capi.Logger.Event(boundsHie);

        //Composer.Compose();

        //container.Bounds.CalcWorldBounds();

        //var mainScrollBar = Composer.GetScrollbar("scroll-bar");
        //mainScrollBar.SetHeights(scrollBarBounds.OuterHeightInt, (float)(containerBounds.OuterHeight + GuiStyle.HalfPadding * 2));
        //scrollBarContentFixedY = container.Bounds.fixedY;

        //return Composer;
        return composer;
    }

    public void SetupDialog() {
        SingleComposer = CreateCompoWithStandardLayout(400);

        var container = SingleComposer.GetElement<GuiElementContainer>("container-main");
        container.Tabbable = true;

        var textInput = new GuiElementTextInput(capi, ElementBounds.Fixed(10, 10, 100, GuiStyle.SmallishFontSize), null, CairoFont.WhiteDetailText());
        //var textInput = new GuiElementTextInput(capi, ElementBounds.Fixed(0, 0, 100, GuiStyle.DetailFontSize), null, CairoFont.WhiteDetailText());
        container.Add(textInput);

        SingleComposer.Compose();

        var boundsInfo = DebugUtil.GetBoundsTree(SingleComposer.Bounds);
        capi.Logger.Event(boundsInfo);
    }

    public override void OnGuiOpened() {
        // SetupDialog every time opened
        SetupDialog();
        base.OnGuiOpened();
    }
}
