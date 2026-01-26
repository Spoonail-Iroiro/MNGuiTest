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
            );

        RootLayout.SetChildLayout(layout);
        ClearComposers();
        SingleComposer = RootLayout.Layout(capi, this, "refactored-layout-test");
    }

    public override void OnGuiOpened() {
        // SetupDialog every time opened, for test
        SetupDialog();
        base.OnGuiOpened();
    }
}
