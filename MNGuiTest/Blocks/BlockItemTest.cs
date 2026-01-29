using MNGuiTest.BlockEntities;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace MNGuiTest.Blocks;

public class BlockItemTest : Block {
    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel) {
        var be = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BEItemTest;

        if (be != null) {
            return be.OnInteract(world, byPlayer, blockSel);
        }
        else {
            api.Logger.Warning($"Couldn't find BlockEntity!");
        }

        return base.OnBlockInteractStart(world, byPlayer, blockSel);

    }
}
