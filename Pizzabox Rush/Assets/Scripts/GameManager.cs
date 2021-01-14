using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PizzaBox PizzaBoxControl;
    public SceneManagerSO SceneManager;

    private void Awake()
    {
        Instance = this;
    }
}
