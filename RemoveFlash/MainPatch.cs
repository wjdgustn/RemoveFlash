using System;
using System.CodeDom;
using System.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace RemoveFlash.MainPatch {
    [HarmonyPatch(typeof(scrFlash), "OnDamage")]

    internal static class RemoveDamageFlash {
        private static bool Prefix() {
            return false;
        }
    }

    [HarmonyPatch(typeof(ffxCheckpoint), "doEffect")]

    internal static class RemoveCheckpointFlash {
        private static bool Prefix(ffxCheckpoint __instance) {
            if (GCS.speedTrialMode)
                return false;
            GCS.checkpointNum = __instance.GetComponent<scrFloor>().seqID + __instance.checkpointTileOffset;
            __instance.floor.floorIcon = FloorIcon.Checkpoint;
            __instance.floor.UpdateIconSprite();
            
            return false;
        }
    }
}