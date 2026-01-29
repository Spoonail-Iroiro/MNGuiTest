using MNGui.Extensions;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

namespace MNGuiTest.GUI;
public class GuiDialogSimplestContainer : GuiDialogGeneric {
    public GuiDialogSimplestContainer(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
        //SetupDialog();
    }

    public GuiComposer CreateCompoWithStandardLayout(int fixedHeight, string dialogId) {
        var dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
        dialogBounds.Name = "bounds-dialog";

        var bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
        bgBounds.Name = "bounds-bg";
        bgBounds.BothSizing = ElementSizing.FitToChildren;

        var insetBounds = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight, 10, fixedHeight + GuiStyle.HalfPadding * 2);
        insetBounds.Name = "bounds-inset";
        insetBounds.horizontalSizing = ElementSizing.FitToChildren;
        //bgBounds.WithChild(insetBounds);

        var scrollBarBounds = insetBounds.CopyOffsetedSibling()
            .WithFixedWidth(20)
            .WithSizing(ElementSizing.Fixed);
        scrollBarBounds.Name = "bounds-scroll-bar";
        scrollBarBounds.RightOf(insetBounds, 3);

        // Bounds to add paddings between the inset and the clip
        // Adding paddings to the inset doesnt work: inset drawing breaks
        // TODO: Try adding paddings to the clip - but doesn't sound right
        var clipParentBounds = insetBounds.ForkContainingChild();
        clipParentBounds.Name = "bounds-clipparent";
        clipParentBounds.WithFixedPadding(GuiStyle.HalfPadding);
        clipParentBounds.horizontalSizing = ElementSizing.FitToChildren;

        var clipBounds = clipParentBounds.ForkContainingChild();
        clipBounds.Name = "bounds-clip";
        clipBounds.horizontalSizing = ElementSizing.FitToChildren;
        //insetBounds.WithChild(clipBounds);

        var containerBounds = clipBounds.ForkContainingChild();
        containerBounds.BothSizing = ElementSizing.FitToChildren;
        containerBounds.Name = "container";

        var composer = capi.Gui.CreateCompo(dialogId, dialogBounds);
        composer
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar(dialogId, () => TryClose())
            .BeginChildElements(bgBounds) // Begin bgBounds child
                .AddInset(insetBounds, 3)
                .BeginChildElements() // Begin insetBounds child
                    .BeginChildElements(clipParentBounds) // Begin clipParentBounds (now child of insetBounds) child
                        .BeginClip(clipBounds) // Begin clipBounds child (auto)
                            .AddInteractiveElement(new GuiElementContainer(capi, containerBounds), "container-main")
                        .EndClip()
                    .EndChildElements()
                .EndChildElements()
                .AddVerticalScrollbar(val => { }, scrollBarBounds, "scrollbar-main")
            .EndChildElements();

        return composer;
    }

    public void SetupDialog() {
        SingleComposer = CreateCompoWithStandardLayout(400, "simplest-container");

        var container = SingleComposer.GetElement<GuiElementContainer>("container-main");
        container.Tabbable = true;

        var textInput = new GuiElementTextInput(capi, ElementBounds.Fixed(10, 10, 100, GuiStyle.SmallishFontSize), null, CairoFont.WhiteDetailText());
        textInput.BeforeCalcBounds();
        textInput.Bounds.CalcWorldBounds();
        //var textInput = new GuiElementTextInput(capi, ElementBounds.Fixed(0, 0, 100, GuiStyle.DetailFontSize), null, CairoFont.WhiteDetailText());
        container.Add(textInput);

        var text1 = new GuiElementStaticText(capi, "Foo", bounds: ElementBounds.Fixed(200, 0, 100, GuiStyle.SmallishFontSize), orientation: EnumTextOrientation.Left, font: CairoFont.WhiteSmallishText());
        container.Add(text1);

        var text2 = new GuiElementStaticText(capi, "Bar", bounds: ElementBounds.Fixed(0, 200, 100, GuiStyle.SmallishFontSize), orientation: EnumTextOrientation.Left, font: CairoFont.WhiteSmallishText());
        container.Add(text2);

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
