namespace AdvancedWorldGen.ModHooks;

public class Fargowiltas : ModSystem
{
    // I'm too lazy to make their config work and it seems they don't care about mod compat, so that's fair
    // This removes the effect of the config hard setting the "special seed"
    // Could have changed the behavior to |= instead of =, but eh, easier to nullify it
    public override void Load()
    {
        if (ModLoader.TryGetMod("Fargowiltas", out Mod mod) &&
            mod.TryGetMethod("Fargowiltas.FargoWorld", "PreWorldGen", BindingFlags.Public | BindingFlags.Instance, out MethodInfo? methodInfo))
            HookEndpointManager.Modify(methodInfo, GetRidOfFlagConfig);
    }
    
    public override void Unload()
    {
        if (ModLoader.TryGetMod("Fargowiltas", out Mod mod) &&
            mod.TryGetMethod("Fargowiltas.FargoWorld", "PreWorldGen", BindingFlags.Public | BindingFlags.Instance, out MethodInfo? methodInfo))
            HookEndpointManager.Unmodify(methodInfo, GetRidOfFlagConfig);
    }

    private static void GetRidOfFlagConfig(ILContext ilContext)
    {
        ILCursor cursor = new(ilContext);
        while (!cursor.Next.MatchLdarg(0))
        {
            cursor.Remove();
        }
    }
}