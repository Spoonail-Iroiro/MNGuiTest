using MNGUI.Layouts;
using MNGUI.DialogBuilders;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace MNGUITest.GUI;
public class GuiDialogBETest3 : GuiDialogBlockEntity {
    public GuiDialogBETest3(string dialogTitle, BlockPos blockEntityPos, ICoreClientAPI capi) : base(dialogTitle, blockEntityPos, capi) {
    }

    public void SetupDialog() {
        //var rootLayout = new StandardRootLayout();

        //var mainLayout = new VerticalLayout(capi)
        //    .Add()

    }
}
