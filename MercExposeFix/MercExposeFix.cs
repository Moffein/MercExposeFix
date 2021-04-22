using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MercExposeFix
{
    [BepInPlugin("com.Moffein.MercExposeFix", "Merc Expose Fix", "1.0.1")]
    public class MercExposeFix : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool applyExpose = damageInfo.attacker && (damageInfo.damageType & DamageType.ApplyMercExpose) > 0;
                ExposeComponent ec = null;
                GameObject oldExpose = null;
                if (damageInfo.procCoefficient > 0f && self.body.HasBuff(RoR2Content.Buffs.MercExpose))
                {
                    ec = self.gameObject.GetComponent<ExposeComponent>();
                    if (ec && ec.pendingExpose == damageInfo.attacker)
                    {
                        oldExpose = ec.pendingExpose;
                        ec.pendingExpose = null;
                    }
                }

                orig(self, damageInfo);

                if (damageInfo.rejected && oldExpose)
                {
                    ec.pendingExpose = oldExpose;
                }
                else if (applyExpose && !damageInfo.rejected && self.alive)
                {
                    if (!ec)
                    {
                        ec = self.gameObject.AddComponent<ExposeComponent>();
                    }
                    ec.pendingExpose = damageInfo.attacker;
                }
            };

            On.RoR2.CharacterBody.RemoveBuff_BuffIndex += (orig, self, buffIndex) =>
            {
                orig(self, buffIndex);
                if (NetworkServer.active && buffIndex == RoR2Content.Buffs.MercExpose.buffIndex)
                {
                    ExposeComponent ec = self.gameObject.GetComponent<ExposeComponent>();
                    if (ec && ec.pendingExpose)
                    {
                        CharacterBody cb = ec.pendingExpose.GetComponent<CharacterBody>();
                        if (cb && cb.skillLocator)
                        {
                            cb.skillLocator.DeductCooldownFromAllSkillsServer(1f);
                        }
                        ec.pendingExpose = null;
                    }
                }
            };
        }
    }

    public class ExposeComponent : MonoBehaviour
    {
        public GameObject pendingExpose;
    }
}

namespace R2API.Utils
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ManualNetworkRegistrationAttribute : Attribute
    {
    }
}
