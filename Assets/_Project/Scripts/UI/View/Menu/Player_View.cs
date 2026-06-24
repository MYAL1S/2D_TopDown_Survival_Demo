using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色选择View
/// 保存ScrollView内容根节点、返回按钮和角色条目预制体引用
/// </summary>
public class Player_View : MonoBehaviour
{
    // ScrollView中用于承载角色条目的Content节点
    public Transform contentRoot;
    // 返回主菜单按钮
    public Button buttonBack;
    // 角色条目预制体
    public GameObject characterItemPrefab;
}
