using Core.Patterns;
using Core.TurnSystem;
using UnityEngine;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        


        void Start()
        {
            Debug.Log($"GameManager Start!, Current Turn:{TurnManager.Instance.CurrentTurn} ");
        }


        
        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 150, 30), "Click Me (IMGUI)"))
            {
            }
        }
        
    }
}