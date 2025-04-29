using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    protected static CharacterManager _CM {  get { return CharacterManager.instance; } }
    protected static GameManager _GM { get { return GameManager.instance; } }
    protected static EnemyManager _EM { get { return EnemyManager.instance; } }
}
