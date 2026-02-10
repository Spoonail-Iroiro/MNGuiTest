using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace GUIDebugMod {
    //public class GUIDebugModSystem : ModSystem
    //{
    //    private ICoreClientAPI capi;
    //    private HudElementGUIDebug hudElement;

    //    public override void StartClientSide(ICoreClientAPI api)
    //    {
    //        base.StartClientSide(api);
    //        capi = api;

    //        // HUD要素を登録
    //        hudElement = new HudElementGUIDebug(capi);
    //        capi.Gui.RegisterDialog(hudElement);

    //        // マウス移動イベントを登録
    //        capi.Event.MouseMove += OnMouseMove;
    //    }

    //    private void OnMouseMove(MouseEvent e)
    //    {
    //        if (hudElement != null)
    //        {
    //            hudElement.UpdateMousePosition(e.X, e.Y);
    //        }
    //    }

    //    public override void Dispose()
    //    {
    //        if (capi != null)
    //        {
    //            capi.Event.MouseMove -= OnMouseMove;
    //        }
    //        base.Dispose();
    //    }
    //}

    //public class HudElementGUIDebug : HudElement
    //{
    //    private int mouseX;
    //    private int mouseY;
    //    private StringBuilder debugText = new StringBuilder();

    //    public HudElementGUIDebug(ICoreClientAPI capi) : base(capi)
    //    {
    //        // HUD要素を常に表示
    //        this.Compose();
    //    }

    //    public void UpdateMousePosition(int x, int y)
    //    {
    //        mouseX = x;
    //        mouseY = y;
    //    }

    //    public override void OnRenderGUI(float deltaTime)
    //    {
    //        base.OnRenderGUI(deltaTime);

    //        // マウス位置にあるGUI要素を取得
    //        GuiElement hoveredElement = FindHoveredElement();

    //        if (hoveredElement != null)
    //        {
    //            // デバッグ情報を構築
    //            debugText.Clear();
    //            debugText.AppendLine("=== GUI Element Debug Info ===");
    //            debugText.AppendLine($"Type: {hoveredElement.GetType().Name}");

    //            // Bounds情報
    //            if (hoveredElement.Bounds != null)
    //            {
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

    //            if (!string.IsNullOrEmpty(hoveredElement.TabName))
    //            {
    //                debugText.AppendLine($"  TabName: {hoveredElement.TabName}");
    //            }

    //            // テキストを画面左上に描画
    //            double lineHeight = 20;
    //            double y = 10;

    //            capi.Render.RenderRectangle(5, 5, 400, debugText.ToString().Split('\n').Length * lineHeight + 10, 
    //                ColorUtil.ColorFromRgba(0, 0, 0, 200));

    //            string[] lines = debugText.ToString().Split('\n');
    //            foreach (string line in lines)
    //            {
    //                if (!string.IsNullOrEmpty(line))
    //                {
    //                    capi.Render.RenderText(line, 10, y, ColorUtil.WhiteArgb);
    //                }
    //                y += lineHeight;
    //            }
    //        }
    //    }

    //    private GuiElement FindHoveredElement()
    //    {
    //        // 開いているダイアログを走査
    //        foreach (var dialog in capi.Gui.OpenedGuis)
    //        {
    //            if (dialog is GuiDialog guiDialog && guiDialog.IsOpened())
    //            {
    //                // ダイアログ内の要素を再帰的に検索
    //                GuiElement found = FindElementRecursive(guiDialog.SingleComposer?.GetElement(""), mouseX, mouseY);
    //                if (found != null)
    //                {
    //                    return found;
    //                }

    //                // Composerがnullでない場合、全要素を検索
    //                if (guiDialog.SingleComposer != null)
    //                {
    //                    foreach (var element in guiDialog.SingleComposer)
    //                    {
    //                        if (IsMouseOver(element, mouseX, mouseY))
    //                        {
    //                            return element;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        return null;
    //    }

    //    private GuiElement FindElementRecursive(GuiElement element, int x, int y)
    //    {
    //        if (element == null) return null;

    //        // GuiComposerの場合は子要素を探す
    //        if (element is GuiComposer composer)
    //        {
    //            foreach (var child in composer.interactiveElements)
    //            {
    //                if (IsMouseOver(child, x, y))
    //                {
    //                    return child;
    //                }
    //            }
    //        }

    //        return null;
    //    }

    //    private bool IsMouseOver(GuiElement element, int x, int y)
    //    {
    //        if (element?.Bounds == null) return false;

    //        return x >= element.Bounds.absX && 
    //               x <= element.Bounds.absX + element.Bounds.OuterWidth &&
    //               y >= element.Bounds.absY && 
    //               y <= element.Bounds.absY + element.Bounds.OuterHeight;
    //    }

    //    private void Compose()
    //    {
    //        // この要素は描画のみなのでComposerは不要
    //    }

    //    public override string ToggleKeyCombinationCode => null;
    //}
}
