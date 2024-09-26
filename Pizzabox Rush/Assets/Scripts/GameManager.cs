using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PizzaBox PizzaBoxControl;
    public SceneManagerSO SceneManager;

    public Action OnGameStart;

    [HideInInspector] public bool HasGameStarted = false;

    [SerializeField] private GameObject mainMenu, hud;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && OnGameStart != null)
        {
            OnGameStart();

            HasGameStarted = true;

            mainMenu.SetActive(false);
            hud.SetActive(true);
        }
    }
}
