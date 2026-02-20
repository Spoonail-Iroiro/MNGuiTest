using System;
using HarmonyLib;
using MNGuiTest.BlockEntities;
using MNGuiTest.Blocks;
using MNGuiTest.Gui;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using System.Text;
using MNGuiTest.Patches;
using MNGuiTest.Gui.Specific;
using MNGui.Extensions;
using MNGui.Util;

namespace MNGuiTest;

public class MNGuiTestModSystem : ModSystem {
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

        api.Logger.Event($"Using MNGui v{MNGui.Meta.LibraryInfo.Version}!");
    }

    public override void StartServerSide(ICoreServerAPI api) {
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
            ["debug-window"] = new GuiDialogDebugWindow("Debugging", api),
            ["inset-container-samples"] = new GuiDialogInsetContainerSamples("Debugging", api),
            ["test1-with-container"] = new GuiDialogTest1WithContainer("Test1WithContainer", api),
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

        var debugWindowCommand = api.ChatCommands
            .Create("debug-window")
            .RequiresPrivilege(Privilege.chat)
            .RequiresPlayer()
            .HandleWith(args => {
                var cmdSB = new StringBuilder();

                var modSystem = api.ModLoader.GetModSystem<GUIDebugModSystem>();

                var hoveredElements = modSystem.FindHoveredElements().ToList();

                foreach (var elem in hoveredElements) {
                    cmdSB.AppendLine($"{elem.GetType().Name}");
                    if (elem.Bounds != null) {
                        var bounds = elem.Bounds;
                        cmdSB.AppendLine($"Pos: ({BoundsUtil.UnScaled(bounds.absX)}, {BoundsUtil.UnScaled(bounds.absY)})");
                        cmdSB.AppendLine($"Size: ({bounds.UnscaledOuterWidth()}, {bounds.UnscaledOuterHeight()})");
                    }
                }

                return TextCommandResult.Success(cmdSB.ToString());
            });

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
