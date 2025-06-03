using UnityEngine;

public class TreeTest : MonoBehaviour
{
    GameObject playerMovement = null;

    void Update()
    {
        playerMovement ??= GameObject.FindWithTag("Player");
        if (Vector3.Distance(playerMovement.transform.position, transform.position) < 1f && Input.GetKeyDown(KeyCode.Backspace))
        {
            gameObject.SetActive(false);
            GeneratorManager.Instance.KillForestAt(transform.position.tofloat3().xz);
        }
    }
}
