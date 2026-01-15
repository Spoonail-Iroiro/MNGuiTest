using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace MNGUITest.BlockEntities;

public class BEItemTest : BlockEntity {
    public bool OnInteract(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel) {
        Api.Logger.Event($"Interact!");
        return true;
    }
}
