using HarmonyLib;
using MNGui.GuiElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace MNGuiTest;
public class GUIDebugModSystem : ModSystem {
    private ICoreClientAPI capi;

    public string CurrentText { get; set; } = "";

    public override void StartClientSide(ICoreClientAPI api) {
        this.capi = api;
    }

    public override void Dispose() {
        base.Dispose();
    }

    public IEnumerable<GuiElement> FindHoveredElements() {
        var mouseX = capi.Input.MouseX;
        var mouseY = capi.Input.MouseY;
        foreach (var dialog in capi.Gui.OpenedGuis) {
            if (dialog is GuiDialog guiDialog && guiDialog.IsOpened() && guiDialog.DialogType == EnumDialogType.Dialog) {

                foreach (var (_, composer) in guiDialog.Composers) {
                    var founds = FindElementsInSingleComposer(composer, mouseX, mouseY);
                    foreach (var found in founds) {
                        yield return found;
                    }
                }
            }
        }
    }

    private IEnumerable<GuiElement> FindElementsInSingleComposer(GuiComposer composer, int mouseX, int mouseY) {
        if (!composer.Bounds.PointInside(mouseX, mouseY)) yield break;

        var staticElements = Traverse.Create(composer).Field<Dictionary<string, GuiElement>>("staticElements").Value;
        var interactiveElements = Traverse.Create(composer).Field<Dictionary<string, GuiElement>>("interactiveElements").Value;

        if (staticElements != null) {
            foreach (var (_, element) in staticElements) {
                foreach (var insideElement in FindElementsInElement(element, mouseX, mouseY)) {
                    yield return insideElement;
                }
            }
        }

        // staticElements contains all interactiveElements
        //if (interactiveElements != null) {
        //    foreach (var (_, element) in interactiveElements) {
        //        foreach (var insideElement in FindElementsInElement(element, mouseX, mouseY)) {
        //            yield return insideElement;
        //        }
        //    }
        //}
    }

    private IEnumerable<GuiElement> FindElementsInElement(GuiElement target, int mouseX, int mouseY) {
        // returns myself
        if (target.Bounds.PointInside(mouseX, mouseY)) yield return target;

        if (target is GuiElementContainer container) {
            foreach (var elem in container.Elements) {
                foreach (var child in FindElementsInElement(elem, mouseX, mouseY)) {
                    yield return child;
                }
            }
        }

        if (target is MNGuiElementContainer mnContainer) {
            foreach (var elem in mnContainer.Elements) {
                foreach (var child in FindElementsInElement(elem, mouseX, mouseY)) {
                    yield return child;
                }
            }
        }
    }
}

//public class HudElementGUIDebug : HudElement {
//    private int mouseX;
//    private int mouseY;
//    private StringBuilder debugText = new StringBuilder();

//    public HudElementGUIDebug(ICoreClientAPI capi) : base(capi) {
//        // HUD要素を常に表示
//        this.Compose();
//    }

//    public void UpdateMousePosition(int x, int y) {
//        mouseX = x;
//        mouseY = y;
//    }

//    public override void OnRenderGUI(float deltaTime) {
//        base.OnRenderGUI(deltaTime);

//        // マウス位置にあるGUI要素を取得
//        GuiElement hoveredElement = FindHoveredElement();

//        if (hoveredElement != null) {
//            // デバッグ情報を構築
//            debugText.Clear();
//            debugText.AppendLine("=== GUI Element Debug Info ===");
//            debugText.AppendLine($"Type: {hoveredElement.GetType().Name}");

//            // Bounds情報
//            if (hoveredElement.Bounds != null) {
//                var bounds = hoveredElement.Bounds;
//                debugText.AppendLine($"\n[Bounds]");
//                debugText.AppendLine($"  absX: {bounds.absX:F1}");
//                debugText.AppendLine($"  absY: {bounds.absY:F1}");
//                debugText.AppendLine($"  absInnerWidth: {bounds.absInnerWidth:F1}");
//                debugText.AppendLine($"  absInnerHeight: {bounds.absInnerHeight:F1}");
//                debugText.AppendLine($"  OuterWidth: {bounds.OuterWidth:F1}");
//                debugText.AppendLine($"  OuterHeight: {bounds.OuterHeight:F1}");
//                debugText.AppendLine($"  InnerWidth: {bounds.InnerWidth:F1}");
//                debugText.AppendLine($"  InnerHeight: {bounds.InnerHeight:F1}");
//                debugText.AppendLine($"  fixedX: {bounds.fixedX:F1}");
//                debugText.AppendLine($"  fixedY: {bounds.fixedY:F1}");
//                debugText.AppendLine($"  fixedWidth: {bounds.fixedWidth:F1}");
//                debugText.AppendLine($"  fixedHeight: {bounds.fixedHeight:F1}");
//            }

//            // 追加情報
//            debugText.AppendLine($"\n[Additional Info]");
//            debugText.AppendLine($"  Enabled: {hoveredElement.Enabled}");
//            debugText.AppendLine($"  HasFocus: {hoveredElement.HasFocus}");

//            if (!string.IsNullOrEmpty(hoveredElement.TabName)) {
//                debugText.AppendLine($"  TabName: {hoveredElement.TabName}");
//            }

//            // テキストを画面左上に描画
//            double lineHeight = 20;
//            double y = 10;

//            capi.Render.RenderRectangle(5, 5, 400, debugText.ToString().Split('\n').Length * lineHeight + 10,
//                ColorUtil.ColorFromRgba(0, 0, 0, 200));

//            string[] lines = debugText.ToString().Split('\n');
//            foreach (string line in lines) {
//                if (!string.IsNullOrEmpty(line)) {
//                    capi.Render.RenderText(line, 10, y, ColorUtil.WhiteArgb);
//                }
//                y += lineHeight;
//            }
//        }
//    }


//    private bool IsMouseOver(GuiElement element, int x, int y) {
//        if (element?.Bounds == null) return false;

//        return x >= element.Bounds.absX &&
//               x <= element.Bounds.absX + element.Bounds.OuterWidth &&
//               y >= element.Bounds.absY &&
//               y <= element.Bounds.absY + element.Bounds.OuterHeight;
//    }

//    private void Compose() {
//        // この要素は描画のみなのでComposerは不要
//    }

//    public override string ToggleKeyCombinationCode => null;
//}
