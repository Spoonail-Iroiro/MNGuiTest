using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using RemoteTraderCheckMod.BlockEntities;
using Vintagestory.API.Server;
using Vintagestory.API.Config;

namespace MNGuiTest.BlockEntities;

public class BEItemTest : BlockEntity {
    public override void Initialize(ICoreAPI api) {
        base.Initialize(api);

        if (api.Side.IsServer()) {
            RegisterGameTickListener(OnServerGameTick, 900 + api.World.Rand.Next(200));
        }
    }

    void OnServerGameTick(float dt) {

    }


    public bool OnInteract(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel) {

        if (Api.Side.IsClient()) {
            return true;
        }

        if (byPlayer.Entity.Controls.ShiftKey) {
        }
        else {

        }

        return true;
    }
}
