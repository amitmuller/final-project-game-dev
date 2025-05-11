using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using CodeMonkey.MonoBehaviours;

public class GameHandler_SetupEnemyViewCone : MonoBehaviour {

    [SerializeField] private    CodeMonkey.MonoBehaviours.CameraFollow cameraFollow;
    [SerializeField] private Player player;

    private void Start() {
        cameraFollow.Setup(GetCameraPosition, () => 80f, true, true);

        //FunctionPeriodic.Create(SpawnEnemy, 2f);
        //SpawnEnemy();
        //EnemyHandler.Create(new Vector3(20, 0));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    private Vector3 GetCameraPosition() {
        return player.GetPosition();
    }

    private void SpawnEnemy() {
        Vector3 spawnPosition = player.GetPosition() + UtilsClass.GetRandomDir() * 60f;// 100f;
        EnemyHandler enemyHandler = EnemyHandler.Create(spawnPosition);
        enemyHandler.SetGetTarget(() => player);
    }

}
