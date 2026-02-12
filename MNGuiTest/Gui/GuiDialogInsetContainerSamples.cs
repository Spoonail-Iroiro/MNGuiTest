using MNGui.DialogBuilders;
using MNGui.GuiElements;
using MNGui.Layouts;
using MNGui.Std;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

namespace MNGuiTest.Gui;
public class GuiDialogInsetContainerSamples : GuiDialogGeneric {
    ContainerDialogController? controller;

    public GuiDialogInsetContainerSamples(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
    }

    public void SetupDialog() {
        var elementStd = new ElementStd(capi);

        var rowLayout1 = new HorizontalLayout(capi, 10)
            .Add(
                GetAllFixedLayout()
            )
            .Add(
                GetAllFitToChildrenLayout()
            )
            .Add(
                GetVerticalFitToChildrenRangeLayout()
            );

        var rowLayout2 = new HorizontalLayout(capi, 10)
            .Add(
                GetVerticalFixedWithoutClipLayout()
            );

        var layout = new VerticalLayout(capi, 10)
            .Add(
                rowLayout1
            )
            .Add(
                rowLayout2
            );

        var dialogBuilder = new ContainerDialogBuilder();
        dialogBuilder.SetChildLayout(layout);

        ClearComposers();
        SingleComposer = dialogBuilder.Layout(capi, this, this.GetType().Name);

        controller = new(capi, SingleComposer, layout);

        string[] sampleIDs = ["allfixed", "allfittochildren", "verticalfittochildrenrange", "verticalfixedwithoutclip"];

        foreach (var sampleID in sampleIDs) {
            var button = controller.GetElement<MNGuiElementTextButton>("button-add-" + sampleID);
            button.EventClicked = () => { AddAndUpdate("container-" + sampleID, sampleID); return true; };
        }
    }

    LayoutBase GetAllFixedLayout() {
        var sampleID = "allfixed";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID)
            .WithFixed(BoxSide.Horizontal, 100)
            .WithFixed(BoxSide.Vertical, 100);
        return GetSampleLayout(layoutBuilder, sampleID);
    }

    LayoutBase GetAllFitToChildrenLayout() {
        var sampleID = "allfittochildren";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID)
            .WithFitToChildren(BoxSide.Horizontal)
            .WithFitToChildren(BoxSide.Vertical)
            .WithScrollbar(false)
            .WithClip(false);

        return GetSampleLayout(layoutBuilder, sampleID);
    }

    LayoutBase GetVerticalFitToChildrenRangeLayout() {
        var sampleID = "verticalfittochildrenrange";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID)
            .WithFitToChildren(BoxSide.Horizontal)
            .WithFitToChildrenRange(BoxSide.Vertical, 150);

        return GetSampleLayout(layoutBuilder, sampleID);
    }

    LayoutBase GetVerticalFixedWithoutClipLayout() {
        var sampleID = "verticalfixedwithoutclip";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID)
            .WithFitToChildren(BoxSide.Horizontal)
            .WithFixed(BoxSide.Vertical, 200)
            .WithScrollbar(false)
            .WithClip(false);
        return GetSampleLayout(layoutBuilder, sampleID);
    }

    LayoutBase GetSampleLayout(InsetContainerLayoutBuilder layoutBuilder, string sampleID) {
        var elementStd = new ElementStd(capi);
        var topInsetLayout = layoutBuilder.Build();

        var layout = new VerticalLayout(capi, 10)
            .Add(
                topInsetLayout
            )
            .Add(
                new HorizontalLayout(capi)
                    .Add(
                        new MNGuiElementTextButton(capi, "Add", ElementBounds.FixedSize(100, 26)),
                        "button-add-" + sampleID
                    )
            );

        return layout;
    }

    Dictionary<string, string> contents = new();

    void AddAndUpdate(string containerName, string addContent) {
        var elementStd = new ElementStd(capi);
        var container = controller.GetElement<MNGuiElementLayoutContainer>(containerName);
        if (container == null) {
            capi.Logger.Warning($"Couldn't find {containerName}");
            return;
        }

        if (!contents.ContainsKey(containerName)) contents[containerName] = "";
        contents[containerName] += $"{addContent}\n";

        var newLayout = new SingleLayout(elementStd.TextAutoBoxSize(contents[containerName]));

        container.SetNewLayout(newLayout);

        var boundsTree = DebugUtil.GetBoundsTree(controller.Composer.Bounds);
        capi.Logger.Event(boundsTree);
    }

    public override void OnGuiOpened() {
        SetupDialog();
        base.OnGuiOpened();
    }
}
