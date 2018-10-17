﻿using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Sharlayan.Models;
using Sharlayan;
using Sharlayan.Core;
using System;
using Sharlayan.Models.ReadResults;

namespace Mappy.Helpers
{
    static class GameMemory
    {
        /// <summary>
        /// Set the game process
        /// </summary>
        /// <returns></returns>
        public static bool SetGameProcess()
        {
            Logger.Add("Looking for the FFXIV game process (ffxiv_dx11.exe)");

            Process[] processes = Process.GetProcessesByName("ffxiv_dx11");
            if (processes.Length > 0) {
                // supported: English, Chinese, Japanese, French, German, Korean
                string gameLanguage = "English";

                // whether to always hit API on start to get the latest sigs based on patchVersion, or use the local json cache (if the file doesn't exist, API will be hit)
                bool useLocalCache = true;

                // patchVersion of game, or latest
                string patchVersion = "latest";
                Process process = processes[0];
                ProcessModel processModel = new ProcessModel
                {
                    Process = process,
                    IsWin64 = true
                };
                MemoryHandler.Instance.SetProcess(processModel, gameLanguage, patchVersion, useLocalCache);

                Logger.Add("--> Hooked into ffxiv_dx11.exe");
                return true;
            }

            Logger.Add("! Could not find the FFXIV Game, please make sure you're on DX11 (dx9 dead)");
            return false;
        }

        /// <summary>
        /// Get the current player actor
        /// </summary>
        /// <returns></returns>
        public static ActorItem GetPlayer()
        {
            Reader.GetActors();
            return ActorItem.CurrentUser;
        }

        /// <summary>
        /// Get the current target actor
        /// </summary>
        /// <returns></returns>
        public static ActorItem GetCurrentTarget()
        {
            try
            {
                TargetResult readResult = Reader.GetTargetInfo();
                return readResult.TargetInfo.CurrentTarget;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GameMemory -> GetCurrentTarget");
            }

            return GetPlayer();
        }

        /// <summary>
        /// Get all monsters around the player in memory
        /// </summary>
        /// <returns></returns>
        public static List<ActorItem> GetMonstersAroundPlayer()
        {
            ActorResult readResult = Reader.GetActors();
            return readResult.CurrentMonsters.Select(e => e.Value).ToList();
        }

        /// <summary>
        /// Get all npcs around the player in memory
        /// </summary>
        /// <returns></returns>
        public static List<ActorItem> GetNpcsAroundPlayer()
        {
            ActorResult readResult = Reader.GetActors();
            return readResult.CurrentNPCs.Select(e => e.Value).ToList();
        }

        public static float GetCameraHeading()
        {
            var source = MemoryHandler.Instance.GetByteArray(Scanner.Instance.Locations["CAMERA"], 512);
            return BitConverter.ToSingle(source, 0x134);
        }
    }
}
