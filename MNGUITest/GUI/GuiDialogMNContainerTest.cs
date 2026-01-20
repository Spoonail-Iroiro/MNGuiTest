using MNGUI.GUI.MNGui;
using MNGUI.GUIElements;
using MNGUITest.MNGUI.Extensions;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using MNGUI.Extensions;

namespace MNGUITest.GUI;
public class GuiDialogMNContainerTest : GuiDialogGeneric {
    public GuiDialogMNContainerTest(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
        SetupDialog();
    }

    public GuiComposer CreateCompoWithStandardLayout(int fixedHeight) {
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

        var clipBounds = insetBounds.ForkContainingChild(GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding, GuiStyle.HalfPadding);
        clipBounds.Name = "bounds-clip";
        clipBounds.horizontalSizing = ElementSizing.FitToChildren;
        //insetBounds.WithChild(clipBounds);

        var containerBounds = clipBounds.ForkContainingChild();
        containerBounds.BothSizing = ElementSizing.FitToChildren;
        containerBounds.Name = "container";
        //ContainerBounds = containerBounds;

        var composer = capi.Gui.CreateCompo(DialogTitle, dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar(DialogTitle, () => TryClose())
            .BeginChildElements(bgBounds) // Begin bgBounds child
                .AddInset(insetBounds, 3)
                .BeginChildElements() // Begin insetBounds child
                    .BeginClip(clipBounds) // Begin clipBounds child (auto)
                        .AddInteractiveElement(new MNGuiElementContainer(capi, containerBounds), "scroll-content")
                    .EndClip()
                .EndChildElements()
                .AddVerticalScrollbar(newValue => { }, scrollBarBounds, "scroll-bar")
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

        var guiStd = new GuiStd(capi);

        var container = SingleComposer.GetElement<MNGuiElementContainer>("scroll-content")!;

        var rootBound = ElementBounds.FixedSize(10, 10).WithSizing(ElementSizing.FitToChildren);
        rootBound.Name = "bounds-containerroot";

        var elements = new List<GuiElement>();

        var elem = guiStd.TextAutoBoxSize("Hoge");
        rootBound.WithChild(elem.Bounds);
        var l1RefBound = elem.Bounds;
        elements.Add(elem);

        var dynText = """
        My
        Special
        Dynamic
        Text
        """;
        var elem2 = new GuiElementDynamicText(capi, dynText, CairoFont.WhiteDetailText(), l1RefBound.CopyOffsetedSibling());
        rootBound.WithChild(elem2.Bounds);  // CopyOffsetedSibling doesn't make sibling???
        elem2.Bounds.fixedWidth = 50;
        elem2.Bounds.CalcWorldBounds();
        elem2.AutoHeight();
        elem2.Bounds.CalcWorldBounds();
        elem2.Bounds.FitToChildrenFixedRightOf(elem.Bounds);
        elements.Add(elem2);

        var elem2_2 = new GuiElementDynamicText(capi, dynText, CairoFont.WhiteDetailText(), l1RefBound.CopyOffsetedSibling());
        rootBound.WithChild(elem2_2.Bounds);
        elem2_2.Bounds.fixedWidth = 50;
        elem2_2.Bounds.fixedHeight = 50;
        elem2_2.Bounds.FitToChildrenFixedRightOf(elem2.Bounds);
        elements.Add(elem2_2);

        var elem3 = guiStd.TextAutoBoxSize("Fuga");
        rootBound.WithChild(elem3.Bounds);
        elem3.Bounds.FitToChildrenFixedUnder(elem.Bounds);
        elements.Add(elem3);


        foreach (var element in elements) {
            container.Add(element);
        }

        container.SetChildBound(rootBound);

        SingleComposer.Compose();

        var boundsInfo = DebugUtil.GetBoundsTree(container.Bounds);
        capi.Logger.Event(boundsInfo);
    }

    public override void OnGuiOpened() {
        // SetupDialog every time opened
        SetupDialog();
        base.OnGuiOpened();
    }
}
