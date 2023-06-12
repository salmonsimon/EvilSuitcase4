using Microlight.MicroBar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private MicroBar playerHealthBar;

    private HealthManager playerHealthManager;

    private void OnEnable()
    {
        if (playerHealthManager)
        {
            playerHealthManager.OnCurrentHealthChange += Damaged;
            playerHealthBar.Initialize(playerHealthManager.MaxHitPoints);
        }
    }

    private void OnDisable()
    {
        if (playerHealthManager)
            playerHealthManager.OnCurrentHealthChange -= Damaged;
    }

    private void Start()
    {
        playerHealthManager = GameManager.instance.GetPlayer().GetComponent<HealthManager>();

        playerHealthBar.Initialize(playerHealthManager.MaxHitPoints);

        playerHealthManager.OnCurrentHealthChange += Damaged;
    }

    private void Damaged()
    {
        playerHealthBar.UpdateHealthBar(playerHealthManager.CurrentHitPoints);
    }
}
