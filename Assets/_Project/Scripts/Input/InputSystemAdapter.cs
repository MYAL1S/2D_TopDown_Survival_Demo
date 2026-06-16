using UnityEngine;

/// <summary>
/// 输入系统适配器
/// </summary>
public class InputSystemAdapter : MonoBehaviour
{
    private static InputSystemAdapter _instance;

    public static InputSystemAdapter Instance
    {
        get
        {
            //懒汉模式单例
            if(_instance == null)
            {
                GameObject obj = new GameObject("inputSystemAdapter");
                _instance = obj.AddComponent<InputSystemAdapter>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    /// <summary>
    /// 移动输入 通过属性暴露给外部使用
    /// </summary>
    public Vector2 MoveInput { get; private set; }

    private void Update()
    {
        ///每一帧读取数入
        ReadMoveInput();
    }

    /// <summary>
    /// 读取移动输入
    /// 并归一化 避免斜向移动时速度过快
    /// </summary>
    private void ReadMoveInput()
    {
        Vector2 keyboardInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        Vector2 joystickInput = VirtualJoystick.ActiveJoystick != null
            ? VirtualJoystick.ActiveJoystick.MoveInput
            : Vector2.zero;

        MoveInput = joystickInput.sqrMagnitude > 0f ? joystickInput : keyboardInput;

        if (MoveInput.sqrMagnitude > 1f)
        {
            MoveInput = MoveInput.normalized;
        }
    }
}
