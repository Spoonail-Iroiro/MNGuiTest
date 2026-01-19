using MNGUITest.BlockEntities;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace MNGUITest.Blocks;
public class BlockClientCommand : BlockCommand {
    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel) {
        BlockEntityGuiConfigurableCommands bec = world.BlockAccessor.GetBlockEntity<BEClientCommand>(blockSel.Position);
        if (bec != null) {
            var caller = new Caller() { Player = byPlayer };
            return bec.OnInteract(caller);
        }
        else {
            api.Logger.Warning($"Couldn't find BlockEntity!");
            return false;
        }
    }

    public override void Activate(IWorldAccessor world, Caller caller, BlockSelection blockSel, ITreeAttribute activationArgs = null) {
        return;
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer) {
        WorldInteraction[] rtn = [
            new() {
                ActionLangCode = "Run commands",
                MouseButton = EnumMouseButton.Right
            },
            new() {
                ActionLangCode = "Edit (requires Creative mode)",
                HotKeyCode = "shift",
                MouseButton = EnumMouseButton.Right
            }
        ];

        return rtn;
    }
}
