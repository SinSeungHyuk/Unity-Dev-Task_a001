


using UnityEngine;

public static class Settings
{
    #region GAME PARAMETER
    public static float truckStopCount = 4;
    #endregion

    #region MONSTER PARAMETER
    public static float jumpForce = 15f;
    public static Vector2 jumpDir = new Vector2(-0.4f,1.2f).normalized;
    #endregion


    #region LAYER MASK
    public static LayerMask truckLayer = LayerMask.GetMask("Truck"); // 트럭 레이어
    #endregion


}