using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.NPC
{
    public class NPCUnit : Unit
    {       
        protected override void Start()
        {

        }

        protected override void Update()
        {
            
        }

        protected override void FindNewTarget()
        {
            var player = _unitManager.GetPlayer;
            Target = player;
        }

    }
}
