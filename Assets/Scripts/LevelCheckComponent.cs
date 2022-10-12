using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace RPG
{
    public class LevelCheckComponent : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var objects = GetComponentsInChildren<Transform>(true);
            var str = new StringBuilder();

            foreach (var obj in objects)
            {
                var check1 = !obj.CompareTag(Constants.FloorTag);
                var check2 = obj.gameObject.layer != Constants.ObstacleLayerInt;
                if (check1 && check2)
                    str.Append($"{obj.name} does not have the correct tag and layer\n");
                else if(check1)
                    str.Append($"{obj.name} does not have the correct tag\n");
                else if(check2)
                    str.Append($"{obj.name} does not have the correct layer\n");
            }

            if (str.Length > 0)
                Editor.EditorExtentions.LogError(str.ToString(), Editor.PriorityMessageType.Critical);
        }

    }
}
