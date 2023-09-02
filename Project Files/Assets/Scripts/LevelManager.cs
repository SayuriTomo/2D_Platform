using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public PlayerController ThePlayer;

    // Start is called before the first frame update
    void Start()
    {
        ThePlayer = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        StartCoroutine("RespawnCo");
    }

    public IEnumerator RespawnCo()
    {
        yield return new WaitForSeconds(1);

        ThePlayer.gameObject.SetActive(false);

        yield return new WaitForSeconds(2);

        ThePlayer.transform.position = ThePlayer.RespawnPosition;

        ThePlayer.bIsDead = false;

        ThePlayer.gameObject.SetActive(true);
    }
}
