using BepInEx;
using R2API.Utils;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using E8BarrierAdjustment.Configuration;
using Sandswept.Items.Greens;
using RiskOfOptions;
using System;

namespace E8BarrierAdjustment
{
    [BepInPlugin(E8BARRIERADJUSTMENT_GUID, E8BARRIERADJUSTMENT_NAME, E8BARRIERADJUSTMENT_VER)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamSandswept.Sandswept", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string E8BARRIERADJUSTMENT_GUID = "com.Hex3.E8BarrierAdjustment";
        public const string E8BARRIERADJUSTMENT_NAME = "E8BarrierAdjustment";
        public const string E8BARRIERADJUSTMENT_VER = "1.0.0";
        public static Main Instance;
        public static ConfigOptions configOptions;

        public void Awake()
        {
            Log.Init(Logger);
            Log.Info($"Init {E8BARRIERADJUSTMENT_NAME} {E8BARRIERADJUSTMENT_VER}");

            Instance = this;

            Log.Info($"Creating config...");
            configOptions = new ConfigOptions();
            if (Compat.Sandswept)
            {
                Log.Info($"Detected Sandswept");
            }
            if (Compat.RiskOfOptions)
            {
                Log.Info($"Detected RiskOfOptions");
                ModSettingsManager.SetModDescription("Prevents Eclipse 8 curse when damage is blocked by temporary barrier. Includes support for Sandswept's temporary plating.");
                // ModSettingsManager.SetModIcon(MainAssets.LoadAsset<Sprite>("Assets/VFXPASS3/Icons/icon.png"));
            }

            Log.Info($"Creating hooks...");

            IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
            {
                ILCursor c = new ILCursor(il);

                c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.PermanentCurse)));
                bool hit = c.TryGotoPrev
                (
                    x => x.MatchCall(out _),
                    x => x.MatchDiv(),
                    x => x.MatchLdcR4(out _),
                    x => x.MatchMul()
                );

                if (hit)
                {
                    c.Index += 4;
                    c.Emit(OpCodes.Ldarg, 0);
                    c.EmitDelegate<Func<HealthComponent, float>>((hc) =>
                    {
                        return SubtractOverhealth(hc);
                    });
                    c.Emit(OpCodes.Sub);
                }
                else 
                {
                    Log.Error("IL hook failed for barrier decay adjustment.");
                }
            };

            Log.Info($"Done");
        }

        public static float SubtractOverhealth(HealthComponent healthComponent)
        {
            float overhealth = 0f;

            if (configOptions.Barrier_Enabled.Value == true)
            {
                overhealth += healthComponent.barrier;
            }

            if (configOptions.Plating_Enabled.Value == true && Compat.Sandswept && healthComponent.body && healthComponent.body.master && healthComponent.body.master.bodyInstanceObject)
            {
                CharacterMaster characterMaster = healthComponent.body.master;
                MakeshiftPlate.PlatingManager platingManager = characterMaster.bodyInstanceObject.GetComponent<MakeshiftPlate.PlatingManager>();

                if (platingManager)
                {
                    overhealth += platingManager.CurrentPlating;
                }
            }

            return overhealth;
        }
    }
}
