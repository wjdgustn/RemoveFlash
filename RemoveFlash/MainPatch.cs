using System;
using HarmonyLib;
using UnityEngine;

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

    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
    internal static class RemoveClearFlash {
        private static bool Prefix(scrController __instance, int portalDestination, string portalArguments) {
            var disableCongratsMessage = 
                (Boolean) AccessTools.Field(__instance.GetType(), "disableCongratsMessage").GetValue(__instance);
            
            if (GCS.d_newgrounds)
                scrNewgroundsAPIManager.StaticCheckMedals();
            __instance.portalArguments = portalArguments;
            __instance.portalDestination = portalDestination;
            if(!__instance.gameworld) scrFlash.Flash(Color.white.WithAlpha(0.4f));

            if (__instance.gameworld) {
                bool flag1 = __instance.mistakesManager.IsAllPurePerfect();
                var flag2 = false;

                if (__instance.isLevelEditor) {
                    __instance.txtCongrats.text = RDString.Get(flag1 ? "status.allPurePerfect" : "status.congratulations");

                    if (GCS.standaloneLevelMode) {
                        var num = (int)__instance.mistakesManager.SaveCustom(__instance.customLevel.levelData.Hash, true, GCS.currentSpeedRun);
                    }
                }
                else if (__instance.isbosslevel) {
                    __instance.mistakesManager.CalculatePercentAcc();
                    var language = (int)RDString.language;

                    if ((scrController.currentWorld == 7) & flag1) {
                        var str = RDString.Get("status.world7Purrfect");
                        __instance.txtCongrats.text = !str.Contains("[Don't translate]") ? str : RDString.Get("status.allPurePerfect");
                    }
                    else {
                        __instance.txtCongrats.text = RDString.Get(flag1 ? "status.allPurePerfect" : "status.congratulations");
                    }
                    __instance.endLevelType = GCS.practiceMode || GCS.d_booth ? EndLevelType.WinInPracticeMode : __instance.mistakesManager.Save(scrController.currentWorld, true, GCS.currentSpeedRun);

                    if (GCS.speedTrialMode)
                        GCS.nextSpeedRun = GCS.currentSpeedRun + 0.1f;
                }
                else {
                    flag2 = true;
                    __instance.printe((object)("levelName: " + __instance.levelName));
                    var level = int.Parse(__instance.levelName.ToCharArray()[__instance.levelName.Length - 1].ToString());
                    var currentWorld = scrController.currentWorld;

                    if (Persistence.GetLevelTutorialProgress(currentWorld) < level)
                        Persistence.SetLevelTutorialProgress(currentWorld, level);
                }

                if (disableCongratsMessage | flag2)
                    __instance.txtCongrats.text = "";
                __instance.controller.txtCongrats.gameObject.SetActive(true);

                if (scrController.showDetailedResults && !flag2) {
                    var hits1 = GetHits(HitMargin.Perfect);
                    var hits2 = GetHits(HitMargin.EarlyPerfect);
                    var hits3 = GetHits(HitMargin.LatePerfect);
                    var hits4 = GetHits(HitMargin.VeryEarly);
                    var hits5 = GetHits(HitMargin.VeryLate);
                    var resultCount = GetHits(HitMargin.TooEarly) + GetHits(HitMargin.TooLate);
                    var hitMarginColoursUi = RDConstants.data.hitMarginColoursUI;

                    __instance.controller.txtResults.text = Localized("ePerfect") + Result(hits2, hitMarginColoursUi.colourLittleEarly.ToHex()) + "     " + Localized("perfect") + Result(hits1, hitMarginColoursUi.colourPerfect.ToHex()) + "     " +
                                                      Localized("lPerfect") + Result(hits3, hitMarginColoursUi.colourLittleLate.ToHex()) + "\n" + Localized("early") + Result(hits4, hitMarginColoursUi.colourVeryEarly.ToHex()) + "     " + Localized("misses") +
                                                      Result(resultCount, hitMarginColoursUi.colourTooEarly.ToHex()) + "     " + Localized("late") + Result(hits5, hitMarginColoursUi.colourVeryLate.ToHex()) + "\n" + Localized("accuracy") +
                                                      string.Format("{0:0.00}%", (float)((double)__instance.mistakesManager.percentAcc * 100.0)) + "     " + Localized("checkpoints") + scrController.checkpointsUsed;
                    __instance.controller.txtResults.gameObject.SetActive(true);
                }
            }

            if (__instance.uiController.firstDifficultyIndex == 2 && !__instance.uiController.changedDifficulty) {
                __instance.controller.txtAllStrictClear.text = RDString.Get("status.allStrictClear");
                __instance.controller.txtAllStrictClear.gameObject.SetActive(true);
            }

            if (__instance.isEditingLevel)
                return false;
            __instance.ChangeState((Enum)scrController.States.Won);

            string Localized(string s) {
                return RDString.Get("status.results." + s) + ": ";
            }

            string Result(int resultCount, string color) {
                return string.Format("<color={0}>{1}</color>", color, resultCount);
            }

            int GetHits(HitMargin hitMargin) {
                return __instance.mistakesManager.GetHits(hitMargin);
            }

            return false;
        }
    }
}