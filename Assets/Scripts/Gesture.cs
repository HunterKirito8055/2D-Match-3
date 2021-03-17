using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Launchship2DTiles
{
    public class Gesture : MonoBehaviour
    {
        public static SwipeEnum direction;
        static public float swipeDead = 0.3f;
        static BoardBuilding boardBuilding;
        [SerializeField] float _angle;
        static float deltaX, deltaY;
        private void Awake()
        {
            boardBuilding = FindObjectOfType<BoardBuilding>();
        }
        public static SwipeEnum GetDirection(Vector2 lasttouch, Vector2 firstTouch)
        {
            deltaX = lasttouch.x - firstTouch.x;
            deltaY = lasttouch.y - firstTouch.y;

            direction = SwipeEnum.none;
            if ((Mathf.Abs(deltaY) > swipeDead || Mathf.Abs(deltaX) > swipeDead) && !BoardBuilding.isTransitioning)
            {
                float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;

                if (angle >= 45 && angle <= 135)
                {
                    direction = SwipeEnum.up;
                }
                if (angle <= 45 && angle >= -45)
                {
                    direction = SwipeEnum.right;

                }
                if (angle <= -45 && angle >= -135)
                {
                    direction = SwipeEnum.down;

                }
                if (angle >= 135 || angle <= -135)
                {
                    direction = SwipeEnum.left;
                }
            }
            else
            {
                return SwipeEnum.none;
            }

            return direction;
        }

        //static public CommandPattern Cass()
        //{
        //    switch (direction)
        //    {
        //        case SwipeEnum.none:
        //            return null;

        //        case SwipeEnum.up:
        //            return SwipeUp;

        //        case SwipeEnum.right:
        //            return SwipeRight;

        //        case SwipeEnum.left:
        //            return SwipeLeft;

        //        case SwipeEnum.down:
        //            return SwipeDown;

        //        default:
        //            break;
        //    }
        //    return null;
        //}


    }//class

    public enum SwipeEnum
    {
        none, up, right, left, down
    }
}