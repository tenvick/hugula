using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Hugula.Profiler
{

    #region EarlyUpdate 1
    public class TAG_PlayerCleanupCachedData
    {
        public static readonly string NAME = "PlayerCleanupCachedData";
    }
    #endregion

    #region FixedUpdate 2
    /// <summary>
    /// FixedUpdate 2
    /// </summary>
    public class TAG_PhysicsFixedUpdate
    {
        public static readonly string NAME = "PhysicsFixedUpdate";
    }

    #endregion

    #region PreUpdate 3
    /// <summary>
    /// PreUpdate 3
    /// </summary>
    public class TAG_AIUpdate
    {
        public static readonly string NAME = "AIUpdate";
    }

    #endregion

    #region Update 4

    /// <summary>
    /// Update 4
    /// </summary>
    public class TAG_DirectorUpdate
    {
        public static readonly string NAME = "DirectorUpdate";
    }

    /// <summary>
    /// Update 4
    /// </summary>
    public class TAG_ScriptRunBehaviourUpdate
    {
        public static readonly string NAME = "ScriptRunBehaviourUpdate";
    }

    /// <summary>
    /// Update 4
    /// </summary>
    public class TAG_ScriptRunDelayedDynamicFrameRate
    {
        public static readonly string NAME = "ScriptRunDelayedDynamicFrameRate";
    }


    #endregion

    #region PreLateUpdate 5
    /// <summary>
    /// PreLateUpdate 5
    /// </summary>
    public class TAG_ParticleSystemBeginUpdateAll
    {
        public static readonly string NAME = "ParticleSystemBeginUpdateAll";
    }

    /// <summary>
    /// PreLateUpdate 5
    /// </summary>
    public class TAG_DirectorUpdateAnimationBegin
    {
        public static readonly string NAME = "DirectorUpdateAnimationBegin";
    }

    /// <summary>
    /// PreLateUpdate 5
    /// </summary>
    public class TAG_DirectorUpdateAnimationEnd
    {
        public static readonly string NAME = "DirectorUpdateAnimationEnd";
    }
    #endregion

    #region PostLateUpdate 6

    /// <summary>
    /// PostLateUpdate 6
    /// </summary>
    public class TAG_UpdateAllRenderers
    {
        public static readonly string NAME = "UpdateAllRenderers";
    }

    /// <summary>
    /// PostLateUpdate 6
    /// </summary>
    public class TAG_PlayerUpdateCanvases
    {
        public static readonly string NAME = "PlayerUpdateCanvases";
    }

    /// <summary>
    /// PostLateUpdate 6
    /// </summary>
    public class TAG_ParticleSystemEndUpdateAll
    {
        public static readonly string NAME = "ParticleSystemEndUpdateAll";
    }

    /// <summary>
    /// PostLateUpdate 6
    /// </summary>
    public class TAG_UpdateRectTransform
    {
        public static readonly string NAME = "UpdateRectTransform";
    }
    #endregion

    public struct CreateProfilerMarker<T>
    {
        public delegate void OnUpdate();
        public static UnityEngine.LowLevel.PlayerLoopSystem GetNewSystem(string kWatchFuncName)
        {
            UnityEngine.LowLevel.PlayerLoopSystem kNewObj = new UnityEngine.LowLevel.PlayerLoopSystem();
            kNewObj.type = typeof(T);
            kNewObj.updateDelegate = (UnityEngine.LowLevel.PlayerLoopSystem.UpdateFunction)(typeof(T).GetMethod("UpdateFunction").CreateDelegate(typeof(UnityEngine.LowLevel.PlayerLoopSystem.UpdateFunction)));
            var type = typeof(T);
            var field = type.GetField("m_CurrentProfiler", BindingFlags.Public | BindingFlags.Static);
            field.SetValue(null, ProfilerFactory.GetProfiler(kWatchFuncName));
            return kNewObj;
        }

    }

    public struct ProfilerMarkerStart<T>
    {
        public static StopwatchProfiler m_CurrentProfiler;
        public static void UpdateFunction()
        {
            if (m_CurrentProfiler != null)
            {
                m_CurrentProfiler.Start();
            }
        }
    }

    public struct ProfilerMarkerEnd<T>
    {
        public static StopwatchProfiler m_CurrentProfiler;
        public static void UpdateFunction()
        {
            if (m_CurrentProfiler != null)
            {
                m_CurrentProfiler.Stop();
            }
        }
    }
}