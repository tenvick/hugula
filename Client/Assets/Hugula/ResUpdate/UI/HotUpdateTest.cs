using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.ResUpdate
{

    public class HotUpdateTest : MonoBehaviour
    {
        public Button btn;
        public Text text;
        public string pauseText = "暂停";
        public string ContinueText = "继续";

        // Start is called before the first frame update
        void Start()
        {
            btn.onClick.AddListener(OnClick);
            text.text = pauseText;
        }

        void OnClick()
        {
            if (text.text == pauseText)
            {
                text.text = ContinueText;
                BackGroundDownload.instance.Pause();
            }
            else if (text.text == ContinueText)
            {
                text.text = pauseText;
                BackGroundDownload.instance.Begin();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}