using MNGUI.GUI.MNGui;
using MNGUI.Layouts;
using MNGUI.RootLayouts;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

namespace MNGUITest.GUI;
public class GuiDialogRefactoredLayoutTest : GuiDialogGeneric {
    public StandardRootLayout? RootLayout { get; set; }

    public GuiDialogRefactoredLayoutTest(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
    }

    public void SetupDialog() {
        RootLayout = new StandardRootLayout();
        var guiStd = new GuiStd(capi);

        var layout = new VerticalLayout(capi)
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
            );

        RootLayout.SetChildLayout(layout);
        ClearComposers();
        SingleComposer = RootLayout.Layout(capi, this, "refactored-layout-test");

        var boundsHie = DebugUtil.GetBoundsTree(SingleComposer.Bounds);
        capi.Logger.Event(boundsHie);
    }

    public override void OnGuiOpened() {
        // SetupDialog every time opened, for test
        SetupDialog();
        base.OnGuiOpened();
    }
}
