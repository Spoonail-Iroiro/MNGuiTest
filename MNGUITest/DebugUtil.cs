using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Vintagestory.API.Client;

namespace MNGUITest;
public class DebugUtil {
    public static ElementBounds? SearchBoundsRecursive(ElementBounds rootBounds, string searchingNamePartial) {
        if (rootBounds.Name?.Contains(searchingNamePartial) == true) return rootBounds;
        if (rootBounds.ChildBounds == null) return null;
        foreach (var bounds in rootBounds.ChildBounds) {
            var result = SearchBoundsRecursive(bounds, searchingNamePartial);
            if (result != null) return result;
        }
        return null;

    }

    public static string GetBoundsTree(ElementBounds bounds, StringBuilder? sb = null, int hie = 0, bool isInconsistentParent = false) {
        if (sb == null) {
            sb = new StringBuilder();
            // For avoid misreading logging
            sb.AppendLine("");
        }

        for (var i = 0; i < hie; ++i) {
            sb.Append("  ");
        }
        var noname = "(-)";
        var inconsistentMark = isInconsistentParent ? "[!] " : "";

        sb.AppendLine($"- {inconsistentMark}{bounds.Name ?? noname} ({bounds.fixedX},{bounds.fixedY},{bounds.fixedWidth},{bounds.fixedHeight})");

        foreach (var bd in bounds.ChildBounds ?? []) {
            var isWeirdParent = bd.ParentBounds != bounds;
            GetBoundsTree(bd, sb, hie + 1, isWeirdParent);
        }

        return sb.ToString();

    }
}
