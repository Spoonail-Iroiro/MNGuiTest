using HarmonyLib;
using RemoteTraderCheckMod.GUI;
using RemoteTraderCheckMod.Core;
using System.Collections.Generic;
using System.Linq;

namespace MNGUITest.Patches;

[HarmonyPatch(typeof(GuiDialogTraderStockCheck), nameof(GuiDialogTraderStockCheck.ShowStockDialog))]
public class Patcher1 {
    public static TraderStockInfo? previousStockInfo;

    public static void Prefix(TraderStockInfo traderStockInfo) {
        previousStockInfo = traderStockInfo;
    }
}
