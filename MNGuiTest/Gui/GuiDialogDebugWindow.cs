using MNGui.DialogBuilders;
using MNGui.GuiElements;
using MNGui.Layouts;
using MNGui.Std;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

namespace MNGuiTest.Gui;
public class GuiDialogDebugWindow : GuiDialogGeneric {
    ContainerDialogController controller;

    public GuiDialogDebugWindow(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
    }

    public void SetupDialog() {
        var elementStd = new ElementStd(capi);
        var builder = new ContainerDialogBuilder()
            .WithStandardClose(false);

        var topInsetLayoutBuilder = new InsetContainerLayoutBuilder(capi, "container-top")
            .WithFixed(BoxSide.Horizontal, 100)
            .WithFixed(BoxSide.Vertical, 100)
            .WithInitialLayout(new HorizontalLayout(capi).Add(elementStd.TextAutoBoxSize("ABC")).Add(elementStd.TextAutoBoxSize("123")));
        var topInsetLayout = topInsetLayoutBuilder.Build();

        var layout = new VerticalLayout(capi, 10)
            .Add(
                topInsetLayout
            )
            .Add(
                new HorizontalLayout(capi, 50)
                    .Add(
                        new MNGuiElementTextButton(capi, "Close", ElementBounds.FixedSize(100, 26)),
                        "button-close"
                    )
            );

        builder.SetChildLayout(layout);

        ClearComposers();
        SingleComposer = builder.Layout(capi, this, nameof(GuiDialogDebugWindow));

        controller = new(capi, SingleComposer, layout);

        var closeButton = controller.GetElement<MNGuiElementTextButton>("button-close");
        closeButton.EventClicked = () => TryClose();
    }

    public override void OnGuiOpened() {
        SetupDialog();
        base.OnGuiOpened();
    }

    public override bool OnEscapePressed() {
        return false;
    }

    public override bool DisableMouseGrab => true;
}
