using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Vintagestory.API.Client;

namespace MNGuiTest.Patches;

[HarmonyPatch(typeof(ElementBounds), "buildBoundsFromChildren")]
public class Patcher2 {
    public static Exception? Finalizer(ElementBounds __instance, Exception __exception) {
        if (__exception == null) return null;
        if (__exception is ElementBoundsLayoutException) return null;

        return new ElementBoundsLayoutException(
            $"Failed to build ElementBounds from children: {__exception.Message}",
            __instance,
            __exception
        );
    }
}

public class ElementBoundsLayoutException : Exception {
    public ElementBounds Bounds { get; }

    public GuiElement? GuiElement { get; }

    public ElementBoundsLayoutException(
        string message,
        ElementBounds bounds,
        Exception inner
    ) : base(BuildMessage(message, bounds), inner) {
        Bounds = bounds;
    }

    private static string BuildMessage(string message, ElementBounds b) {
        return
$"""
{message}

Bounds:
  Name: {b.Name}
  Sizing: H={b.horizontalSizing}, V={b.verticalSizing}
  FixedSize: W={b.fixedWidth}, H={b.fixedHeight}
  RelPos: X={b.relX}, Y={b.relY}
  Children: {b.ChildBounds?.Count ?? 0}
""";
    }
}
