using Vintagestory.API.Client;
using MNGui.DialogBuilders;
using MNGui.GuiElements;
using MNGui.Layouts;
using MNGui.Std;
using System.Security.Cryptography;

namespace MNGuiTest.Gui;
public class GuiDialogTest1WithContainer : GuiDialogGeneric {
    ContainerDialogController? controller;

    public GuiDialogTest1WithContainer(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
    }

    public void SetupDialog() {
        var test1Layout = GuiDialogTest1.GetBody(capi);

        var layoutBuilder = new InsetContainerLayoutBuilder(capi, "container-inside")
            .WithSizeFitToChildren(BoxSide.Horizontal)
            //.WithFitToChildrenRange(BoxSide.Vertical, 450)
            .WithSizeFixed(BoxSide.Vertical, 400)
            .WithInitialLayout(test1Layout);

        var dialogBuilder = new ContainerDialogBuilder();

        var layout = layoutBuilder.Build();
        dialogBuilder.SetChildLayout(layout);

        ClearComposers();
        SingleComposer = dialogBuilder.Layout(capi, this, this.GetType().Name);

        controller = new(capi, SingleComposer, layout);

        var button = controller.GetElement<MNGuiElementTextButton>("btn-click");
        button.EventClicked = OnButtonClicked;
    }

    bool OnButtonClicked() {
        var dynText = controller?.GetElement<GuiElementDynamicText>("txt-result");
        if (dynText != null) {
            var text = dynText.Text;
            text += "\nNewLine!";
            dynText.SetNewText(text, true, true);

            controller.OnBoundsUpdated();
        }
        else {
            capi.Logger.Warning($"txt-result not found!");
        }

        return true;
    }

    public override void OnGuiOpened() {
        SetupDialog();
        base.OnGuiOpened();
    }

}
