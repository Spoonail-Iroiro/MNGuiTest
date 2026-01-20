using HarmonyLib;
using MNGUITest.BlockEntities;
using MNGUITest.Blocks;
using MNGUITest.GUI;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace MNGUITest {
    public class MNGUITestModSystem : ModSystem {
        GuiDialogTest1? test1Dialog;

        public static string ModID { get; private set; } = "";

        public Harmony Harmony => new Harmony(Mod.Info.ModID);

        static void RegisterBlockClass<T>(ICoreAPI api) {
            api.RegisterBlockClass(ModID + "." + typeof(T).Name, typeof(T));
        }

        static void RegisterBlockEntityClass<T>(ICoreAPI api) {
            api.RegisterBlockEntityClass(ModID + "." + typeof(T).Name, typeof(T));
        }

        public override void StartPre(ICoreAPI api) {
            ModID = Mod.Info.ModID;

#if DEBUG
            Harmony.DEBUG = true;
#endif
        }

        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void Start(ICoreAPI api) {
            //api.RegisterBlockClass();

            RegisterBlockClass<BlockItemTest>(api);
            RegisterBlockEntityClass<BEItemTest>(api);

            if (!Harmony.HasAnyPatches(Mod.Info.ModID)) {
                Harmony.PatchAll();
            }
        }

        public override void StartServerSide(ICoreServerAPI api) {
        }

        public override void StartClientSide(ICoreClientAPI api) {

            var forms = new Dictionary<string, GuiDialog>() {
                ["test1"] = new GuiDialogTest1("Test1", api),
                ["test2"] = new GuiDialogTest2(api),
                ["cont-test1"] = new GuiDialogMNContainerTest("Container", api)
            };

            var parsers = api.ChatCommands.Parsers;
            var rootCommand = api.ChatCommands
                .Create("testgui")
                .RequiresPrivilege(Privilege.chat)
                .RequiresPlayer()
                .WithArgs(parsers.WordRange("formname", forms.Keys.ToArray()))
                .HandleWith(args => {
                    var form = forms[args[0].ToString()!];

                    form.TryOpen();

                    return TextCommandResult.Deferred;
                });

            //var subCommand1 = rootCommand
            //    .BeginSubCommand("test1")
            //    .WithArgs(parsers.Word("formname"))
            //    .HandleWith(args => {
            //        //if (test1Dialog == null) {
            //        //    test1Dialog = ;
            //        //}
            //        if (!test1Dialog.IsOpened()) {
            //            test1Dialog.SetupDialog();
            //            test1Dialog.TryOpen();
            //        }

            //        return TextCommandResult.Success("");
            //    });

            //GuiDialogTest2? test2Dialog = null;

            //var subCommand2 = rootCommand
            //    .BeginSubCommand("test2")
            //    .HandleWith(args => {
            //        if (test2Dialog == null) {
            //            test2Dialog = new(api);
            //        }

            //        if (!test2Dialog.IsOpened()) {
            //            test2Dialog.TryOpen();
            //        }
            //        return TextCommandResult.Success("");
            //    });
        }

        public override void Dispose() {
            base.Dispose();
            Harmony.UnpatchAll(Mod.Info.ModID);
        }
    }
}
