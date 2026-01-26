using System;
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
using System.Text;
using MNGUITest.Patches;

namespace MNGUITest;

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

            PatchScreenConnectingServer(api);
        }
    }

    public override void StartServerSide(ICoreServerAPI api) {
        try {
            throw new Exception($"Test exception");
        }
        catch (Exception ex) {
            api.Logger.Error(ex);
        }
        var sb = new StringBuilder();
        for (int i = 0; i < 10; ++i) {
            sb.Append("0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
        }

        api.Logger.Event(sb.ToString());
    }

    public override void StartClientSide(ICoreClientAPI api) {

        var forms = new Dictionary<string, GuiDialog>() {
            ["test1"] = new GuiDialogTest1("Test1", api),
            ["test2"] = new GuiDialogTest2(api),
            ["cont-test1"] = new GuiDialogMNContainerTest("Container", api),
            ["simplest"] = new GuiDialogSimplest("Simplest", api),
            ["simplest-container"] = new GuiDialogSimplestContainer("Simplest Container", api),
            ["dyntext"] = new GuiDialogDynamicTextTest("dyntext", api),
            ["refactored-layout"] = new GuiDialogRefactoredLayoutTest("Refactor", api),
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

    public void PatchScreenConnectingServer(ICoreAPI api) {
        var classType = GetScreenConnectingServerType();
        if (classType == null) {
            api.Logger.Error("Type not found");
            return;
        }

        var method = AccessTools.Method(classType, "LogAdded");

        var harmonyMethod = new HarmonyMethod(AccessTools.Method(typeof(GuiScreenConnectingServerLogAddedPatcher), "Prefix"));

        Harmony.Patch(method, prefix: harmonyMethod);
    }

    public Type? GetScreenConnectingServerType() {
        var typeName = "Vintagestory.Client.GuiScreenConnectingToServer";
        var assem = typeof(SaveGame).Assembly;
        var type = assem.GetType(typeName, ignoreCase: false, throwOnError: false);

        return type;
    }
}
