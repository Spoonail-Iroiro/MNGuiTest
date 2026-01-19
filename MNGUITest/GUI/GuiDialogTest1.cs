using MNGUI.GUI.MNGui;
using MNGUI.Layouts;
using MNGUI.RootLayouts;
using MNGUITest.Patches;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace MNGUITest.GUI;
public class GuiDialogTest1 : GuiDialogGeneric {
    public GuiDialogTest1(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
        SetupDialog();
    }

    public void SetupDialog() {
        var rootLayout = new StandardRootLayout();

        var guiStd = new GuiStd(capi);

        var body = new VerticalLayout(capi, 5)
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
                        () => {
                            var dyn = new GuiElementDynamicText(capi, "placeholder\nfoo\nbar", font: CairoFont.WhiteDetailText(), GuiStd.ElementBoundsWH(300, GuiStyle.DetailFontSize * 1 + 4));
                            // TODO: processed in layout?
                            dyn.BeforeCalcBounds();
                            dyn.Bounds.CalcWorldBounds();
                            dyn.AutoHeight();
                            return dyn;
                        },
                        "txt-result"
                    )
            )
            .Add(
                new HorizontalLayout(capi, 10)
                    .Add(
                        new HorizontalLayout(capi)
                            .Add(guiStd.TextAutoBoxSize("X"))
                            .Add(
                            new GuiElementTextInput(capi, ElementBounds.FixedSize(100, GuiStyle.SmallishFontSize), null, CairoFont.WhiteDetailText()),
                            "txt-x"
                            )
                    )
                    .Add(
                        new HorizontalLayout(capi)
                            .Add(guiStd.TextAutoBoxSize("Y"))
                            .Add(
                            new GuiElementTextInput(capi, ElementBounds.FixedSize(100, GuiStyle.SmallishFontSize), null, CairoFont.WhiteDetailText()),
                            "txt-y"
                            )
                    )
                    .Add(
                        new HorizontalLayout(capi)
                            .Add(guiStd.TextAutoBoxSize("Z"))
                            .Add(
                            new GuiElementTextInput(capi, ElementBounds.FixedSize(100, GuiStyle.SmallishFontSize), null, CairoFont.WhiteDetailText()),
                            "txt-z"
                            )
                    )
            )
            .Add(
                // Unreadable way to add right aligned multiple elements
                new HorizontalLayout(capi, alignment: HorizontalLayoutAlignment.Right)
                    .Add(
                        new HorizontalLayout(capi)
                        .Add(
                            new GuiElementTextButton(capi, "Button", CairoFont.ButtonText(), CairoFont.ButtonText(), () => true, GuiStd.ElementBoundsWH(100, 24)),
                            "btn-click"
                        )
                        .AddHorizontalSpace(20)
                    )
            ).Add(
                () => {
                    var sld = new GuiElementSlider(capi, null, ElementBounds.FixedSize(400, 26));
                    sld.SetValues(5, 0, 10, 1);
                    return sld;
                },
                "slider-1"
            ).Add(
                () => { var elem = new GuiElementSwitch(capi, null, ElementBounds.FixedSize(26, 26)); elem.SetValue(false); return elem; },
                "switch-1"
            )
            .Add(
                new HorizontalLayout(capi)
                    .Add(
                        () => {
                            var elem = new GuiElementDropDown(
                                capi,
                                ["val1", "val2"],
                                ["name1", "name3"],
                                0,
                                null,
                                bounds: ElementBounds.FixedSize(26, 26),
                                font: CairoFont.WhiteDetailText(),
                                false
                            );
                            return elem;
                        },
                        "dropdown-1"
                    )
                    .Add(
                        () => {
                            var elem = new GuiElementDropDown(
                                capi,
                                ["val1", "val2"],
                                ["name1", "name2"],
                                0,
                                null,
                                bounds: ElementBounds.FixedSize(26, 26),
                                font: CairoFont.WhiteDetailText(),
                                true
                            );
                            return elem;
                        },
                        "dropdown0"
                    )
            )
            .Add(
                GetItemStackTestLayout(capi, Patcher1.previousStockInfo?.GetInventoryRemoteTrader(capi))
            );

        rootLayout.SetChildLayout(body);
        SetupDialogWithRootLayout(rootLayout);
        rootLayout.ConnectToTitleBarClose(() => TryClose());
    }

    protected LayoutBase GetItemStackTestLayout(
        ICoreClientAPI capi,
        InventoryTrader? inventory
    ) {
        var guiStd = new GuiStd(capi);
        if (inventory == null) {
            return new HorizontalLayout(capi).Add(guiStd.TextAutoBoxSize("No inventory to show"));
        }

        var isSelling = false;
        var maxN = 10;

        var layouts = new List<LayoutBase>();

        double pad = GuiElementItemSlotGridBase.unscaledSlotPadding;
        double slotOneSideLength = GuiElementItemSlotGridBase.unscaledSlotPadding + GuiElementPassiveItemSlot.unscaledSlotSize;
        var sortedSlots = (isSelling ? inventory.SellingSlots : inventory.BuyingSlots)
            .Select((slot, idx) => (slot, i: (isSelling ? 0 : 20) + idx))
            .OrderBy(slotI => slotI.slot.GetStackName());
        var i = 0;
        foreach (var slotI in sortedSlots) {
            if (slotI.slot.Empty) continue;
            if (i >= maxN) break;
            var slotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, pad, pad, 1, 1);
            var slotGrid = new GuiElementItemSlotGrid(capi, inventory, (obj) => { }, 1, new int[] { slotI.i }, slotBounds);
            var texts = new VerticalLayout(capi)
                .Add(guiStd.TextAutoBoxSize(slotI.slot.GetStackName(), font: CairoFont.WhiteSmallishText()))
                .Add(guiStd.TextAutoBoxSize($"x{slotI.slot.StackSize}", font: CairoFont.WhiteSmallText()));
            layouts.Add(
                new HorizontalLayout(capi)
                .Add(
                    slotGrid
                )
                .Add(
                    texts
                )
                .AddHorizontalSpace(20)
            );

            ++i;
        }

        var rtnLayout = new VerticalLayout(capi, 5);

        foreach (var lay in layouts) {
            rtnLayout.Add(lay);
        }

        return rtnLayout;
    }

    protected void SetupDialogWithRootLayout(StandardRootLayout rootLayout) {
        ClearComposers();
        SingleComposer = rootLayout.Layout(capi, this, "test1");
        var foundBounds = DebugUtil.SearchBoundsRecursive(SingleComposer.Bounds, "richtext1");
    }
}
