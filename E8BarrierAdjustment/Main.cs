using BepInEx;
using R2API.Utils;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace E8BarrierAdjustment
{
    [BepInPlugin(E8BARRIERADJUSTMENT_GUID, E8BARRIERADJUSTMENT_NAME, E8BARRIERADJUSTMENT_VER)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    public class Main : BaseUnityPlugin
    {
        public const string E8BARRIERADJUSTMENT_GUID = "com.Hex3.E8BarrierAdjustment";
        public const string E8BARRIERADJUSTMENT_NAME = "E8BarrierAdjustment";
        public const string E8BARRIERADJUSTMENT_VER = "1.0.0";
        public static Main Instance;

        public void Awake()
        {
            Log.Init(Logger);
            Log.Info($"Init {E8BARRIERADJUSTMENT_NAME} {E8BARRIERADJUSTMENT_VER}");

            Instance = this;

            // IL: Subtracts current barrier amount from the damage considered by E8 curse
            IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if 
                (c.TryGotoNext
                    (
                        x => x.MatchCall(out _),
                        x => x.MatchDiv(),
                        x => x.MatchLdcR4(100f),
                        x => x.MatchMul(),
                        x => x.MatchLdcR4(0.4f),
                        x => x.MatchStloc(out _)
                    )
                )
                {
                    c.Index += 4;
                    c.Emit(OpCodes.Ldarg, 0);
                    c.Emit(OpCodes.Ldfld, typeof(HealthComponent).GetField("barrier", BindingFlags.Public | BindingFlags.Instance));
                    c.Emit(OpCodes.Sub);
                }
                else 
                {
                    Log.Error("IL hook failed for barrier decay adjustment.");
                }
            };

            Log.Info($"Done");
        }
    }
}
