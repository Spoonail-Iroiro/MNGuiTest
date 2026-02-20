using MNGui;
using MNGui.Layouts;
using MNGui.DialogBuilders;
using MNGuiTest.Patches;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using MNGui;
using Vintagestory.API.Util;
using MNGui.Std;
using MNGui.GuiElements;
using MNGui.Layouts.Extensions;

namespace MNGuiTest.Gui;
public class GuiDialogTest1 : GuiDialogGeneric {
    StandardDialogController? dialogController;

    public GuiDialogTest1(string DialogTitle, ICoreClientAPI capi) : base(DialogTitle, capi) {
        //SetupDialog();
    }

    public void SetupDialog() {
        var rootLayout = new StandardDialogBuilder();
        var guiStd = new ElementStd(capi);

        var body = GetBody(capi);

        rootLayout.SetChildLayout(body);
        SetupDialogWithRootLayout(rootLayout);

        dialogController = new(capi, SingleComposer, rootLayout.ChildLayout);

        var button = dialogController.GetElement<MNGuiElementTextButton>("btn-click");
        button.EventClicked = OnButtonClicked;
    }

    public static VerticalLayout GetBody(ICoreClientAPI capi) {
        var guiStd = new ElementStd(capi);
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
                () => {
                    var dyn = new GuiElementDynamicText(capi, "placeholder\nfoo\nbar", font: CairoFont.WhiteDetailText(), ElementBounds.FixedSize(300, GuiStyle.DetailFontSize * 1 + 4));
                    // TODO: processed in layout?
                    dyn.BeforeCalcBounds();
                    dyn.Bounds.CalcWorldBounds();
                    dyn.AutoHeight();
                    return dyn;
                },
                "txt-result"
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
                new HorizontalLayout(capi, hAlign: AlignmentHorizontal.Right)
                    .Add(
                        new HorizontalLayout(capi)
                        .Add(
                            new MNGuiElementTextButton(capi, "Button", ElementBounds.FixedSize(100, 24)),
                            "btn-click"
                        )
                    )
            ).Add(
                () => {
                    var sld = new GuiElementSlider(capi, null, ElementBounds.FixedSize(400, 26));
                    sld.SetValues(5, 0, 10, 1);
                    return sld;
                },
                "slider1"
            ).Add(
                () => { var elem = new GuiElementSwitch(capi, null, ElementBounds.FixedSize(26, 26)); elem.SetValue(false); return elem; },
                "switch1"
            ).Add(
                () => {
                    var elem = new GuiElementTextArea(
                        capi,
                        bounds: ElementBounds.FixedSize(400, 100),
                        OnTextChanged: null,
                        font: CairoFont.WhiteDetailText()
                    );
                    return elem;
                },
                "text2"
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
                                bounds: ElementBounds.FixedSize(150, 26),
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
                                bounds: ElementBounds.FixedSize(150, 26),
                                font: CairoFont.WhiteDetailText(),
                                true
                            );
                            return elem;
                        },
                        "dropdown-2"
                    )
            )
            .Add(
                new GuiElementToggleButton(capi, "", "Hoge", CairoFont.ButtonText(), OnToggled: null, ElementBounds.FixedSize(100, 26), true)
            )
            .Add(
                GetItemStackTestLayout(capi, Patcher1.previousStockInfo?.GetInventoryRemoteTrader(capi))
            );

        return body;
    }


    protected static LayoutBase GetItemStackTestLayout(
        ICoreClientAPI capi,
        InventoryTrader? inventory
    ) {
        var guiStd = new ElementStd(capi);
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

    protected void SetupDialogWithRootLayout(StandardDialogBuilder rootLayout) {
        ClearComposers();
        SingleComposer = rootLayout.Layout(capi, this, "test1");
        var foundBounds = DebugUtil.SearchBoundsRecursive(SingleComposer.Bounds, "richtext1");
    }

    public override void OnGuiOpened() {
        SetupDialog();
        base.OnGuiOpened();
    }

    public bool OnButtonClicked() {
        capi.Logger.Event("Button clicked!");
        var boundsTree = DebugUtil.GetBoundsTree(dialogController.Composer.Bounds);
        capi.Logger.Event(boundsTree);

        var dynText = dialogController.GetElement<GuiElementDynamicText>("txt-result");
        if (dynText != null) {
            var text = dynText.Text;
            text += "\nNewLine!";
            dynText.SetNewText(text, true, true);

            dialogController.OnBoundsUpdated();
        }
        else {
            capi.Logger.Warning($"txt-result not found!");
        }

        return true;
    }
}
