using UnityEngine;

public class Goal : MonoBehaviour
{
    public void CheckPlayerPosition(Vector3 playerPosition)
    {
        if (Vector3.Distance(playerPosition, transform.position) < 0.1f)
        {
            GameManager.Instance.LevelComplete();
        }
    }
}
