using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using MNGui.GuiElements;
using MNGui.Std;
using MNGui.Layouts;
using MNGui.DialogBuilders;
using MNGui.Layouts.Extensions;

namespace MNGuiTest.Gui;
public class GuiDialogRefactoredLayoutTest : GuiDialogGeneric {
    public StandardDialogBuilder? DialogBuilder { get; set; }

    StandardDialogController? dialogController;

    List<string> posts = new List<string>();

    public GuiDialogRefactoredLayoutTest(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {

    }

    public void SetupDialog() {
        DialogBuilder = new StandardDialogBuilder();
        var guiStd = new ElementStd(capi);
        var elementStd = new ElementStd(capi);

        var contentLayout = new VerticalLayout(capi)
            .Add(
                new HorizontalLayout(capi).Add(elementStd.TextAutoBoxSize("123")).Add(elementStd.TextAutoBoxSize("456"))
            )
            .Add(
                new HorizontalLayout(capi).Add(elementStd.TextAutoBoxSize("ABCDEF")).Add(elementStd.TextAutoBoxSize("XYZ"))
            );

        var insetContainerLayoutBuilder = new InsetContainerLayoutBuilder(capi, "container-insideclip")
            .WithSizeFitToChildren(BoxSide.Horizontal)
            .WithSizeFitToChildrenRange(BoxSide.Vertical, 100)
            .WithInitialLayout(contentLayout);
        var insetContainerLayout = insetContainerLayoutBuilder.Build();

        var layout = new VerticalLayout(capi, 5)
            .Add(
                // Test for horizontal layout in horizontal layout
                new HorizontalLayout(capi, 5, vAlign: AlignmentVertical.Middle)
                .Add(
                    new HorizontalLayout(capi, 2, vAlign: AlignmentVertical.Middle)
                        .Add(guiStd.TextAutoBoxSize("input"))
                        .Add(
                            new GuiElementTextInput(capi, ElementBounds.FixedSize(100, GuiStyle.SmallishFontSize), null, font: CairoFont.WhiteSmallText()),
                            "input-top",
                            hSizePolicy: SizePolicy.Stretch
                        )
                )
                .Add(new MNGuiElementTextButton(capi, "submit", ElementBounds.FixedSize(100, GuiStyle.SmallishFontSize), font: CairoFont.WhiteSmallishText()))
            )
            .Add(
                // Horizontal spacing element (to test other stretcch layouts)
                new HorizontalLayout(capi)
                .Add(guiStd.TextAutoBoxSize("123"))
                .Add(guiStd.TextAutoBoxSize("456789012345678901234567890", font: CairoFont.WhiteSmallishText()))
            )
            .Add(
                new HorizontalLayout(capi)
                .Add(new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 24), null, CairoFont.TextInput()))
                .Add(guiStd.TextAutoBoxSize("456"))
            )
            .Add(
                new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 24), null, CairoFont.TextInput()),
                hSizePolicy: SizePolicy.Stretch
            )
            .Add(
                new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 24), null, CairoFont.TextInput())
            )
            .Add(
                new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 24), null, CairoFont.TextInput())
            )
            .Add(
                new MNGuiElementTextButton(capi, "Button", ElementBounds.FixedSize(100, 26)),
                "button-button"
            )
            .Add(
                new MNGuiElementInnerLayoutContainer(capi, BoundsStd.FitToChildren()),
                "container-sub"
            )
            .Add(
                insetContainerLayout
            )
            .Add(
                new HorizontalLayout(capi, hAlign: AlignmentHorizontal.Center)
                .Add(
                    new MNGuiElementTextButton(capi, "Push to clip", ElementBounds.FixedSize(100, 26)),
                    "button-pushtoclip"
                )
            )
            .Add(
                new HorizontalLayout(capi)
                    .WithAlignment(vAlign: AlignmentVertical.Middle)
                    .Add(
                        new HorizontalLayout(capi).Add(elementStd.TextAutoBoxSize("ABC"))
                            .WithAlignment(AlignmentHorizontal.Right, AlignmentVertical.Bottom)
                            .WithGuaranteedMinSize(width: 200)
                    )
                    .Add(
                        new VerticalLayout(capi).Add(elementStd.TextAutoBoxSize("ABC\nDEF\nGHQ"))
                            .WithContentClampedMaxSize(height: 10)
                            .WithVerticalSizePolicy(SizePolicy.MinSize)
                    )
                    .Add(
                        new HorizontalLayout(capi).Add(elementStd.TextAutoBoxSize("ABC"))
                            .WithAlignment(AlignmentHorizontal.Center, AlignmentVertical.Middle)
                            .WithFixedSize(100, 100)
                    )
            )
            .Add(
                CreateHorizontalLayoutAlignmentTest()
            )
            .Add(
                CreateVerticalLayoutAlignmentTest()
            );
        //layout.CustomMinWidth = 500;
        //layout.CustomMinHeight = 500;

        DialogBuilder.SetChildLayout(layout);
        posts.Clear();
        ClearComposers();
        SingleComposer = DialogBuilder.Layout(capi, this, "refactored-layout-test");
        var boundsHie = DebugUtil.GetBoundsTree(SingleComposer.Bounds);
        capi.Logger.Event(boundsHie);

        dialogController = new StandardDialogController(capi, SingleComposer, layout);

        var buttonButton = dialogController.GetElement<MNGuiElementTextButton>("button-button");
        buttonButton!.EventClicked = OnButtonClicked;
        var pushToClipButton = dialogController.GetElement<MNGuiElementTextButton>("button-pushtoclip");
        pushToClipButton!.EventClicked = OnPushToClip;
    }

    public override void OnGuiOpened() {
        // SetupDialog every time opened, for test
        SetupDialog();
        base.OnGuiOpened();
    }

    private LayoutBase CreateHorizontalLayoutAlignmentTest() {
        var eStd = new ElementStd(capi);
        var rootLayout = new HorizontalLayout(capi, 10);
        foreach (var hAlignment in Enum.GetValues<AlignmentHorizontal>()) {
            var columnLayout = new VerticalLayout(capi);
            columnLayout.Add(new MNGuiElementStaticText(capi, "(Spacer 300px)", ElementBounds.FixedSize(300, 20), backgroundColorRGBA: new(1.0, 1.0, 1.0, 0.2)));
            foreach (var vAlignment in Enum.GetValues<AlignmentVertical>()) {
                var cellLayout = new HorizontalLayout(capi, hAlign: hAlignment, vAlign: vAlignment)
                    .Add(eStd.TextAutoBoxSize("Test"))
                    .Add(new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 50), null, font: CairoFont.WhiteDetailText()))
                    .Add(new MNGuiElementTextButton(capi, "Push", ElementBounds.FixedSize(100, 25)));
                columnLayout.Add(cellLayout);
            }
            rootLayout.Add(columnLayout);
        }

        return rootLayout;
    }

    private LayoutBase CreateVerticalLayoutAlignmentTest() {
        var eStd = new ElementStd(capi);
        var rootLayout = new VerticalLayout(capi, 10);
        foreach (var vAlignment in Enum.GetValues<AlignmentVertical>()) {
            var columnLayout = new HorizontalLayout(capi, 2);
            columnLayout.Add(new MNGuiElementStaticText(capi, "Space", ElementBounds.FixedSize(50, 150), backgroundColorRGBA: new Vec4d(1.0, 1.0, 1.0, 0.2)));
            foreach (var hAlignment in Enum.GetValues<AlignmentHorizontal>()) {
                var cellLayout = new VerticalLayout(capi, hAlign: hAlignment, vAlign: vAlignment)
                    .WithGuaranteedMinSize(100)
                    .Add(eStd.TextAutoBoxSize("Test"))
                    .Add(new GuiElementTextArea(capi, ElementBounds.FixedSize(50, 100), null, font: CairoFont.WhiteDetailText()));
                columnLayout.Add(cellLayout);
            }
            rootLayout.Add(columnLayout);
        }

        return rootLayout;
    }

    void UpdateContainerLayoutWithPosts(MNGuiElementInnerLayoutContainer container) {
        var guiStd = new ElementStd(capi);
        var layout = new VerticalLayout(capi, 5);

        foreach (var post in posts) {
            layout.Add(guiStd.TextAutoBoxSize(post));
        }

        container.SetNewLayout(layout);
    }

    bool OnButtonClicked() {
        if (dialogController == null) return false;

        var layoutContainer = dialogController.GetElement<MNGuiElementInnerLayoutContainer>("container-sub");

        if (layoutContainer == null) {
            capi.Logger.Warning($"Couldn't find container");
            return false;
        }

        // Add post and update layout
        posts.Add("Foo!");

        UpdateContainerLayoutWithPosts(layoutContainer);

        //capi.Event.RegisterCallback(dt => {
        //    dialogController.OnBoundsUpdated();
        //},
        //0);

        return true;
    }

    bool OnPushToClip() {
        if (dialogController == null) return false;

        var layoutContainer = dialogController.GetElement<MNGuiElementInnerLayoutContainer>("container-insideclip");

        if (layoutContainer == null) {
            capi.Logger.Warning($"Couldn't find container");
            return false;
        }

        // Add post and update layout
        posts.Add("Foo!");

        UpdateContainerLayoutWithPosts(layoutContainer);

        return true;
    }

    /*
    // Prevent close on ESC
    public override bool OnEscapePressed() {
        return false;
    }
    */
}
