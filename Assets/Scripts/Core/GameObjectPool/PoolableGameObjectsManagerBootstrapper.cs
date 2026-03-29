using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Zeke.PoolableGameObjects
{
    internal static class PoolableGameObjectsManagerBootstrapper 
    {
        static PlayerLoopSystem timerSystem;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize() {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            if (!InsertTimerManager<Update>(ref currentPlayerLoop, 0)) {
                Debug.LogWarning("Improved Timers not initialized, unable to register TimerManager into the Update loop.");
                return;
            }
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
            
            static void OnPlayModeState(PlayModeStateChange state) {
                if (state == PlayModeStateChange.ExitingPlayMode) {
                    PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    RemoveTimerManager<Update>(ref currentPlayerLoop);
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);
                    
                    PoolableGameObjectsManager.Clear();
                }
            }
#endif
        }

        static void RemoveTimerManager<T>(ref PlayerLoopSystem loop) 
        {
            RemoveSystem<T>(ref loop, in timerSystem);
        }

        static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index) 
        {
            timerSystem = new PlayerLoopSystem() {
                type = typeof(PoolableGameObjectsManager),
                updateDelegate = PoolableGameObjectsManager.Update,
                subSystemList = null
            };
            return InsertSystem<T>(ref loop, in timerSystem, index);
        }

        public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;

            var playerLoopSystemList = new List<PlayerLoopSystem>(loop.subSystemList);
            for (int i = 0; i < playerLoopSystemList.Count; ++i)
            {
                if (playerLoopSystemList[i].type == systemToRemove.type && playerLoopSystemList[i].updateDelegate == systemToRemove.updateDelegate)
                {
                    playerLoopSystemList.RemoveAt(i);
                    loop.subSystemList = playerLoopSystemList.ToArray();
                    return;
                }
            }

            HandleSubSystemLoopForRemoval<T>(ref loop, systemToRemove);
        }

        static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;

            for (int i = 0; i < loop.subSystemList.Length; ++i)
            {
                RemoveSystem<T>(ref loop.subSystemList[i], systemToRemove);
            }
        }

        // Insert a system into the player loop
        public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
        {
            if (loop.type != typeof(T)) return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);

            var playerLoopSystemList = new List<PlayerLoopSystem>();
            if (loop.subSystemList != null) playerLoopSystemList.AddRange(loop.subSystemList);
            playerLoopSystemList.Insert(index, systemToInsert);
            loop.subSystemList = playerLoopSystemList.ToArray();
            return true;
        }

        static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
        {
            if (loop.subSystemList == null) return false;

            for (int i = 0; i < loop.subSystemList.Length; ++i)
            {
                if (!InsertSystem<T>(ref loop.subSystemList[i], in systemToInsert, index)) continue;
                return true;
            }

            return false;
        }

        public static void PrintPlayerLoop(PlayerLoopSystem loop)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Unity Player Loop");
            foreach (PlayerLoopSystem subSystem in loop.subSystemList)
            {
                PrintSubsystem(subSystem, sb, 0);
            }
            Debug.Log(sb.ToString());
        }

        static void PrintSubsystem(PlayerLoopSystem system, StringBuilder sb, int level)
        {
            sb.Append(' ', level * 2).AppendLine(system.type.ToString());
            if (system.subSystemList == null || system.subSystemList.Length == 0) return;

            foreach (PlayerLoopSystem subSystem in system.subSystemList)
            {
                PrintSubsystem(subSystem, sb, level + 1);
            }
        }
    }
}