using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Profiler
{
    public class ProfilerDump : MonoBehaviour
    {
#if PROFILER_DUMP || !HUGULA_RELEASE
        private void OnDestroy()
        {
            ProfilerFactory.DumpProfilerInfo(0, true, false);
        }

#endif

    }
}