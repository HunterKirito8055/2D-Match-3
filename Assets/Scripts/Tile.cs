using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

namespace Launchship2DTiles
{
    public class Tile /* : MonoBehaviour*/
    {
        public delegate Color colorDelegate(out Color _color);
        public colorDelegate colorChange;
        //public bool isTransitioning;

        Vector2 tileSelfIndex;
        public GameObject thisObject;
        public SpriteRenderer frameObj;


        public string name = "";
        //public SpriteRenderer sr;


        public BoardBuilding boardBuilding;
        [SerializeField] Color color;


        List<Color> colors;
        //public bool isMatchedAlready;
        public Color ColorShow
        {
            get { return color; }
            set
            {
                color = value;
                this.thisObject.GetComponent<SpriteRenderer>().DOColor(color, 0.2f);
                //sr.DOColor(value, 0.2f).SetUpdate(true);
            }
        }

        public Tile(GameObject _go, string _name)
        {
            thisObject = _go;
            name = _name;
            frameObj = _go.transform.GetChild(0).GetComponent<SpriteRenderer>();
            Blink();
        }

        public void Blink()
        {
            frameObj.color = new Color(1, 1, 1, 1);
            //frameObj.DORewind(true);
            frameObj.DOFade(0, 0.05f).SetLoops(2, LoopType.Restart).SetUpdate(true).SetAutoKill(false);
        }

        public void BlinkForMatch()
        {
            frameObj.color = new Color(1, 1, 1, 0);
            //frameObj.DORewind(true);
            frameObj.DOFade(1, 1f).SetLoops(3, LoopType.Restart).SetUpdate(true).SetAutoKill(false);
            frameObj.DOGradientColor(boardBuilding.gradient, 1f).SetLoops(3, LoopType.Incremental).SetUpdate(true).SetAutoKill(false).OnComplete(() => Blink());


        }
        public void GatheringIntel(Vector2 _index, BoardBuilding _board, colorDelegate _colorDelegate, List<Color> _colorSet)
        {
            UpdateTileNewIndex(_index);
            boardBuilding = _board;
            colorChange = _colorDelegate;
            colors = _colorSet;
            ApplyIndividualColors();
        }
        public void ApplyIndividualColors()
        {
            ColorShow = (Color)colorChange?.Invoke(out color); // Here applying color using Delegate 
            //ColorShow = sr.color;
            //sr.color = colors[colorIndex];
            if ((int)tileSelfIndex.x > 1 && (int)tileSelfIndex.x < boardBuilding.X_Axis)
                if (this.ColorShow == boardBuilding.tilesInArray[(int)tileSelfIndex.x - 1, (int)tileSelfIndex.y].ColorShow
                    && this.ColorShow == boardBuilding.tilesInArray[(int)tileSelfIndex.x - 2, (int)tileSelfIndex.y].ColorShow)
                {
                    ApplyIndividualColors();
                    //GatheringIntel(tileSelfIndex, boardBuilding, colorChange, colors);
                }
            if ((int)tileSelfIndex.y > 1 && (int)tileSelfIndex.y < boardBuilding.Y_Axis - 1)
                if (this.ColorShow == boardBuilding.tilesInArray[(int)tileSelfIndex.x, (int)tileSelfIndex.y - 1].ColorShow
                    && this.ColorShow == boardBuilding.tilesInArray[(int)tileSelfIndex.x, (int)tileSelfIndex.y - 2].ColorShow)
                {
                    ApplyIndividualColors();
                    //GatheringIntel(tileSelfIndex, boardBuilding, colorChange, colors);
                }
        }


        public Vector2 GetTileSelfIndex()
        {
            return this.tileSelfIndex;
        }
        public void UpdateTileNewIndex(Vector2 _indexVec)
        {
            this.tileSelfIndex = _indexVec;
            thisObject.name = _indexVec.x.ToString() + _indexVec.y.ToString();
        }
        public Color GetColor()
        {
            thisObject.GetComponent<SpriteRenderer>().color = ColorShow;
            return ColorShow;
        }
        public void SetColor(Color _c)
        {
            ColorShow = _c;
        }

    }
}//namespace