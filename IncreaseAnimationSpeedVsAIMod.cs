using MelonLoader;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System;
using Blizzard.T5.Core.Time;
using PegasusShared;

namespace IncreaseAnimationSpeedVsAI
{
    public class IncreaseAnimationSpeedVsAIMod : MelonMod
    {
        public static MelonLogger.Instance SharedLogger;

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            IncreaseAnimationSpeedVsAIMod.SharedLogger = LoggerInstance;
            var harmony = this.HarmonyInstance;
            harmony.PatchAll(typeof(GameMgrPatcher));
        }

        public override void OnDeinitializeMelon()
        {
            base.OnDeinitializeMelon();
            TimeScaleMgr.Get().SetGameTimeScale(1);
        }
    }

    public static class GameMgrPatcher
    {
        public static List<GameType?> AI_GAME_TYPES = new List<GameType?> {
            GameType.GT_VS_AI,
            GameType.GT_TUTORIAL,
            GameType.GT_TEST_AI_VS_AI,
            GameType.GT_MERCENARIES_PVE,
        };

        [HarmonyPatch(typeof(GameMgr), "OnGameSetup")]
        [HarmonyPostfix]
        public static void OnGameSetupPostfix()
        {
            var isAgainstAI = AI_GAME_TYPES.Contains(GameMgr.Get()?.GetGameType());
            var timeScale = isAgainstAI ? 4 : 1;
            if (isAgainstAI)
            {
                IncreaseAnimationSpeedVsAIMod.SharedLogger.Msg($"Starting game for gameType: {GameMgr.Get().GetGameType()} with timeScale: {timeScale}");
            }
            TimeScaleMgr.Get().SetGameTimeScale(timeScale);
        }

        [HarmonyPatch(typeof(GameMgr), "OnGameEnded")]
        [HarmonyPostfix]
        public static void OnGameEndedPostfix()
        {
            //IncreaseAnimationSpeedVsAIMod.SharedLogger.Msg($"Ending game");
            TimeScaleMgr.Get().SetGameTimeScale(1);
        }
    }
}
