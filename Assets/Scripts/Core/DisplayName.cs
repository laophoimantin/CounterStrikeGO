using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DisplayName : MonoBehaviour
    {
        
        [SerializeField] private string _displayName;
        
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnGUI()
        {
            if (!Camera.main) return;
        
            var worldPoint = transform.position + Vector3.up * 2f;
            var p = Camera.main.WorldToScreenPoint(worldPoint);
            
            if (p.z > 0)
            {
                string label = _displayName;
                var size = GUI.skin.label.CalcSize(new GUIContent(label));
                GUI.Label(new Rect(p.x - size.x / 2f, Screen.height - p.y - size.y, size.x, size.y), label);
            }
        }
    }
}