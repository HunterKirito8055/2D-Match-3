using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Launchship2DTiles
{
    public class UndoClass : MonoBehaviour
    {
        List<Vector2> listofSelectAndTargetIndex = new List<Vector2>();
        List<Color> listofColors = new List<Color>();

        List<Vector2> matchedListIndexes = new List<Vector2>();

        Stack<List<Color>> undoStackColor = new Stack<List<Color>>();
        Queue<List<Color>> undoQueueColor = new Queue<List<Color>>();

        Stack<List<Vector2>> undoStackList = new Stack<List<Vector2>>();
        Queue<List<Vector2>> undoQueueList = new Queue<List<Vector2>>();

        Color selectOnesColor, targetOnesColor, matchedColor;
        int maxUndoChances;

        public UndoClass(List<Vector2> _selectedAndOtherIndexes, List<Vector2> _matchListsIndexes, List<Color> _colors, int _maxUndoChances)
        {
            listofSelectAndTargetIndex = _selectedAndOtherIndexes;
            matchedListIndexes = _matchListsIndexes;
            listofColors = _colors;
            maxUndoChances = _maxUndoChances;
            RegisterEntries();
        }
        public void RegisterEntries()
        {
            undoQueueColor.Enqueue(listofColors);
            if (undoQueueColor.Count > 1 * maxUndoChances)
            {
                undoStackColor.Clear();
                undoQueueColor.Dequeue();
            }

            foreach (var item in undoQueueColor)
            {
                undoStackColor.Push(item);
            }

            undoQueueList.Enqueue(matchedListIndexes); // 1st
            undoQueueList.Enqueue(listofSelectAndTargetIndex); // 2nd
            if (undoQueueList.Count > 2 * maxUndoChances)
            {
                undoStackList.Clear();
                undoQueueList.Dequeue();
            }
            foreach (var item in undoQueueList)
            {
                undoStackList.Push(item);
            }
        }

        public void UndoEntries()
        {
            if (undoStackList.Count < 2 * maxUndoChances)
            {
                return;
            }

        }

        public void GetListForIndex(List<Vector2> _vecList)
        {

        }
        public void GetListForColor(List<Color> _colorList)
        {

        }
    }
}