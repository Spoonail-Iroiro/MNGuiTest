using MNGUITest.BlockEntities;
using MNGUITest.Blocks;
using MNGUITest.GUI;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace MNGUITest {
    public class MNGUITestModSystem : ModSystem {
        GuiDialogTest1? test1Dialog;

        public static string ModID { get; private set; } = "";

        static void RegisterBlockClass<T>(ICoreAPI api) {
            api.RegisterBlockClass(ModID + "." + typeof(T).Name, typeof(T));
        }

        static void RegisterBlockEntityClass<T>(ICoreAPI api) {
            api.RegisterBlockEntityClass(ModID + "." + typeof(T).Name, typeof(T));
        }

        public override void StartPre(ICoreAPI api) {
            ModID = Mod.Info.ModID;
        }

        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void Start(ICoreAPI api) {
            RegisterBlockClass<BlockItemTest>(api);
            RegisterBlockEntityClass<BEItemTest>(api);
        }

        public override void StartServerSide(ICoreServerAPI api) {
        }

        public override void StartClientSide(ICoreClientAPI api) {
            var rootCommand = api.ChatCommands
                .Create("mngui-test")
                .RequiresPrivilege(Privilege.chat)
                .RequiresPlayer();

            var subCommand1 = rootCommand
                .BeginSubCommand("test1")
                .HandleWith(args => {
                    if (test1Dialog == null) {
                        test1Dialog = new GuiDialogTest1("Test1", api);
                    }
                    if (!test1Dialog.IsOpened()) {
                        test1Dialog.SetupDialog();
                        test1Dialog.TryOpen();
                    }

                    return TextCommandResult.Success("");
                });

            GuiDialogTest2? test2Dialog = null;

            var subCommand2 = rootCommand
                .BeginSubCommand("test2")
                .HandleWith(args => {
                    if (test2Dialog == null) {
                        test2Dialog = new(api);
                    }

                    if (!test2Dialog.IsOpened()) {
                        test2Dialog.TryOpen();
                    }
                    return TextCommandResult.Success("");
                });
        }

    }
}
