using HarmonyLib;
using RemoteTraderCheckMod.GUI;
using RemoteTraderCheckMod.Core;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;

namespace MNGuiTest.Patches;

[HarmonyPatch(typeof(GuiDialogTraderStockCheck), nameof(GuiDialogTraderStockCheck.ShowStockDialog))]
public class Patcher1 {
    public static TraderStockInfo? previousStockInfo;

    public static void Prefix(TraderStockInfo traderStockInfo) {
        previousStockInfo = traderStockInfo;
    }
}


public class GuiScreenConnectingServerLogAddedPatcher {
    public static void Prefix(string message, string ___prevText, GuiComposer ___ElementComposer) {
        GuiElementDynamicText textElem = ___ElementComposer.GetDynamicText("centertext");
        var mes = message;
    }
}
