using MNGui.Layouts;
using MNGui.DialogBuilders;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using MNGui.GuiElements;
using MNGui.Std;
using MNGui.GuiElements.Layout;

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

        //var layout = new HorizontalLayout(capi, 5)
        //    .Add(guiStd.TextAutoBoxSize("Foo"))
        //    .Add(guiStd.TextAutoBoxSize("Bar"))
        //    .Add(guiStd.TextAutoBoxSize("Buff"))
        //    .Add(
        //        new HorizontalLayout(capi, 10)
        //        .Add(guiStd.TextAutoBoxSize("1234"))
        //        .Add(guiStd.TextAutoBoxSize("234"))
        //    );

        var layout = new VerticalLayout(capi, 5)
            .Add(
                new HorizontalLayout(capi)
                .Add(guiStd.TextAutoBoxSize("Foo"))
                .Add(guiStd.TextAutoBoxSize("Bar"))
            )
            .Add(
                new HorizontalLayout(capi)
                .Add(guiStd.TextAutoBoxSize("123"))
                .Add(guiStd.TextAutoBoxSize("456"))
            )
            .Add(
                new HorizontalLayout(capi)
                .Add(new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 24), null, CairoFont.TextInput()))
                .Add(guiStd.TextAutoBoxSize("456"))
            )
            .Add(
                new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 24), null, CairoFont.TextInput())
            )
            .Add(
                new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 24), null, CairoFont.TextInput())
            )
            .Add(
                new GuiElementTextInput(capi, ElementBounds.FixedSize(100, 24), null, CairoFont.TextInput())
            )
            .Add(
                new GuiElementTextButton(capi, "Button", CairoFont.ButtonText(), CairoFont.ButtonText(), OnButtonClicked, ElementBounds.FixedSize(100, 26))
            )
            .Add(
                new MNGuiElementLayoutContainer(capi, BoundsStd.FitToChildren()),
                "container-sub"
            )
            .Add(
                CreateInsetContainer()
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
    }

    public override void OnGuiOpened() {
        // SetupDialog every time opened, for test
        SetupDialog();
        base.OnGuiOpened();
    }

    private LayoutBase CreateInsetContainer() {
        var elementStd = new ElementStd(capi);
        var contentLayout = new VerticalLayout(capi)
            .Add(
                new HorizontalLayout(capi).Add(elementStd.TextAutoBoxSize("123")).Add(elementStd.TextAutoBoxSize("456"))
            )
            .Add(
                new HorizontalLayout(capi).Add(elementStd.TextAutoBoxSize("ABCDEF")).Add(elementStd.TextAutoBoxSize("XYZ"))
            );


        var insetLayout = new SimpleWrapperLayout(new MNGuiElementInset(capi, BoundsStd.FitToChildren()), "inset-e1");
        var clipParentLayout = new SimpleWrapperLayout(new GuiElementDebugHorizontalLayout(capi, BoundsStd.FitToChildren().WithFixedPadding(5.0)));
        var containerLayout = new SingleLayout(new MNGuiElementLayoutContainer(capi, BoundsStd.FitToChildren(), initialLayout: contentLayout), "container-inset");

        insetLayout
            .SetChild(
                clipParentLayout
                    .SetChild(
                        containerLayout
                    )
            );

        return insetLayout;

    }

    void UpdateContainerLayoutWithPosts(MNGuiElementLayoutContainer container) {
        var guiStd = new ElementStd(capi);
        var layout = new VerticalLayout(capi, 5);

        foreach (var post in posts) {
            layout.Add(guiStd.TextAutoBoxSize(post));
        }

        container.SetNewLayout(layout);
    }

    bool OnButtonClicked() {
        if (dialogController == null) return false;

        var layoutContainer = dialogController.GetElement<MNGuiElementLayoutContainer>("container-sub");

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
}
