using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using RPG.Units.Player;

namespace RPG.Managers
{
    public class GameManager : MonoInstaller
    {
        public override void InstallBindings()
        {
            var player = FindObjectOfType<PlayerUnit>();
            Container.BindInstance(player).AsSingle();
        }
    }
}
