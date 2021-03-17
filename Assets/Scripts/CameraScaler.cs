using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Launchship2DTiles
{
    public class CameraScaler : MonoBehaviour
    {
        public BoardBuilding boardBuilding;
        public float cameraDepthOffset = -10f;
        public float aspectRatio = 0.625f;
        public float padding = 2;
        public float yOffset = 1;
        public float xOffset = 1;

        [HideInInspector] public float xaxsis;
        [HideInInspector] public float yaxsis;
        void Start()
        {
            xOffset = boardBuilding.offsetX;
            yOffset = boardBuilding.offsetY;
            //aspectRatio = Camera.main.aspect;
        }
        private void Update()
        {
            if (boardBuilding != null)
            {
                xaxsis = boardBuilding.X_Axis - 1;
                yaxsis = boardBuilding.Y_Axis - 1;
                RepositionCamera(xaxsis, yaxsis);
                aspectRatio = (xaxsis / yaxsis) * Camera.main.aspect;
                padding = 2 - (yaxsis / xaxsis);
            }
        }
        void RepositionCamera(float x, float y)
        {
            //Vector3 tempPosition = new Vector3((x / 2) + xOffset, (y / 2) + yOffset, cameraDepthOffset);
            Vector3 tempPosition = new Vector3(x * xOffset, y * yOffset, cameraDepthOffset);
            tempPosition = tempPosition / 2;
            tempPosition.z = cameraDepthOffset;
            transform.position = tempPosition;
            if (x >= y)
            {
                Camera.main.orthographicSize = (boardBuilding.X_Axis / 2 + padding) / aspectRatio;
            }
            //else if (x == y - 1)
            //{
            //    Camera.main.orthographicSize = (boardBuilding.Y_Axis / 2 + padding) / aspectRatio;
            //}
            else
            {
                Camera.main.orthographicSize = (boardBuilding.Y_Axis / 2 + padding) / aspectRatio;
            }
        }

        Vector2 screenBounds;
        void ScreenInView()
        {
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, this.transform.position.z));
        }
    }//class
}//namespace