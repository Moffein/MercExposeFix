using BepInEx;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
using R2API.Utils;
using Mono.Cecil.Cil;

namespace MercExposeFix
{
    [BepInPlugin("com.Moffein.MercExposeFix", "Merc Expose Fix", "1.2.0")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class MercExposeFix : BaseUnityPlugin
    {
        public static BuffDef emptyBuff = null;
        public void Awake()
        {
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "MercExpose"),
                     x => x.MatchCallvirt<CharacterBody>("RemoveBuff")
                    );
                c.Remove();
                c.Emit<MercExposeFix>(OpCodes.Ldsfld, nameof(MercExposeFix.emptyBuff));

                //Remove buff when it is confirmed that it's Merc attacking.
               c.GotoNext(
                     x => x.MatchCallvirt<CharacterBody>("get_damage")
                    );
                
                c.Emit(OpCodes.Ldarg_0);//victim healthcomponent
                c.EmitDelegate<Func<CharacterBody, HealthComponent, CharacterBody>>((body, victimHealth) =>
                {
                    if(victimHealth.body)
                    {
                        victimHealth.body.RemoveBuff(RoR2Content.Buffs.MercExpose);
                    }
                    return body;
                });
            };

        }
    }
}
