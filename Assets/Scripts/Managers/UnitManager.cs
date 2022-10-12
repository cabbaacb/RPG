using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Units.Player;
using RPG.Units.NPC;
using Zenject;
using RPG.Units;

namespace RPG.Managers
{
    public class UnitManager : MonoBehaviour
    {
        private LinkedList<NPCUnit> _bots = new LinkedList<NPCUnit>();
        [SerializeField]
        private Transform _npcPool;

        [Inject]
        public PlayerUnit GetPlayer { get; private set; }
        public IReadOnlyCollection<NPCUnit> GetNPCs => _bots;

        private void Start()
        {
            var units = _npcPool.GetComponentsInChildren<Unit>();

#if UNITY_EDITOR
            foreach(var npc in units)
            {
                var bot = npc as NPCUnit;
                if (bot == null)
                    Editor.EditorExtentions.LogError($"the unit {npc.name} is not a NPC");
            }
#endif
            foreach (NPCUnit bot in units)
                _bots.AddLast(bot);


#if UNITY_EDITOR
            if (FindObjectsOfType<NPCUnit>().Length != _bots.Count)
                Editor.EditorExtentions.LogError($"NPC must be in a <b>{_npcPool.name}</b> pool", 
                    Editor.PriorityMessageType.Critical);
#endif
        }
    }
}
