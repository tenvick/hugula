using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Profiler
{

    public class ProfilerLoopStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // BuildCustomPlayerLoop();
        }

        #region  public 

        public void DumpProfilerInfo()
        {
            ProfilerFactory.DumpProfilerInfo(0, false, true);
        }
        #endregion

        #region  private
        void BuildCustomPlayerLoop()
        {
            if (ProfilerFactory.DoNotProfile == true)
                return;
            var newLoop = InsertCustomLoop();
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(newLoop);
        }

        bool InsertProfileMarker<T>(ref List<UnityEngine.LowLevel.PlayerLoopSystem> newList, string NAME)
        {
            UnityEngine.LowLevel.PlayerLoopSystem beginUpdateSystem = CreateProfilerMarker<ProfilerMarkerStart<T>>.GetNewSystem(NAME);
            UnityEngine.LowLevel.PlayerLoopSystem endUpdateSystem = CreateProfilerMarker<ProfilerMarkerEnd<T>>.GetNewSystem(NAME);

            int i = 0;
            bool bFound = false;
            foreach (var s in newList)
            {
                i++;
#if UNITY_EDITOR
                // Debug.LogFormat("PlayerLoopSystem({0})  find name={1} ", s.type, NAME);
#endif
                if (s.type.Name.Contains(NAME))
                {
                    bFound = true;
                    break;
                }
            }

            if (bFound)
            {
                newList.Insert(i - 1, beginUpdateSystem);
                newList.Insert(i + 1, endUpdateSystem);
            }
            return bFound;
        }

        void SetSubSystemListUpdate(ref UnityEngine.LowLevel.PlayerLoopSystem playerLoop, ref UnityEngine.LowLevel.PlayerLoopSystem update, ref List<UnityEngine.LowLevel.PlayerLoopSystem> newList, int idx)
        {
            update.subSystemList = newList.ToArray();
            playerLoop.subSystemList[idx] = update;
        }

        ///4-Update,5-PrelateUpdate,6-PostLateUpdate
        UnityEngine.LowLevel.PlayerLoopSystem InsertCustomLoop()
        {
            var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetDefaultPlayerLoop();
#if UNITY_EDITOR
            System.Type type;
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                type = playerLoop.subSystemList[i].type;
                Debug.LogFormat("{0}={1}", i, type);
                foreach (var k in playerLoop.subSystemList)
                {
                    Debug.LogFormat("       child {0}.Add({1})", type, k.type);
                }

            }
#endif

            var fixedUpdate = playerLoop.subSystemList[2];
            var preUpdate = playerLoop.subSystemList[3];
            var update = playerLoop.subSystemList[4];
            var preLateUpdate = playerLoop.subSystemList[5];
            var postLateUpdate = playerLoop.subSystemList[6];

            var newList_fixedUpdate = new List<UnityEngine.LowLevel.PlayerLoopSystem>(fixedUpdate.subSystemList);
            var newList_preUpdate = new List<UnityEngine.LowLevel.PlayerLoopSystem>(preUpdate.subSystemList);
            var newList_update = new List<UnityEngine.LowLevel.PlayerLoopSystem>(update.subSystemList);
            var newList_preLateUpdate = new List<UnityEngine.LowLevel.PlayerLoopSystem>(preLateUpdate.subSystemList);
            var newList_postLateUpdate = new List<UnityEngine.LowLevel.PlayerLoopSystem>(postLateUpdate.subSystemList);

            //fixed update
            InsertProfileMarker<TAG_PhysicsFixedUpdate>(ref newList_fixedUpdate, TAG_PhysicsFixedUpdate.NAME);

            //preUpdate
            InsertProfileMarker<TAG_AIUpdate>(ref newList_preUpdate, TAG_AIUpdate.NAME);
            InsertProfileMarker<TAG_ParticleSystemBeginUpdateAll>(ref newList_preLateUpdate, TAG_ParticleSystemBeginUpdateAll.NAME);
            // InsertProfileMarker<TAG_DirectorUpdateAnimationBegin>(ref newList_preLateUpdate, TAG_DirectorUpdateAnimationBegin.NAME);
            // InsertProfileMarker<TAG_DirectorUpdateAnimationEnd>(ref newList_preLateUpdate, TAG_DirectorUpdateAnimationEnd.NAME);

            //update
            InsertProfileMarker<TAG_ScriptRunBehaviourUpdate>(ref newList_update, TAG_ScriptRunBehaviourUpdate.NAME);
            InsertProfileMarker<TAG_ScriptRunDelayedDynamicFrameRate>(ref newList_update, TAG_ScriptRunDelayedDynamicFrameRate.NAME);

            //postLateUpdate
            InsertProfileMarker<TAG_ParticleSystemEndUpdateAll>(ref newList_postLateUpdate, TAG_ParticleSystemEndUpdateAll.NAME);
            InsertProfileMarker<TAG_UpdateRectTransform>(ref newList_postLateUpdate, TAG_UpdateRectTransform.NAME);
            InsertProfileMarker<TAG_PlayerUpdateCanvases>(ref newList_postLateUpdate, TAG_PlayerUpdateCanvases.NAME);
            InsertProfileMarker<TAG_UpdateAllRenderers>(ref newList_postLateUpdate, TAG_UpdateAllRenderers.NAME);

            //更新update
            SetSubSystemListUpdate(ref playerLoop, ref fixedUpdate, ref newList_fixedUpdate, 2);
            SetSubSystemListUpdate(ref playerLoop, ref preUpdate, ref newList_preUpdate, 3);
            SetSubSystemListUpdate(ref playerLoop, ref update, ref newList_update, 4);
            SetSubSystemListUpdate(ref playerLoop, ref preLateUpdate, ref newList_preLateUpdate, 5);
            SetSubSystemListUpdate(ref playerLoop, ref postLateUpdate, ref newList_postLateUpdate, 6);

            return playerLoop;
        }
        #endregion
    }
}