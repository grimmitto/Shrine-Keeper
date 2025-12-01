using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Scene Player Objects (NOT Prefabs!)")]
    public GameObject malePlayerInScene;
    public GameObject femalePlayerInScene;

    [Header("Spawn Point For New Game")]
    public Transform newGameSpawnPoint;

    private GameObject activePlayer;

    public GameObject SpawnPlayer(SaveData save)
    {
        // Pick which in-scene object to activate
        GameObject chosen =
            (save.playerGender == PlayerGender.Male)
            ? malePlayerInScene
            : femalePlayerInScene;

        GameObject other =
            (save.playerGender == PlayerGender.Male)
            ? femalePlayerInScene
            : malePlayerInScene;

        // Activate correct player, disable the wrong one
        chosen.SetActive(true);
        other.SetActive(false);

        activePlayer = chosen;

        // Determine if this is a brand new game
        bool isNewGame =
            save.lastScene == GameManager.Instance.newGameSceneName &&
            !SaveUtility.SaveExists(GameManager.Instance.currentSlot);

        Vector3 pos;
        Quaternion rot;

        if (isNewGame)
        {
            pos = newGameSpawnPoint.position;
            rot = newGameSpawnPoint.rotation;

            
        }
        else
        {
            pos = new Vector3(
                save.playerPosition[0],
                save.playerPosition[1],
                save.playerPosition[2]
            );

            rot = new Quaternion(
                save.playerRotation[0],
                save.playerRotation[1],
                save.playerRotation[2],
                save.playerRotation[3]
            );

            // 🚨 If save is 0,0,0 (bad save), fall back
            if (pos == Vector3.zero)
            {
                pos = newGameSpawnPoint.position;
                rot = newGameSpawnPoint.rotation;
            }
        }

        // Place the player safely
        var cc = activePlayer.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        activePlayer.transform.SetPositionAndRotation(pos, rot);

        if (cc) cc.enabled = true;

        activePlayer.tag = "Player";
        return activePlayer;
    }
}
