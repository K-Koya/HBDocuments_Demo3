using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Cinemachine
{
    /// <summary>CinemachineInputProviderを継承して、照準感度調整を可能に</summary>
    public class CinemachineInputProviderTunable : CinemachineInputProvider
    {
        /// <summary>水平方向入力感度</summary>
        static float _SenseRateX = 1.0f;

        /// <summary>鉛直方向入力感度</summary>
        static float _SenseRateY = 1.0f;

        /// <summary>水平方向入力感度</summary>
        public static float SenseRateX { get => _SenseRateX; set => _SenseRateX = value; }
        /// <summary>鉛直方向入力感度</summary>
        public static float SenseRateY { get => _SenseRateY; set => _SenseRateY = value; }


        /// <summary>
        /// TODO
        /// Implementation of AxisState.IInputAxisProvider.GetAxisValue().
        /// Axis index ranges from 0...2 for X, Y, and Z.
        /// Reads the action associated with the axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns>The current axis value</returns>
        public override float GetAxisValue(int axis)
        {
            if (enabled)
            {
                var action = ResolveForPlayer(axis, axis == 2 ? ZAxis : XYAxis);
                if (action != null)
                {
                    switch (axis)
                    {
                        case 0: return action.ReadValue<Vector2>().x * _SenseRateX;
                        case 1: return action.ReadValue<Vector2>().y * _SenseRateY;
                        case 2: return action.ReadValue<float>();
                    }
                }
            }
            return 0;
        }
    }
}
