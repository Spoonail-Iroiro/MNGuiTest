using MNGUI.GUI.MNGui;
using MNGUI.Layouts;
using MNGUI.RootLayouts;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

namespace MNGUITest.GUI;
public class GuiDialogTest1 : GuiDialogGeneric {
    public GuiDialogTest1(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
        SetupDialog();
    }

    public void SetupDialog() {
        var rootLayout = new StandardRootLayout();

        var guiStd = new GuiStd(capi);

        var body = new VerticalLayout(capi)
            .Add(
                new HorizontalLayout(capi)
                    .Add(guiStd.TextAutoBoxSize("whiteDetail", font: CairoFont.WhiteDetailText()))
                    .Add(guiStd.TextAutoBoxSize("whiteSmall", font: CairoFont.WhiteSmallText()))
                    .Add(guiStd.TextAutoBoxSize("whiteSmallish", font: CairoFont.WhiteSmallishText()))
                    .Add(guiStd.TextAutoBoxSize("whiteMedium", font: CairoFont.WhiteMediumText()))
            )
            .Add(
                new HorizontalLayout(capi)
                    .Add(guiStd.TextAutoBoxSize("whiteDetailBold", font: CairoFont.WhiteDetailText().WithWeight(Cairo.FontWeight.Bold)))
                    .Add(guiStd.TextAutoBoxSize("whiteDetailItalic", font: CairoFont.WhiteDetailText().WithSlant(Cairo.FontSlant.Italic)))
                    .Add(guiStd.TextAutoBoxSize("whiteDetailOblique", font: CairoFont.WhiteDetailText().WithSlant(Cairo.FontSlant.Oblique)))
            )
            .Add(
                new HorizontalLayout(capi)
                    .Add(
                        guiStd.StandardRichText(
                            """
                            RichText <font size="20">might</font> be <strong>suitable</strong> for <font size="16">texts</font> with mixed font sizes.
                            But it requires fixed width. This might be tricky to layouting.
                            """,
                            400
                        ),
                        "richtext1"
                    )
            )
            .Add(
                new HorizontalLayout(capi)
                    .Add(guiStd.StandardRichText("<strong>Search Result</strong>", 300, font: CairoFont.WhiteSmallishText()))
            )
            .Add(
                new HorizontalLayout(capi)
                    .Add(
                        new GuiElementDynamicText(capi, "placeholder\nfoo\nbar", font: CairoFont.WhiteDetailText(), GuiStd.ElementBoundsWH(300, GuiStyle.DetailFontSize * 1 + 4)),
                        "txt-result"
                    )
            )
            .Add(
                new HorizontalLayout(capi, alignment: HorizontalLayoutAlignment.Right)
                    .Add(
                        new GuiElementTextButton(capi, "Button", CairoFont.ButtonText(), CairoFont.ButtonText(), () => true, GuiStd.ElementBoundsWH(100, 24)),
                        "btn-click"
                    )
                    .AddHorizontalSpace(10)
            )
            .Add(
                new HorizontalLayout(capi, 10)
                    .Add(
                        new HorizontalLayout(capi)
                            .Add(guiStd.TextAutoBoxSize("X"))
                            .Add(
                            new GuiElementTextInput(capi, ElementBounds.FixedSize(100, GuiStyle.SmallishFontSize), null, CairoFont.WhiteSmallishText()),
                            "txt-x"
                            )
                    )
                    .Add(
                        new HorizontalLayout(capi)
                            .Add(guiStd.TextAutoBoxSize("Y"))
                            .Add(
                            new GuiElementTextInput(capi, ElementBounds.FixedSize(100, GuiStyle.SmallishFontSize), null, CairoFont.WhiteSmallishText()),
                            "txt-y"
                            )
                    )
                    .Add(
                        new HorizontalLayout(capi)
                            .Add(guiStd.TextAutoBoxSize("Z"))
                            .Add(
                            new GuiElementTextInput(capi, ElementBounds.FixedSize(100, GuiStyle.SmallishFontSize), null, CairoFont.WhiteSmallishText()),
                            "txt-z"
                            )
                    )
            );

        rootLayout.SetChildLayout(body);
        var dyn = rootLayout.GetNamedElement<GuiElementDynamicText>("txt-result");
        // DynamicText neeeds CalcWorldBounds before AutoHeight()
        dyn.BeforeCalcBounds();
        dyn.Bounds.CalcWorldBounds();
        dyn.AutoHeight();
        SetupDialogWithRootLayout(rootLayout);
        rootLayout.ConnectToTitleBarClose(() => TryClose());
    }

    protected void SetupDialogWithRootLayout(StandardRootLayout rootLayout) {
        ClearComposers();
        SingleComposer = rootLayout.Layout(capi, this, "test1");
        var foundBounds = DebugUtil.SearchBoundsRecursive(SingleComposer.Bounds, "richtext1");
    }
}
