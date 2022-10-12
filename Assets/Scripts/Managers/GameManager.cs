using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using RPG.Units.Player;
using System.Linq;

namespace RPG.Managers
{
    public class GameManager : MonoInstaller
    {
        public override void InstallBindings()
        {
            var player = FindObjectOfType<PlayerUnit>();
            Container.BindInstance(player).AsSingle();
        }

        public override void Start()
        {
            base.Start();
            Constants.Construct();

#if UNITY_EDITOR
            GetComponentsInChildren<Transform>(true).First(t => t.name == "Level").gameObject.AddComponent<LevelCheckComponent>();
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Configuration Level")]
        private void ConfigurationLevel()
        {
            var level = GetComponentsInChildren<Transform>(true).First(t => t.name == "Level");

            if (level == null)
                Editor.EditorExtentions.LogError($"Scene does not contain \"Level\" game object", Editor.PriorityMessageType.Critical);
            foreach(var obj in level.GetComponentsInChildren<Transform>(true))
            {
                obj.gameObject.layer = LayerMask.NameToLayer(Constants.ObstacleLayerName);
                obj.gameObject.tag = Constants.FloorTag;
            }
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
#endif



    }
}
