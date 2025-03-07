


using UnityEngine;

public static class Settings
{
    #region GAME PARAMETER
    public static int truckStopCount = 3;
    #endregion

    #region MONSTER PARAMETER
    public static float jumpForce = 10f;
    #endregion


    #region LAYER MASK
    public static LayerMask truckLayer = LayerMask.GetMask("Truck"); // 트럭 레이어
    #endregion


}