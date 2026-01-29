using System.Linq;
using System.Collections.Generic;
using MNGui;
using MNGui.GuiElements;
using Vintagestory.API.Client;
using MNGui.Extensions;
using System.Text;

namespace MNGuiTest.GUI;
public class GuiDialogDynamicTextTest : GuiDialogGeneric {
    public GuiDialogDynamicTextTest(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
    }

    public GuiComposer CreateCompoWithStandardLayout(int fixedHeight) {
        var dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
        dialogBounds.Name = "bounds-dialog";

        var bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
        bgBounds.Name = "bounds-bg";
        bgBounds.BothSizing = ElementSizing.FitToChildren;

        var clientAreaBounds = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight, 10, 10);
        clientAreaBounds.BothSizing = ElementSizing.FitToChildren;

        // DynamicText is simply unable to auto wrap properly. Use RichText, or just use it with one-line
        var dynText = """
            My
            Special
            Dynamic
            Text
            """;
        var elem1 = new GuiElementDynamicText(capi, dynText, CairoFont.WhiteSmallishText(), ElementBounds.FixedSize(900, 200));

        var composer = capi.Gui.CreateCompo(DialogTitle, dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar(DialogTitle, () => TryClose())
            .BeginChildElements(bgBounds) // Begin bgBounds child
                .BeginChildElements(clientAreaBounds)
                    .AddInteractiveElement(elem1, "dyntext-e1")
                .EndChildElements()
            //.AddInteractiveElement(new MNGuiElementContainer(capi, containerBounds), "scroll-content")
            .EndChildElements();

        // Scroll bar setting

        //var container = Composer.GetElement("scroll-content") as OldMNGuiElementContainer;

        //ChildLayout.Layout(container);

        //container.Bounds.CalcWorldBounds();
        //ChildLayout.BeforeComposerCompose();

        //var boundsHie = DebugUtil.GetBoundsTree(Composer.Bounds);
        //capi.Logger.Event(boundsHie);

        //Composer.Compose();

        //container.Bounds.CalcWorldBounds();

        //var mainScrollBar = Composer.GetScrollbar("scroll-bar");
        //mainScrollBar.SetHeights(scrollBarBounds.OuterHeightInt, (float)(containerBounds.OuterHeight + GuiStyle.HalfPadding * 2));
        //scrollBarContentFixedY = container.Bounds.fixedY;

        //return Composer;
        return composer;
    }

    public string GetDynText() {
        var sb = new StringBuilder();
        sb.AppendLine("My");
        sb.AppendLine("Special");
        for (int i = 0; i < 10; ++i) {
            sb.Append("0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
        }
        sb.AppendLine();
        sb.AppendLine("Dynamic");
        sb.AppendLine("Text");

        return sb.ToString();
    }

    public void SetupDialog() {
        SingleComposer = CreateCompoWithStandardLayout(400);

        SingleComposer.Compose();

        var dynTextElem = SingleComposer.GetElement<GuiElementDynamicText>("dyntext-e1");

        dynTextElem.SetNewText(GetDynText(), true, false, false);

        var boundsInfo = DebugUtil.GetBoundsTree(SingleComposer.Bounds);
        capi.Logger.Event(boundsInfo);
    }

    public override void OnGuiOpened() {
        // SetupDialog every time opened
        SetupDialog();
        base.OnGuiOpened();
    }

}
