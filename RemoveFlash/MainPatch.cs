using System;
using HarmonyLib;
using UnityEngine;

namespace RemoveFlash.MainPatch {
    [HarmonyPatch(typeof(scrFlash), "Flash")]
    internal static class RemoveFailFlash {
        private static bool Prefix() {
            if (Main.disableFlashOnce) {
                Main.disableFlashOnce = false;
                return false;
            }
            if (!Main.Settings.FailWhiteFlash) return true;
            if (scrController.CheckStateIs("Fail")) return false;
            return true;
        }
    }
    
    [HarmonyPatch(typeof(scrFlash), "OnDamage")]
    internal static class RemoveDamageFlash {
        private static void Prefix() {
            if (!Main.Settings.FailRedFlash) return;
            Main.disableFlashOnce = true;
        }
    }

    [HarmonyPatch(typeof(ffxCheckpoint), "doEffect")]
    internal static class RemoveCheckpointFlash {
        private static void Prefix(ffxCheckpoint __instance) {
            if (!Main.Settings.CheckpointFlash) return;
            Main.disableFlashOnce = true;
        }
    }

    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
    internal static class RemoveClearFlash {
        private static void Prefix(scrController __instance, int portalDestination, string portalArguments) {
            if (!Main.Settings.ClearFlash) return;
            Main.disableFlashOnce = true;
        }
    }
}