using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Launchship2DTiles
{
    public class CommandPattern : MonoBehaviour
    {
        private readonly string commandName;
     
        public static List<CommandPattern> commandPatternsList = new List<CommandPattern>();
        
        Stack<CommandPattern> undoStack = new Stack<CommandPattern>();
       
        public delegate GameObject ExecuteCallback(Gesture gesture);
        public ExecuteCallback Execute /*{ get; private set; }*/;
        Coroutine executeUndo;
       
        public CommandPattern(ExecuteCallback executeMethod, string name)
        {
            Execute = executeMethod;
            commandName = name;
        }



        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.U))
            {

            }
        }
        //public void CheckForCommands()
        //{
        //    var _command = Gesture.Cass();
        //    if (_command != null && executeUndo == null)
        //    {
        //        AddToCommands(_command);
        //    }
        //}
        //public void AddToCommands(CommandPattern commandPatterns)
        //{
        //    commandPatternsList.Add(commandPatterns);
        //}

        //void ExecuteUndoF()
        //{
        //    if(executeUndo!=null)
        //    {
        //        return;
        //    }
        //    executeUndo = StartCoroutine(ExecutingUndo_I());
        //}
        //IEnumerator ExecutingUndo_I()
        //{
        //    for(int i=0;i<commandPatternsList.Count;i++)
        //    {
        //        var _command = commandPatternsList[i];
        //        //_command.
        //    }
        //    yield return null;
        //}
      
    }//class
}//namespace