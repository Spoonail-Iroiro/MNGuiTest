using MNGui;
using MNGui.GuiElements;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using MNGui.Extensions;
using MNGui.GuiElements.Layout;

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
        //ContainerBounds = containerBounds;

        var composer = capi.Gui.CreateCompo(DialogTitle, dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar(DialogTitle, () => TryClose())
            .BeginChildElements(bgBounds) // Begin bgBounds child
                .AddInset(insetBounds, 3)
                .BeginChildElements() // Begin insetBounds child
                    .BeginChildElements(clipParentBounds) // Begin clipParentBounds (now child of insetBounds) child
                        .BeginClip(clipBounds) // Begin clipBounds child (auto)
                            .AddInteractiveElement(new MNGuiElementContainer(capi, containerBounds), "scroll-content")
                        .EndClip()
                    .EndChildElements()
                .EndChildElements()
                .AddVerticalScrollbar(newValue => { }, scrollBarBounds, "scroll-bar")
            .EndChildElements();

        var boundsInfo = DebugUtil.GetBoundsTree(composer.Bounds);
        capi.Logger.Event(boundsInfo);

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
        var elem2 = new GuiElementDynamicText(capi, dynText, CairoFont.WhiteDetailText(), ElementBounds.FixedSize(100, 900));
        rootBound.WithChild(elem2.Bounds);
        elem2.Bounds.CalcWorldBounds();
        elem2.AutoHeight();
        elem2.Bounds.FitToChildrenFixedRightOf(elem.Bounds);
        elements.Add(elem2);

        var elem3 = guiStd.TextAutoBoxSize("Fuga");
        rootBound.WithChild(elem3.Bounds);
        elem3.Bounds.FitToChildrenFixedUnder(elem.Bounds);
        elements.Add(elem3);

        {
            var layout1 = new GuiElementParent(capi, GuiStd.ElementBoundsFitToChildren());
            rootBound.WithChild(layout1.Bounds);
            layout1.Bounds.FitToChildrenFixedUnder(elem3.Bounds);
            elements.Add(layout1);

            var layout1_1 = new GuiElementDebugHorizontalLayout(capi, GuiStd.ElementBoundsFitToChildren());
            layout1.Bounds.WithChild(layout1_1.Bounds);
            elements.Add(layout1_1);

            GuiElement? elem_l1_1_e1 = null;
            {
                var bounds_l1 = ElementBounds.FixedSize(100, 30);
                var elem_l1 = new GuiElementTextButton(capi, "Button1", CairoFont.ButtonText(), CairoFont.ButtonText(), null, bounds_l1);
                layout1_1.Bounds.WithChild(elem_l1.Bounds);
                elements.Add(elem_l1);
                elem_l1.BeforeCalcBounds();
                elem_l1.Bounds.CalcWorldBounds();
                elem_l1_1_e1 = elem_l1;
            }

            GuiElement? elem_l1_1_e2 = null;
            {
                var bounds_l1 = ElementBounds.FixedSize(70, 30);
                var elem_l1 = new GuiElementTextButton(capi, "Button1", CairoFont.ButtonText(), CairoFont.ButtonText(), null, bounds_l1);
                layout1_1.Bounds.WithChild(elem_l1.Bounds);
                elements.Add(elem_l1);
                elem_l1.BeforeCalcBounds();
                elem_l1.Bounds.CalcWorldBounds();

                elem_l1.Bounds.FitToChildrenFixedRightOf(elem_l1_1_e1.Bounds);

                elem_l1_1_e2 = elem_l1;
            }

            layout1_1.BeforeCalcBounds();
            layout1_1.Bounds.CalcWorldBounds();


            var layout1_2 = new GuiElementParent(capi, GuiStd.ElementBoundsFitToChildren());
            layout1.Bounds.WithChild(layout1_2.Bounds);
            elements.Add(layout1_2);

            GuiElement? elem_l1_2_e1 = null;
            {
                var bounds_l1 = ElementBounds.FixedSize(50, 30);
                var elem_l1 = new GuiElementTextButton(capi, "yes", CairoFont.ButtonText(), CairoFont.ButtonText(), null, bounds_l1);
                layout1_2.Bounds.WithChild(elem_l1.Bounds);
                elements.Add(elem_l1);
                elem_l1.BeforeCalcBounds();
                elem_l1.Bounds.CalcWorldBounds();
                elem_l1_2_e1 = elem_l1;
            }

            GuiElement? elem_l1_2_e2 = null;
            {
                var bounds_l1 = ElementBounds.FixedSize(70, 30);
                var elem_l1 = new GuiElementTextButton(capi, "no", CairoFont.ButtonText(), CairoFont.ButtonText(), null, bounds_l1);
                layout1_2.Bounds.WithChild(elem_l1.Bounds);
                elements.Add(elem_l1);
                elem_l1.BeforeCalcBounds();
                elem_l1.Bounds.CalcWorldBounds();

                elem_l1.Bounds.FitToChildrenFixedRightOf(elem_l1_2_e1.Bounds);

                elem_l1_2_e2 = elem_l1;
            }

            layout1_2.BeforeCalcBounds();
            layout1_2.Bounds.CalcWorldBounds();
            layout1_2.Bounds.FitToChildrenFixedRightOf(layout1_1.Bounds);

            layout1.BeforeCalcBounds();
            layout1.Bounds.CalcWorldBounds();
        }


        foreach (var element in elements) {
            container.Add(element);
        }

        container.SetChildBound(rootBound);

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
