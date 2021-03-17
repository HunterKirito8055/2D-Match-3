using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Launchship2DTiles
{
    public class BoardBuilding : MonoBehaviour
    {
        public UnityEngine.Gradient gradient;
        public GameObject prefabTile;
        public int X_Axis, Y_Axis;    //X_Axis are COlumns   Y_Axis are Rows
        public Tile[,] tilesInArray; //using this in TIle script

        //Tile[,] TilesInArray
        //{
        //    get { return tilesInArray; }
        //    set
        //    {
        //        tilesInArray = value;
        //        foreach (var item in tilesInArray)
        //        {
        //            foreach (var g in tile2D)
        //            {
        //                item.thisObject = g;
        //            }
        //        }
        //    }
        //}
        //public GameObject[,] tile2D;
        static public bool mouseIns = false;

        public float offsetX = 1.2f, offsetY = 1.5f;
        [Header("Set This before Starting")]
        [SerializeField] float timeLimit;


        [Header("Extra Tweak")]
        public float colorChangingTime = 0.5f;
        public float snappingSpeed = 5f;
        static public bool isTransitioning = true;
        public bool isMatched_Horizontal, isMatched_Vertical;

        [Space(15f)]

        [Header("Color Objects", order = 0)]
        public List<Color> currentColorSet;
        public ColorScriptable lowLevelColors, midLevelColors, highLevelColors, fullHighLevelColrs;
        [HideInInspector] public ColorScriptable currentLevelColr;

        [HideInInspector] public string highScoreKey = "";

        [Header("UI")]
        public IntEvent scoreEvent;
        public InputField rowInput, coloumInput;

        public AudioEvent swipeAudioEvent;
        public AudioEvent matchAudioEvent;

        public StringEvent undoCountEvent;

        [Space(15f)]

        List<GameObject> matchedListObjects = new List<GameObject>();
        List<GameObject> gridList;
        Vector2[,] tilesPositionArray;

        Vector2 mousePos;

        Vector2 firstTouch, lastTouch;

        //[Header("Selected Object", order = 0)]
        public GameObject selectedTileObj = null;
        public Tile selectedTile = null;
        public int selectedTileIndex_X, selectedTileIndex_Y;

        //[Header("Target Object", order = 1)]
        public GameObject otherTileObject;



        //CompareClass tweener;
        private void Awake()
        {
            //tweener = FindObjectOfType<CompareClass>();
            //currentLevelColr = FindObjectOfType<ColorScriptable>();
            //TimerScript.maxtime = timeLimit;

        }
        void Start()
        {
            isTransitioning = false;
            TweenManager.instance.ActivatePanel = PanelsEnum.startPanel;
        }
        private void Update()
        {

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetKeyDown(KeyCode.C))
            {
            }
            if (Input.GetMouseButtonDown(0) && !isTransitioning)
            {
                SelectClickedTile();
            }


            // if Swipe distance is less than Swipe Dead 
            // i.e, value given in Inspector from Gesture.cs
            // then we should make selectedTile object to Null, so as to not interfer with swipe checking
            if (Input.GetMouseButtonUp(0) && !isTransitioning)
            {
                lastTouch = mousePos;
                SwipeEnum swipeEnum = Gesture.GetDirection(lastTouch, firstTouch);
                switch (swipeEnum)
                {
                    case SwipeEnum.none:
                        break;
                    case SwipeEnum.up:
                        StartCoroutine(SwitchTile(selectedTileIndex_X, selectedTileIndex_Y, Vector2.up));
                        break;
                    case SwipeEnum.right:
                        StartCoroutine(SwitchTile(selectedTileIndex_X, selectedTileIndex_Y, Vector2.right));
                        break;
                    case SwipeEnum.left:
                        StartCoroutine(SwitchTile(selectedTileIndex_X, selectedTileIndex_Y, Vector2.left));
                        break;
                    case SwipeEnum.down:
                        StartCoroutine(SwitchTile(selectedTileIndex_X, selectedTileIndex_Y, Vector2.down));
                        break;
                    default:
                        break;
                }
            }


        }
        Tile GetTileTypeFromObject(GameObject Obj)
        {
            int a = int.Parse(Obj.name);
            int b = a / 10;
            int c = a % 10;
            return tilesInArray[b, c];
        }
        IEnumerator CreateTiles()
        {
            currentLevelColr = AdjustingColorSetByDifficult();
            ScoreScript.highScoreKey = highScoreKey;
            currentColorSet = currentLevelColr.colors;

            gridList = new List<GameObject>();
            tilesInArray = new Tile[X_Axis, Y_Axis];
            tilesPositionArray = new Vector2[X_Axis, Y_Axis];
            for (int i = 0; i < X_Axis; i++)
            {
                for (int j = 0; j < Y_Axis; j++)
                {
                    GameObject newTile = Instantiate(prefabTile, transform);
                    newTile.name = i.ToString() + j.ToString();
                    newTile.transform.position = new Vector2((i * offsetX), (j * offsetY)); //allign cells with gaps respective to X axis and Y axis
                    newTile.AddComponent<BoxCollider2D>(); // adding box collider at runtime
                    tilesInArray[i, j] = new Tile(newTile, newTile.name);
                    tilesPositionArray[i, j] = newTile.transform.position;
                    tilesInArray[i, j].GatheringIntel(new Vector2(i, j), this, GetRandomColor, currentColorSet);//calling
                    gridList.Add(tilesInArray[i, j].thisObject);
                    yield return new WaitForSeconds(Time.deltaTime * 1 / (X_Axis * Y_Axis));
                }
            }
            print(tilesInArray.Length);
            TimerScript.instance.TimeMode = TimeMode.run;
            TimerScript.instance.StartTimer(timeLimit);
        }

        public void HintBtn()
        {
            StartCoroutine(CheckForAllMatchables());
        }
        public void MainMenuBtn(RectTransform rect)
        {
            TweenManager.instance.ButtonTweenPunch(rect, PanelsEnum.startPanel);
        }
        public void ShopBtn(RectTransform rect)
        {
            TweenManager.instance.ButtonTweenPunch(rect, PanelsEnum.shopPanel);
        }
        public void PlayBtn(RectTransform rect)
        {
            TweenManager.instance.ButtonTweenPunch(rect, PanelsEnum.hudPanel);
            StartCoroutine(CreateTiles());

        }
        public void BackToHudBtn(RectTransform rect)
        {
            TweenManager.instance.ButtonTweenPunch(rect, PanelsEnum.hudPanel);
        }
        public void PresetPanelBtn(RectTransform rect)
        {
            TweenManager.instance.ButtonTweenPunch(rect, PanelsEnum.presetPanel);
        }
        public void BackToStart(RectTransform rect)
        {
            TweenManager.instance.ButtonTweenPunch(rect, PanelsEnum.startPanel);
        }
        public void CustomCreateBtn(RectTransform rect)
        {

            TweenManager.instance.ButtonTweenPunch(rect, PanelsEnum.customCreatePanel);
            //timeScript.TimeMode = TimeMode.pause;
        }
        bool infoShow;
        public GameObject infoShowTextObj;
        public void InfoShowBtn(RectTransform _rect)
        {
            infoShow = !infoShow;
            TweenManager.instance.ButtonTweenPunch(_rect);
            infoShowTextObj.SetActive(infoShow);


        }
        public void CreateGridBtn(RectTransform _rect)
        {
            EraseGrid();
            X_Axis = int.Parse(rowInput.text);
            Y_Axis = int.Parse(coloumInput.text);

            if (rowInput != null && coloumInput != null)
            {
                TweenManager.instance.ButtonTweenPunch(_rect, PanelsEnum.hudPanel);
                if (X_Axis * Y_Axis >= 9)
                {

                    StartCoroutine(CreateTiles());
                    isTransitioning = false;
                }
            }
            //else
            //{
            //    SetPanel(PanelsEnum.hudPanel);
            //    X_Axis = 4; Y_Axis = 4;
            //    StartCoroutine(CreateTiles());
            //    isTransitioning = false;
            //}
        }
        ColorScriptable AdjustingColorSetByDifficult()
        {
            if (X_Axis * Y_Axis <= 100)
            {
                highScoreKey = lowLevelColors.highLevelKey;
                if (X_Axis * Y_Axis <= 80)
                {
                    highScoreKey = lowLevelColors.semiHighLevelKey;
                    if (X_Axis * Y_Axis <= 49)
                    {
                        highScoreKey = lowLevelColors.midLevelKey;
                        if (X_Axis * Y_Axis <= 20)
                        {
                            highScoreKey = lowLevelColors.lowLevelKey;
                            return lowLevelColors;
                        }
                        return midLevelColors;
                    }
                    return highLevelColors;
                }
                return fullHighLevelColrs;
            }
            return null;
        }
        public Color GetRandomColor(out Color color)
        {
            int i = 0;
            i = UnityEngine.Random.Range(0, currentColorSet.Count);
            color = currentColorSet[i];
            return color;
        }
        GameObject GetObjectFromTileArray(Tile tile)
        {
            int c = int.Parse(tile.name);
            int a = c / 10;
            int b = c % 10;
            if (a < X_Axis && b < Y_Axis && a >= 0 && b >= 0)
            {
                return tilesInArray[a, b].thisObject;
            }
            return null;
        }
        void SelectClickedTile()
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                selectedTileObj = hit.transform.gameObject;
                firstTouch = selectedTileObj.transform.position;
                if (selectedTileObj)
                {
                    selectedTile = GetTileTypeFromObject(selectedTileObj);
                    selectedTileIndex_X = (int)selectedTile.GetTileSelfIndex().x;
                    selectedTileIndex_Y = (int)selectedTile.GetTileSelfIndex().y;
                }
            }
        }
        IEnumerator ChangeAndUpdateIndexes(int _x, int _y, Vector2 dir)
        {
            int otherDirX = _x, otherDirY = _y;
            otherDirX += (int)dir.x;
            otherDirY += (int)dir.y;


            //string _thisname = tilesInArray[_x, _y].name;
            //tilesInArray[_x, _y].name = tilesInArray[otherDirX, otherDirY].name;
            //tilesInArray[otherDirX, otherDirY].name = _thisname;

            Tile _thisTile = tilesInArray[_x, _y];
            tilesInArray[_x, _y] = tilesInArray[otherDirX, otherDirY];
            tilesInArray[otherDirX, otherDirY] = _thisTile;

            yield return StartCoroutine(TransitionObjects(tilesInArray[_x, _y].thisObject, tilesInArray[otherDirX, otherDirY].thisObject));

            //print(new Vector2(_x, _y) + "=====" + new Vector2(otherDirX, otherDirY));
            //tilesInArray[_x, _y].UpdateTileNewIndex(new Vector2(otherDirX, otherDirY));
            //tilesInArray[otherDirX, otherDirY].UpdateTileNewIndex(new Vector2(_x, _y));

        }
        public IEnumerator SwitchTile(int _x, int _y, Vector2 dir)
        {
            swipeAudioEvent?.Invoke();
            if (!isTransitioning && selectedTile != null)
            {
                if (_x + (int)dir.x < X_Axis && _y + (int)dir.y < Y_Axis
                &&
               _x >= 0 && _y >= 0)
                {
                    yield return StartCoroutine(ChangeAndUpdateIndexes(_x, _y, dir));

                    // here checks matches and if found, match is set to true

                    /*if match not found, we need to change tiles to original places*/
                    yield return StartCoroutine(CheckMatchingTiles());
                    if (!isMatched_Horizontal && !isMatched_Vertical)//     
                    {
                        yield return new WaitForSeconds(0.2f);
                        yield return StartCoroutine(ChangeAndUpdateIndexes(_x, _y, dir));
                        //isMatched_Vertical = !isMatched_Vertical;
                        //isMatched_Horizontal = !isMatched_Horizontal;
                    }
                    if (isMatched_Horizontal)
                        isMatched_Horizontal = !isMatched_Horizontal;
                    if (isMatched_Vertical)
                        isMatched_Vertical = !isMatched_Vertical;

                }
                selectedTileObj = null;
                selectedTile = null;
                otherTileObject = null;
            }

        }//method


        IEnumerator TransitionObjects(GameObject one, GameObject two/*Vector2 _selectedObjectPosition, Vector2 _otherObjectPosition*/)
        {
            isTransitioning = true;
            TweenManager.instance.ChangePositions(one, two);
            yield return new WaitForSeconds(0.35f);
            isTransitioning = false;

        }//transition
        void UpdateIndexes()
        {
            for (int i = 0; i < X_Axis; i++)
            {
                for (int j = 0; j < Y_Axis; j++)
                {
                    tilesInArray[i, j].UpdateTileNewIndex(new Vector2(i, j));
                }
            }
        }
        public void GenerateRandomColorBtn()
        {
            if (!isTransitioning)
            {
                StartCoroutine(I_RandomColorGenerate(gridList));
            }

        }

        IEnumerator I_RandomColorGenerate(List<GameObject> _gameObjects)
        {
            //if (_gameObjects == matchedListObjects)
            //{
            //    matchAudioEvent?.Invoke();
            //    //registering LISTS OF STEPS OF COLORS FOR UNDO OPERATION HERE
            //    if (selectedTileObj != null)
            //    {
            //        Vector2 selectOb = selectedTileObj .tileSelfIndex;
            //        selectOnesColor = selectedTileObj .ColorShow;
            //        listofSelectAndTargetIndex.Add(selectOb);
            //        listofColors.Add(selectOnesColor);

            //    }
            //    if (otherTileObject != null)
            //    {

            //        Vector2 otherOb = otherTileObject .tileSelfIndex;
            //        targetOnesColor = otherTileObject .ColorShow;
            //        listofSelectAndTargetIndex.Add(otherOb);
            //        listofColors.Add(targetOnesColor);
            //    }


            //    matchedColor = _gameObjects[0].GetComponent<SpriteRenderer>().color;
            //    //listofColors.Add(matchedColor);

            //    print("registered " + i++);
            //    RegisterEntries(listofSelectAndTargetIndex, matchedListIndexes, listofColors); //matchedListIndexes are added in AddToList()
            //}//

            isTransitioning = true;

            if (_gameObjects == matchedListObjects) // This condition is used so as to not change the original grid tiles colors randomly but only for matched list tiles
            {
                UpdateIndexes();
                yield return new WaitForSeconds(Time.deltaTime);
                float colorTime = colorChangingTime;
                while (colorTime > 0)
                {
                    colorTime -= Time.deltaTime;
                    foreach (var g in matchedListObjects)
                    {
                        g.GetComponent<SpriteRenderer>().DOColor(currentColorSet[UnityEngine.Random.Range(0, currentColorSet.Count)], 0.1f);/*.ColorShow = currentColorSet[UnityEngine.Random.Range(0, currentColorSet.Count)];*/
                    }
                    //yield return new WaitForSeconds(Time.deltaTime * (1f / (X_Axis * Y_Axis)));
                }
            }
            foreach (var item in _gameObjects)
            {
                //if the list of objects is not the matched list, then we should normally and fastly change colors
                GetTileTypeFromObject(item).ApplyIndividualColors();
                yield return new WaitForSeconds(Time.deltaTime);
            }
            //here Add score and Clean matchedList
            //listofColors = new List<Color>();
            //listofSelectAndTargetIndex = new List<Vector2>();
            //matchedListIndexes = new List<Vector2>();

            matchedListObjects = new List<GameObject>();
            //StartCoroutine(CheckMatchingTiles());
            //;

            isTransitioning = false;
        }//randomColor



        IEnumerator CheckMatchingTiles()
        {
            for (int i = 0; i < X_Axis; i++)
            {
                for (int j = 0; j < Y_Axis; j++)
                {

                    Tile _currentTile = tilesInArray[i, j];

                    if (i > 0 && i < X_Axis - 1) //Horizontal rows Checking
                    {
                        if (GetTileFromIndex(i + 1, j) != null && GetTileFromIndex(i - 1, j) != null)
                        {
                            Tile _nextTile = GetTileFromIndex(i + 1, j);
                            Tile _previosTile = GetTileFromIndex(i - 1, j);
                            if (_currentTile.GetColor() == _nextTile.GetColor() && _currentTile.GetColor() == _previosTile.GetColor())
                            {
                                //if matched found. we are changing colord for them....
                                AddToList(_nextTile.thisObject);
                                AddToList(_currentTile.thisObject);
                                AddToList(_previosTile.thisObject);

                                isMatched_Horizontal = true;
                            }

                        }
                    }

                    if (j > 0 && j < Y_Axis - 1) //Vertical Columns Checking
                    {
                        if (GetTileFromIndex(i, j + 1) != null && GetTileFromIndex(i, j - 1) != null)
                        {
                            Tile _topTile = tilesInArray[i, j + 1];
                            Tile _bottomTile = tilesInArray[i, j - 1];
                            if (_currentTile.GetColor() == _topTile.GetColor() && _currentTile.GetColor() == _bottomTile.GetColor())
                            {
                                //if matched found. we are changing colord for them....
                                AddToList(_topTile.thisObject);
                                AddToList(_currentTile.thisObject);
                                AddToList(_bottomTile.thisObject);
                                isMatched_Vertical = true;
                            }
                        }
                    }


                }// for j

            }//for i
            //print(matchedListObjects.Count);
            if (matchedListObjects.Count >= 3)
            {
                StartCoroutine(I_RandomColorGenerate(matchedListObjects));
            }
            else
            {
                isMatched_Horizontal = false;
                isMatched_Vertical = false;
            }
            yield return null;
        }//matchTiles method

        void AddToList(GameObject _tileObject)
        {
            //foreach (var item in matchedListObjects)
            //{
            //    if (item.Equals(_tileObject))
            //    {
            //        return;
            //        //StopAllCoroutines();
            //    }
            //}

            matchedListObjects.Add(_tileObject);
            if (matchedListObjects.Count >= 3)
            {
                foreach (var item in matchedListObjects)
                {
                    matchedListIndexes.Add(GetTileTypeFromObject(item).GetTileSelfIndex());
                    listofColors.Add(GetTileTypeFromObject(item).ColorShow);
                }
                /* yield return */
                AddScore(matchedListObjects.Count);
                //return;
            }

        }//addto List

        /// <summary>
        /// checking For Matches and Providing Hints if any matches
        /// </summary>
        /// <returns></returns>

        #region Checking matchable Conditions and Returns hints
        IEnumerator CheckForAllMatchables()//===========Main Scan Call====================//
        {

            for (int i = 0; i < X_Axis; i++)
            {
                for (int j = 0; j < Y_Axis; j++)
                {
                    matchedTileListBlink.Clear();
                    blinkList.Clear();
                    currentTile = tilesInArray[i, j];
                    AddAllToBlinkedList(i, j);
                    yield return StartCoroutine(StandardCheck(i, j));
                    StandardCrossCheck(i, j);
                    yield return StartCoroutine(StandardConsecutiveCheck(i, j));
                    if (matchFound)
                    {
                        matchFound = false;
                        yield break;
                    }
                    yield return new WaitForSeconds(0.5f * (1/(X_Axis * Y_Axis)));
                }
            }
            yield return null;
        }

        IEnumerator StandardCheck(int a, int b)
        {
            if (!matchFound)
            {
                RightCheck(a, b);
                LeftCheck(a, b);
                UpCheck(a, b);
                DownCheck(a, b);
            }
            yield return null;
        }
        int counter = 0;
        IEnumerator StandardConsecutiveCheck(int a, int b)
        {
            if (!matchFound)
            {
                if (GetObjectFromTileIndex(a, b) != null)
                {
                    Right_UpDownCheck(a, b);
                    Left_UpDownCheck(a, b);
                    Down_RightLeftCheck(a, b);
                    Up_RightLeftCheck(a, b);
                }
            }
            yield return null;
        }
        void StandardCrossCheck(int i, int j)
        {
            if (!matchFound)
            {
                Tile _currentTile = tilesInArray[i, j];

                if ((i + 2) < X_Axis)
                {
                    GameObject alternateRightObject = GetObjectFromTileIndex(i + 2, j);
                    if (alternateRightObject != null)
                    {
                        Tile alternateRightTile = GetTileTypeFromObject(alternateRightObject);
                        Tile crossUpTile, crossDownTile;
                        if (_currentTile.ColorShow == alternateRightTile.ColorShow)
                        {
                            if (j + 1 < Y_Axis)
                            {
                                crossUpTile = tilesInArray[i + 1, j + 1];
                                if (_currentTile.ColorShow == crossUpTile.ColorShow)
                                {
                                    BlinkForMatch(crossUpTile, alternateRightTile);
                                    NotificationShow(crossUpTile.name[0] + "," + crossUpTile.name[1] + " Tile Swipe Down has match");
                                    matchFound = true;
                                    return;
                                }
                            }
                            if (j > 0)
                            {
                                crossDownTile = tilesInArray[i + 1, j - 1];
                                if (_currentTile.ColorShow == crossDownTile.ColorShow)
                                {
                                    BlinkForMatch(crossDownTile, alternateRightTile);
                                    NotificationShow(crossDownTile.name[0] + "," + crossDownTile.name[1] + " Tile Swipe Up Has match");
                                    matchFound = true;
                                    return;
                                }
                            }
                        }
                    }
                }
                if ((j + 2) < Y_Axis/* && ( (i - 1) >= 0 || (i + 1) < X_Axis)*/)
                {
                    GameObject alternateUpObject = GetObjectFromTileArray(tilesInArray[i, j + 2]);
                    if (alternateUpObject != null)
                    {
                        Tile alternateUpTile = GetTileTypeFromObject(alternateUpObject);
                        Tile crossRightTile, crossLeftTile;
                        if (_currentTile.ColorShow == alternateUpTile.ColorShow)
                        {
                            if (i + 1 < X_Axis)
                            {
                                crossRightTile = tilesInArray[i + 1, j + 1];
                                if (_currentTile.ColorShow == crossRightTile.ColorShow)
                                {
                                    BlinkForMatch(crossRightTile, alternateUpTile);
                                    NotificationShow(crossRightTile.name[0] + "," + crossRightTile.name[1] + " Tile Swipe Left Has match");
                                    matchFound = true;
                                    return;
                                }
                            }
                            if (i > 0)
                            {
                                crossLeftTile = tilesInArray[i - 1, j + 1];
                                if (_currentTile.ColorShow == crossLeftTile.ColorShow)
                                {
                                    BlinkForMatch(crossLeftTile, alternateUpTile);
                                    NotificationShow(crossLeftTile.name[0] + "," + crossLeftTile.name[1] + " Tile Swipe Right Has match");
                                    matchFound = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }//standard Cross Checks

        void Right_UpDownCheck(int x, int y)
        {
            if (GetObjectFromTileIndex(x + 1, y + 1) != null)
            {
                Tile up1Tile = tilesInArray[x + 1, y + 1];
                if (currentTile.ColorShow == up1Tile.ColorShow)
                {
                    if (GetObjectFromTileIndex(x + 1, y + 2) != null)
                    {
                        Tile up2Tile = tilesInArray[x + 1, y + 2];
                        if (currentTile.ColorShow == up2Tile.ColorShow)
                        {
                            BlinkForMatch(up1Tile, up2Tile);
                            //right swipe has a match to Up
                            NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + " Tile swiping right Has Match In Upside");
                            matchFound = true;
                            return;
                        }
                    }
                }
            }
            if (GetObjectFromTileIndex(x + 1, y - 1) != null)
            {
                Tile down1Tile = tilesInArray[x + 1, y - 1];
                if (currentTile.ColorShow == down1Tile.ColorShow)
                {
                    if (GetObjectFromTileIndex(x + 1, y - 2) != null)
                    {
                        Tile down2Tile = tilesInArray[x + 1, y - 2];
                        if (currentTile.ColorShow == down2Tile.ColorShow)
                        {
                            BlinkForMatch(down1Tile, down2Tile);
                            //right swipe has a match to the up and down
                            NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + " Tile swiping right Has Match To DownSide");
                            matchFound = true;
                            return /*true*/;
                        }
                    }
                }
            }
        }//rightUpDown
        void Left_UpDownCheck(int x, int y)
        {
            if (GetObjectFromTileIndex(x - 1, y + 1) != null)
            {
                Tile up1Tile = tilesInArray[x - 1, y + 1];
                if (currentTile.ColorShow == up1Tile.ColorShow)
                {
                    if (GetObjectFromTileIndex(x - 1, y + 2) != null)
                    {
                        Tile up2Tile = tilesInArray[x - 1, y + 2];
                        if (currentTile.ColorShow == up2Tile.ColorShow)
                        {
                            BlinkForMatch(up1Tile, up2Tile);
                            //left swipe has a match to Up
                            NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + " Tile swiping left Has Match To UpSide");
                            matchFound = true;
                            return /*true*/;
                        }
                    }
                }
            }
            if (GetObjectFromTileIndex(x - 1, y - 1) != null)
            {
                Tile down1Tile = tilesInArray[x - 1, y - 1];
                if (currentTile.ColorShow == down1Tile.ColorShow)
                {
                    if (GetObjectFromTileIndex(x - 1, y - 2) != null)
                    {
                        Tile down2Tile = tilesInArray[x - 1, y - 2];
                        if (currentTile.ColorShow == down2Tile.ColorShow)
                        {
                            BlinkForMatch(down1Tile, down2Tile);
                            //left swipe has a match to the down
                            NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + " Tile swiping left Has Match To DownSide");
                            matchFound = true;
                            return /*true*/;
                        }
                    }
                }
            }
        }//leftUpDown
        void Up_RightLeftCheck(int x, int y)
        {
            if (GetObjectFromTileIndex(x + 1, y + 1) != null)
            {
                Tile up1Tile = tilesInArray[x + 1, y + 1];
                if (currentTile.ColorShow == up1Tile.ColorShow)
                {
                    if (GetObjectFromTileIndex(x + 2, y + 1) != null)
                    {
                        Tile up2Tile = tilesInArray[x + 2, y + 1];
                        if (currentTile.ColorShow == up2Tile.ColorShow)
                        {
                            BlinkForMatch(up1Tile, up2Tile);
                            //up swipe has a match to right
                            NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + " Tile swiping Up Has Match To Right Side");
                            matchFound = true;
                            return /*true*/;
                        }
                    }
                }
            }
            if (GetObjectFromTileIndex(x - 1, y + 1) != null)
            {
                Tile down1Tile = tilesInArray[x - 1, y + 1];
                if (currentTile.ColorShow == down1Tile.ColorShow)
                {
                    if (GetObjectFromTileIndex(x - 2, y + 1) != null)
                    {
                        Tile down2Tile = tilesInArray[x - 2, y + 1];
                        if (currentTile.ColorShow == down2Tile.ColorShow)
                        {
                            BlinkForMatch(down1Tile, down2Tile);
                            //up swipe has a match to the left
                            NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + " Tile swiping Up Has Match To Left Side");
                            matchFound = true;
                            return /*true*/;
                        }
                    }
                }
            }
        }//up_right_left
        void Down_RightLeftCheck(int x, int y)
        {
            if (GetObjectFromTileIndex(x + 1, y - 1) != null)
            {
                Tile up1Tile = tilesInArray[x + 1, y - 1];
                if (currentTile.ColorShow == up1Tile.ColorShow)
                {
                    if (GetObjectFromTileIndex(x + 2, y - 1) != null)
                    {
                        Tile up2Tile = GetTileFromIndex(x + 2, y - 1);
                        if (currentTile.ColorShow == up2Tile.ColorShow)
                        {
                            BlinkForMatch(up1Tile, up2Tile);
                            //down swipe has a match to right
                            NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + " Tile swiping Down Has Match To Right Side");
                            matchFound = true;
                            return /*true*/;
                        }
                    }
                }
            }
            if (GetObjectFromTileIndex(x - 1, y - 1) != null)
            {
                Tile down1Tile = GetTileFromIndex(x - 1, y - 1);
                if (currentTile.ColorShow == down1Tile.ColorShow)
                {
                    if (GetObjectFromTileIndex(x - 2, y - 1) != null)
                    {
                        Tile down2Tile = GetTileFromIndex(x - 2, y - 1);
                        if (currentTile.ColorShow == down2Tile.ColorShow)
                        {
                            BlinkForMatch(down1Tile, down2Tile);
                            //down swipe has a match to the left
                            NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + " Tile swiping Down Has Match To Left Side");
                            matchFound = true;
                            return /*true*/;
                        }
                    }
                }
            }
        }//up_right_left

        void RightCheck(int i, int j)
        {
            counter = 0;
            int tempX, tempY;
            tempX = i + 2; tempY = j;
            while (tempX < X_Axis && (tempX + 1) < X_Axis && counter < 2)
            {
                Tile otherOneTile = GetTileFromIndex(tempX, tempY);
                Tile otherTwoTile = GetTileFromIndex(tempX + 1, tempY);

                if (currentTile.ColorShow == otherOneTile.ColorShow && currentTile.ColorShow == otherTwoTile.ColorShow)
                {
                    BlinkForMatch(otherOneTile, otherTwoTile);
                    NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + "==> Swipe Left");
                    matchFound = true;
                    return;
                }
                counter++;
            }
        }
        void LeftCheck(int i, int j)
        {
            int tempX, tempY;
            counter = 0;
            tempX = i - 2; tempY = j;
            while (tempX >= 0 && (tempX - 1) >= 0 && counter < 2)
            {
                Tile otherOneTile = tilesInArray[tempX, tempY];
                Tile otherTwoTile = tilesInArray[tempX - 1, tempY];
                if (currentTile.ColorShow == otherOneTile.ColorShow && currentTile.ColorShow == otherTwoTile.ColorShow)
                {
                    BlinkForMatch(otherOneTile, otherTwoTile);
                    NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + "==> Swipe Left");
                    matchFound = true;
                    return;
                }
                counter++;
            }
        }
        void UpCheck(int i, int j)
        {
            int tempX, tempY;
            counter = 0;
            tempX = i; tempY = j + 2;
            while (tempY < Y_Axis && (tempY + 1) < Y_Axis && counter < 2)
            {
                Tile otherOneTile = tilesInArray[tempX, tempY];
                Tile otherTwoTile = tilesInArray[tempX, tempY + 1];
                if (currentTile.ColorShow == otherOneTile.ColorShow && currentTile.ColorShow == otherTwoTile.ColorShow)
                {
                    BlinkForMatch(otherOneTile, otherTwoTile);
                    NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + "==> Swipe Up");
                    matchFound = true;
                    return;
                }
                counter++;
            }

        }
        void DownCheck(int i, int j)
        {
            int tempX, tempY;
            counter = 0;
            tempX = i; tempY = j - 2;
            while (tempY >= 0 && (tempY - 1) >= 0 && counter < 2)
            {
                Tile otherOneTile = tilesInArray[tempX, tempY];
                Tile otherTwoTile = tilesInArray[tempX, tempY - 1];
                if (currentTile.ColorShow == otherOneTile.ColorShow && currentTile.ColorShow == otherTwoTile.ColorShow)
                {
                    BlinkForMatch(otherOneTile, otherTwoTile);
                    NotificationShow(currentTile.name[0] + "," + currentTile.name[1] + "==> Swipe Down");
                    matchFound = true;
                    return;
                }
                counter++;
            }
        }


        Tile GetTileFromIndex(int a, int b)
        {
            if (a < X_Axis && b < Y_Axis && a >= 0 && b >= 0)
            {
                return tilesInArray[a, b];
            }
            return null;
        }
        GameObject GetObjectFromTileIndex(int a, int b)
        {
            if (a < X_Axis && b < Y_Axis && a >= 0 && b >= 0)
            {
                return tilesInArray[a, b].thisObject;
            }
            return null;
        }
        bool matchFound = false;
        Tile currentTile = null;



        public List<Tile> matchedTileListBlink = new List<Tile>();
        public List<Tile> blinkList = new List<Tile>();
        void AddAllToBlinkedList(int n, int m)
        {
            blinkList.Add(tilesInArray[n, m]);
            if (n + 2 < X_Axis)
                blinkList.Add(tilesInArray[n + 2, m]);
            if (n + 3 < X_Axis)
                blinkList.Add(tilesInArray[n + 3, m]);
            if (n - 2 >= 0)
                blinkList.Add(tilesInArray[n - 2, m]);
            if (n - 3 >= 0)
                blinkList.Add(tilesInArray[n - 3, m]);

            if (m + 2 < Y_Axis)
                blinkList.Add(tilesInArray[n, m + 2]);
            if (m + 3 < Y_Axis)
                blinkList.Add(tilesInArray[n, m + 3]);

            if (m - 2 >= 0)
                blinkList.Add(tilesInArray[n, m - 2]);
            if (m - 3 >= 0)
                blinkList.Add(tilesInArray[n, m - 3]);

            //cross

            if (n + 1 < X_Axis && m + 1 < Y_Axis)
                blinkList.Add(tilesInArray[n + 1, m + 1]);
            if (n + 1 < X_Axis && m - 1 >= 0)
                blinkList.Add(tilesInArray[n + 1, m - 1]);
            if (n - 1 >= 0 && m + 1 < Y_Axis)
                blinkList.Add(tilesInArray[n - 1, m + 1]);
            if (n - 1 >= 0 && m - 1 >= 0)
                blinkList.Add(tilesInArray[n - 1, m - 1]);


            if (n + 2 < X_Axis && m + 1 < Y_Axis)
                blinkList.Add(tilesInArray[n + 2, m + 1]);
            if (n - 2 >= 0 && m + 1 < Y_Axis)
                blinkList.Add(tilesInArray[n - 2, m + 1]);
            if (n - 2 >= 0 && m - 1 >= 0)
                blinkList.Add(tilesInArray[n - 2, m - 1]);
            if (n + 2 < X_Axis && m - 1 >= 0)
                blinkList.Add(tilesInArray[n + 2, m - 1]);

            if (n + 1 < X_Axis && m + 2 < Y_Axis)
                blinkList.Add(tilesInArray[n + 1, m + 2]);
            if (n - 1 >= 0 && m + 2 < Y_Axis)
                blinkList.Add(tilesInArray[n - 1, m + 2]);
            if (n - 1 >= 0 && m - 2 >= 0)
                blinkList.Add(tilesInArray[n - 1, m - 2]);
            if (n + 1 < X_Axis && m - 2 >= 0)
                blinkList.Add(tilesInArray[n + 1, m - 2]);

            if (blinkList.Count > 0)
            {
                BlinkScan();
            }
        }
        void BlinkScan()
        {
            foreach (var item in blinkList)
            {
                item.Blink();
            }

        }
        void BlinkForMatch(Tile tile1, Tile tile2)
        {
            matchedTileListBlink.Add(currentTile);
            matchedTileListBlink.Add(tile1);
            matchedTileListBlink.Add(tile2);
            foreach (var item in matchedTileListBlink)
            {
                item.BlinkForMatch();
            }
        }
        void NotificationShow(string _string)
        {
            NotificationView.notifInstance.CreateNotification(_string);
        }
        #endregion



        public void CustomPresetBtn(string _preset)
        {
            gridList = new List<GameObject>();
            bool nextNum = false;
            string c1 = string.Empty;
            string c2 = string.Empty;
            for (int i = 0; i < _preset.Length; i++)
            {

                if (char.IsWhiteSpace(_preset[i]))
                {
                    nextNum = true;
                    continue;
                }
                if (Char.IsDigit(_preset[i]))
                {
                    if (!nextNum)
                    {
                        c1 += _preset[i];
                    }
                    else
                    {
                        c2 += _preset[i];
                    }
                }

            }
            X_Axis = int.Parse(c1);
            Y_Axis = int.Parse(c2);
            EraseGrid();
            StartCoroutine(CreateTiles());
            isTransitioning = false;
        }


        void EraseGrid()
        {
            if (gridList != null)
                foreach (var item in gridList)
                {
                    Destroy(item);
                }
            if (tilesInArray != null)
                foreach (var item in tilesInArray)
                {
                    Destroy(item.thisObject);
                }
            gridList = new List<GameObject>();
        }
        //UI

        void AddScore(int count)
        {
            count *= 3;
            scoreEvent?.Invoke(count);
        }


        //Undo

        [Header("UndoOperation", order = 3)]
        public int maxUndoChances = 3;

        public List<Vector2> listofSelectAndTargetIndex = new List<Vector2>();
        public List<Color> listofColors = new List<Color>();

        public List<Vector2> matchedListIndexes = new List<Vector2>();

        Stack<List<Color>> undoStackColor = new Stack<List<Color>>();
        Queue<List<Color>> undoQueueColor = new Queue<List<Color>>();

        Stack<List<Vector2>> undoStackIndex = new Stack<List<Vector2>>();
        Queue<List<Vector2>> undoQueueIndex = new Queue<List<Vector2>>();

        List<Vector2>[] arrayStackIndex = new List<Vector2>[0];
        List<Color>[] arrayStackColor = new List<Color>[0];


        public List<Vector2>[] arrayQueueIndex = new List<Vector2>[0];
        public List<Color>[] arrayQueueColor = new List<Color>[0];

        Color selectOnesColor, targetOnesColor, matchedColor;
        //          1               2               3

        void RegisterEntries(List<Vector2> _selectedAndOther, List<Vector2> _matchLists, List<Color> _colors)
        {
            undoStackColor.Clear();
            undoStackIndex.Clear();

            Array.Reverse(arrayStackIndex);
            Array.Reverse(arrayStackColor);
            //remaining items should be enqued into queue again...
            #region Remaining items in Stack
            foreach (var item in arrayStackColor)
            {
                undoQueueColor.Enqueue(item);
            }
            undoQueueColor.Enqueue(_colors);

            arrayQueueColor = undoQueueColor.ToArray();

            foreach (var item in arrayStackIndex)
            {
                undoQueueIndex.Enqueue(item);
            }
            undoQueueIndex.Enqueue(_matchLists); // 1st
            undoQueueIndex.Enqueue(_selectedAndOther); // 2nd

            arrayQueueIndex = undoQueueIndex.ToArray();

            #endregion

            //from here we need to enque which are registered again
            //if (undoQueueColor.Count > 1 * maxUndoChances)
            //{
            //    undoQueueColor.Dequeue();
            //}




            //if (_selectedAndOther != null)
            //{
            //}
            //if (undoQueueList.Count > 2 * maxUndoChances)
            //{
            //    if (_selectedAndOther != null)
            //    {
            //        undoQueueList.Dequeue();
            //    }
            //    undoQueueList.Dequeue();
            //}
            arrayStackIndex = undoStackIndex.ToArray();
            arrayStackColor = undoStackColor.ToArray();

            undoCountEvent?.Invoke("(" + undoQueueColor.Count.ToString() + ")");
        }//register

        public void UndoCommandBtn()
        {
            StartCoroutine(IUndoCommands());
        }//undoCommand

        Vector2 ReturnUnMatchIndex()
        {
            foreach (var item in matchedListIndexes)
            {
                if (item == listofSelectAndTargetIndex[0])
                {
                    return listofSelectAndTargetIndex[0]/* true*/;
                }
                else
                {
                    return listofSelectAndTargetIndex[1] /*false*/;
                }
            }
            return Vector2.zero;
        }
        Color ReturnUnmatchedColor() // agenda is, we need to return unmatched color to unmatched index from grid to change color
        {
            if (listofColors[2] == listofColors[0])
            {
                return listofColors[1];
            }
            else
            {
                return listofColors[0];
            }
        }
        bool isUndo = false;
        IEnumerator IUndoCommands()
        {
            isUndo = true;
            listofColors = new List<Color>();
            listofSelectAndTargetIndex = new List<Vector2>();
            matchedListIndexes = new List<Vector2>();


            #region Pushing All into Stack for UNDO operations
            foreach (var item in undoQueueColor)
            {
                undoStackColor.Push(item);
            }
            foreach (var item in undoQueueIndex)
            {
                undoStackIndex.Push(item);
            }
            undoQueueIndex.Clear();
            undoQueueColor.Clear();

            arrayStackIndex = undoStackIndex.ToArray();
            arrayStackColor = undoStackColor.ToArray();
            arrayQueueIndex = undoQueueIndex.ToArray();
            arrayQueueColor = undoQueueColor.ToArray();
            #endregion


            if (undoStackColor.Count < 1)
            {
                yield break;
            }

            /* only 2 objects */
            listofSelectAndTargetIndex = undoStackIndex.Pop(); // 2nd frm queue

            /* Entire Matched Objects, i.e., equal or more than 3 at minimun*/
            matchedListIndexes = undoStackIndex.Pop(); // 1st from queue

            listofColors = undoStackColor.Pop();

            //if (listofColors.Count == matchedListIndexes.Count)
            //    for (int i = 0; i < matchedListIndexes.Count; i++)
            //    {
            //        tilesInArray[(int)matchedListIndexes[i].x, (int)matchedListIndexes[i].y] .ColorShow = listofColors[i];
            //    }
            selectOnesColor = listofColors[0];
            targetOnesColor = listofColors[1];
            matchedColor = listofColors[2];

            int mCount = matchedListIndexes.Count;
            int cCount = listofColors.Count;

            for (int i = 0, c = 0; i < mCount && c < cCount; i++)
            {
                tilesInArray[(int)matchedListIndexes[i].x, (int)matchedListIndexes[i].y].ColorShow = listofColors[c];
            }
            foreach (var item in matchedListIndexes)
            {
                tilesInArray[(int)item.x, (int)item.y].ColorShow = matchedColor;
            }

            //foreach (var item in matchedListIndexes)//applying matched color with same
            //{
            //    tilesInArray[(int)item.x, (int)item.y] .ColorShow = matchedColor;
            //    foreach (var tar in listofSelectAndTargetIndex)
            //    {
            //        if (tar == item)
            //        {
            //            tilesInArray[(int)item.x, (int)item.y] .ColorShow = ReturnUnmatchedColor();
            //        }
            //    }
            //}

            tilesInArray[(int)listofSelectAndTargetIndex[0].x, (int)listofSelectAndTargetIndex[0].y].ColorShow
                = listofColors[listofColors.Count - 1];
            tilesInArray[(int)listofSelectAndTargetIndex[1].x, (int)listofSelectAndTargetIndex[1].y].ColorShow
                = listofColors[listofColors.Count - 2];

            //Vector2 unMatchedTileIndex = ReturnUnMatchIndex();
            //tilesInArray[(int)unMatchedTileIndex.x, (int)unMatchedTileIndex.y] .ColorShow = matchedColor;

            arrayStackIndex = undoStackIndex.ToArray();
            arrayStackColor = undoStackColor.ToArray();
            arrayQueueIndex = undoQueueIndex.ToArray();
            arrayQueueColor = undoQueueColor.ToArray();
            //Reducing Score here
            AddScore(-matchedListIndexes.Count);

            undoCountEvent?.Invoke("(" + undoQueueColor.Count.ToString() + ")");
            yield return null;
            isUndo = false;
        }


    }//class

}//namespace











//otherTileObject.gameObject.name = tilesInArray[(int) updatedIndex.x, (int) updatedIndex.y].ToString();
//selectedTileComp.gameObject.name = tilesInArray[selectedTileIndex_X + (int) dir.x, selectedTileIndex_Y + (int) dir.y].ToString();


//print(selectedTile_X + "," + selectedTile_Y + "hit");
//clickedTileVec = currentTile.tileSelfPosition;
//selectedTileIndex_Y = (int) selectedTileComp.GetTileSelfIndex().y;
//selectedTileIndex_X = (int) selectedTileComp.GetTileSelfIndex().x;

//selectedTileObj.position = otherPos;
//otherTileObject.transform.position = firstTouch;
//otherTileObject.transform.position = tilesPositionArray[(int) updatedIndex.x, (int) updatedIndex.y];
//selectedTileObj.position = tilesPositionArray[selectedTileIndex_X + (int) dir.x, selectedTileIndex_Y + (int) dir.y];