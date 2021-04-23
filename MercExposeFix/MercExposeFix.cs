using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MercExposeFix
{
    [BepInPlugin("com.Moffein.MercExposeFix", "Merc Expose Fix", "1.1.1")]
    public class MercExposeFix : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool hasExpose = self.body.HasBuff(RoR2Content.Buffs.MercExpose);
                orig(self, damageInfo);
                if (hasExpose && !self.body.HasBuff(RoR2Content.Buffs.MercExpose))
                {
                    CharacterBody attackerBody = null;
                    if (damageInfo.attacker)
                    {
                        attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    }
                    if (!damageInfo.attacker || attackerBody == null || (attackerBody != null && attackerBody.bodyIndex != BodyCatalog.FindBodyIndex("MercBody")))
                    {
                        attackerBody.AddBuff(RoR2Content.Buffs.MercExpose);
                    }
                }
            };
        }
    }
}

namespace R2API.Utils
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ManualNetworkRegistrationAttribute : Attribute
    {
    }
}
