using ClientCommandBlockMod;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace MNGUITest.BlockEntities;
public class BEClientCommand : BlockEntityGuiConfigurableCommands {
    public override void Initialize(ICoreAPI api) {
        base.Initialize(api);
    }

    public override bool OnInteract(Caller caller) {
        if (Api.Side.IsServer()) return true;

        if (caller.Player?.Entity.Controls.ShiftKey == true) {
            // Open GUI
            base.OnInteract(caller);
            return true;
        }
        else {
            if (Api is ICoreClientAPI capi) {
                Api.Logger.Event($"Right clicked!");
                if (Commands == null) return false;

                string[] commands = Commands.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in commands) {
                    var cmd = line.Trim();
                    if (cmd == "") continue;
                    if (cmd.StartsWith(".")) {
                        capi.TriggerChatMessage(cmd);
                    }
                    else {
                        var lk = $"{ClientCommandBlockModModSystem.ModID}:ingameerror-notclientcommand";
                        capi.TriggerIngameError(this, "notclientcommand", Lang.Get(lk, cmd));
                    }
                }
            }

            return true;
        }
    }
}
