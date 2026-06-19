using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class Settings
{
    #region ANIMATOR PARAMETERS
    //Animator 参数 - Player
    public static int speed = Animator.StringToHash("Speed");
    public static int defeat = Animator.StringToHash("Defeat");
    public static int revive = Animator.StringToHash("Revive");

    // 基础玩家动画速度
    // 通过玩家速度计算动画速度
    public static float baseSpeedForPlayerAnimations = 2f;
    // 基础敌人动画速度
    // 通过敌人速度计算动画速度
    public static float baseSpeedForEnemyAnimations = 1f;
    #endregion
}
