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
                GetDefaultLayout()
            )
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
        contents.Clear();
        SingleComposer = dialogBuilder.Layout(capi, this, this.GetType().Name);

        controller = new(capi, SingleComposer, layout);

        string[] sampleIDs = ["default", "allfixed", "allfittochildren", "verticalfittochildrenrange", "verticalfixedwithoutclip"];

        foreach (var sampleID in sampleIDs) {
            var button = controller.GetElement<MNGuiElementTextButton>("button-add-" + sampleID);
            button.EventClicked = () => { AddAndUpdate("container-" + sampleID, sampleID); return true; };
        }
    }

    LayoutBase GetDefaultLayout() {
        var sampleID = "default";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID);
        return GetSampleLayout("Default", layoutBuilder, sampleID);
    }

    LayoutBase GetAllFixedLayout() {
        var sampleID = "allfixed";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID)
            .WithSizeFixed(BoxSide.Horizontal, 100)
            .WithSizeFixed(BoxSide.Vertical, 100);
        return GetSampleLayout("Fixed 100 both on horizontal/vertical", layoutBuilder, sampleID);
    }

    LayoutBase GetAllFitToChildrenLayout() {
        var sampleID = "allfittochildren";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID)
            .WithSizeFitToChildren(BoxSide.Horizontal)
            .WithSizeFitToChildren(BoxSide.Vertical)
            .WithScrollbar(false)
            .WithClip(false);

        return GetSampleLayout("FitToChildren both on horizontal/vertical without clip", layoutBuilder, sampleID);
    }

    LayoutBase GetVerticalFitToChildrenRangeLayout() {
        var sampleID = "verticalfittochildrenrange";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID)
            .WithSizeFitToChildrenRange(BoxSide.Vertical, 150);

        return GetSampleLayout("FitToChildrenRange 150 on vertical", layoutBuilder, sampleID);
    }

    LayoutBase GetVerticalFixedWithoutClipLayout() {
        var sampleID = "verticalfixedwithoutclip";
        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-" + sampleID)
            .WithSizeFitToChildren(BoxSide.Horizontal)
            .WithSizeFixed(BoxSide.Vertical, 200)
            .WithScrollbar(false)
            .WithClip(false);
        return GetSampleLayout("Fixed on vertical without clip (not recommended)", layoutBuilder, sampleID);
    }

    LayoutBase GetSampleLayout(string description, InsetContainerLayoutBuilder layoutBuilder, string sampleID) {
        var elementStd = new ElementStd(capi);
        var topInsetLayout = layoutBuilder.Build();

        var layout = new VerticalLayout(capi, 10);
        if (description != "") {
            layout.Add(
                    new SimpleWrapperLayout(
                        new MNGuiElementStaticCustomDraw(
                            capi,
                            ElementBounds.FixedSize(10, 10).WithSizing(ElementSizing.FitToChildren).WithFixedPadding(5),
                            (ctx, surface, bounds) => {
                                ctx.SetSourceRGBA(1.0, 1.0, 1.0, 0.1);
                                GuiElement.RoundRectangle(ctx, bounds.bgDrawX, bounds.bgDrawY, bounds.OuterWidth, bounds.OuterHeight, 1.0);
                                ctx.Fill();
                            }
                    ))
                        .WithHorizontalSizePolicy(SizePolicy.Stretch)
                        .Add(
                            new HorizontalLayout(capi)
                            .Add(
                                () => {
                                    var text = new GuiElementDynamicText(capi, description, CairoFont.WhiteDetailText(), ElementBounds.FixedSize(200, 10));
                                    text.Bounds.CalcWorldBounds();
                                    text.AutoHeight();
                                    return text;
                                }
                            )
                        )

                );
        }

        layout
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
