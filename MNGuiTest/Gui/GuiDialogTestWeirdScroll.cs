using System;
using MNGui.DialogBuilders;
using MNGui.GuiElements;
using MNGui.Layouts;
using MNGui.Layouts.Extensions;
using MNGui.Std;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

namespace MNGuiTest.Gui;

public class GuiDialogTestWeirdScroll : GuiDialogGeneric {
    ContainerDialogController? dialogController;

    int currentMode = 0;

    public GuiDialogTestWeirdScroll(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
    }

    public void SetupDialog() {
        var elementStd = new ElementStd(capi);

        var dialogBuilder = new ContainerDialogBuilder();

        var boxLayout = new InsetContainerLayoutBuilder(capi, "container-main")
            .Build();

        var mainLayout = new VerticalLayout(capi)
            .Add(boxLayout)
            .Add(
                new HorizontalLayout(capi, hAlign: AlignmentHorizontal.Right)
                .Add(
                    new MNGuiElementTextButton(capi, "Switch", ElementBounds.FixedSize(100, 25)),
                    "button-switch"
                )
            );

        dialogBuilder.SetChildLayout(mainLayout);

        ClearComposers();
        SingleComposer = dialogBuilder.Layout(capi, this, this.GetType().Name);

        dialogController = new ContainerDialogController(capi, SingleComposer, mainLayout);

        var button = dialogController.GetElement<MNGuiElementTextButton>("button-switch");
        button.EventClicked = OnSwitchButtonClicked;
    }

    bool OnSwitchButtonClicked() {
        currentMode = (currentMode + 1) % 3;
        var newLayout = GetLayoutBasedOnMode();
        var container = dialogController!.GetElement<MNGuiElementInnerLayoutContainer>("container-main");
        container.SetNewLayout(newLayout);

        return true;
    }

    LayoutWithElementBounds GetLayoutBasedOnMode() {
        var elementStd = new ElementStd(capi);
        if (currentMode == 0) {
            return new VerticalLayout(capi)
                .Add(elementStd.TextAutoBoxSize("small"));
        }
        else if (currentMode == 1) {
            var layout = new VerticalLayout(capi);
            for (var i = 0; i < 30; i++) {
                layout.Add(elementStd.TextAutoBoxSize("long"));
            }
            return layout;

        }
        else {
            var layout = new VerticalLayout(capi);
            for (var i = 0; i < 100; i++) {
                layout.Add(elementStd.TextAutoBoxSize("longer"));
            }
            return layout;
        }
    }

    public override void OnGuiOpened() {
        SetupDialog();
        base.OnGuiOpened();
    }
}
