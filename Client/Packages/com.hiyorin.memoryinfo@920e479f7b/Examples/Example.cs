using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryInfo.Example
{
    public sealed class Example : MonoBehaviour
    {
        [SerializeField] private Text _text = null;

        private IEnumerator Start()
        {
            var plugin = new MemoryInfoPlugin();

            while(true)
            {
                var info = plugin.GetMemoryInfo();
                _text.text = string.Format("{0}/{1} KB ({2}%)", info.UsedSize, info.TotalSize, (int)(100f * info.UsedSize / info.TotalSize));
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
