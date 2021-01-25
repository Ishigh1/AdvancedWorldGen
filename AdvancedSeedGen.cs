using System.Collections.Generic;
using System.Reflection;
using System.Text;
using IL.Terraria.IO;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Newtonsoft.Json;
using Terraria.ModLoader;

namespace AdvancedSeedGen
{
    public class AdvancedSeedGen : Mod
    {
        public static List<string> Options;
        public static Dictionary<string, List<string>> SeedTranslator;

        public override void Load()
        {
            SeedTranslator = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                Encoding.UTF8.GetString(GetFileBytes("SeedTranslation.json")));
            WorldFileData.SetSeed += SeedEdit;
        }

        public override void Unload()
        {
            WorldFileData.SetSeed -= SeedEdit;
        }

        // Edit the seed if necessary to add a worlgen seed if the original seed is just a special seed name
        private static void SeedEdit(ILContext il)
        {
            ILCursor ilCursor = new ILCursor(il);
            ilCursor.GotoNext(i => i.MatchRet());

            // SeedHelper.TweakSeed(ref _seed, ref _seedText);
            ilCursor.Emit(OpCodes.Ldarg_0);
            FieldInfo fieldInfo =
                typeof(Terraria.IO.WorldFileData).GetField("_seed", BindingFlags.NonPublic | BindingFlags.Instance);
            ilCursor.Emit(OpCodes.Ldflda, fieldInfo);
            ilCursor.Emit(OpCodes.Ldarga_S, (byte) 1);
            ilCursor.Emit(OpCodes.Call, typeof(SeedHelper).GetMethod("TweakSeed"));
        }
    }
}