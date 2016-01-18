using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;
/*
 * 
 * 拖入本脚本到附属了Image的对象上时会自动关联Image与Image所在的Atlas
 * 本脚本在Awake时会强制进行对象检查以确保Image和Atlas不为空
 * 如果你不需要动态设置Image的Sprite请不要添加此脚本
 * 
 * 本脚本可以支持多个Atlas的筛选,但是会优先找relativeAtlas,如果都找不到会打印一个错误(不是异常)
 * 
 **/

[SLua.CustomLuaClass]
[UnityEngine.ExecuteInEditMode]
public class ImageExtension : MonoBehaviour
{
    string _spriteName;

    public Image relativeImage;
    public Atlas relativeAtlas;

    public Atlas[] otherAtlas;

    public string spriteName
    {
        set
        {
            _spriteName = value;
            Sprite temp = null;
            temp = relativeAtlas.GetSpriteByName(value);
            if (temp == null && otherAtlas != null)
            {
                for (int i = 0; i < otherAtlas.Length; i++)
                {
                    temp = otherAtlas[i].GetSpriteByName(value);
                    if (temp != null)
                        break;
                }
            }
            relativeImage.sprite = temp;
            if (temp == null)
                Debug.LogWarning(gameObject.name + ": can't find sprite in the atlas [sprite name is " + value + "]");
        }
        get
        {
            return _spriteName;
        }
    }

    public void SetNativeSize()
    {
        if (relativeImage != null)
            relativeImage.SetNativeSize();
    }

    void Awake()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            Assert.IsNotNull(relativeImage, gameObject.name + ": the image extension relative a null image");
            Assert.IsNotNull(relativeAtlas, gameObject.name + ": the image extension relative a null Atlas");
        }
        else
        {
            if (relativeImage == null)
                relativeImage = GetComponent<Image>();

            if (relativeAtlas == null && relativeImage != null)
                relativeAtlas = EditorAtlasUtilites.GetAtlas(relativeImage.sprite);
        }
#endif
    }
}
