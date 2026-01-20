using MNGUI.GUI.MNGui;
using MNGUI.GUIElements;
using MNGUITest.MNGUI.Extensions;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

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

        var elem = guiStd.TextAutoBoxSize("Hoge");
        rootBound.WithChild(elem.Bounds);

        container.Add(elem);

        container.SetChildBound(rootBound);

        SingleComposer.Compose();
    }

    public override void OnGuiOpened() {
        // SetupDialog every time opened
        SetupDialog();
        base.OnGuiOpened();
    }
}
