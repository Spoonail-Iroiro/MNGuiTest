using HarmonyLib;
using MNGUITest.BlockEntities;
using MNGUITest.Blocks;
using MNGUITest.GUI;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace ClientCommandBlockMod;

public class ClientCommandBlockModModSystem : ModSystem {
    public static string ModID = "clientcommandblockmod";

    public override void Start(ICoreAPI api) {
        api.RegisterBlockClass($"{ModID}.{typeof(BlockClientCommand).Name}", typeof(BlockClientCommand));
        api.RegisterBlockEntityClass($"{ModID}.{typeof(BEClientCommand).Name}", typeof(BEClientCommand));
    }
}
