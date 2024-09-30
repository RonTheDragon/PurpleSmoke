using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour, IPlayerComponent
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _chargeBar;
    [SerializeField] private Image _acidationBar;
    [SerializeField] private GameObject _pressSpaceToGetUp;
    [SerializeField] private TMP_Text _respawnCountDown;
    [SerializeField] private GameObject _acidCrosshair;

    private PlayerComponentsRefrences _playerComponents;
    private PlayerHealth _playerHealth;
    private PlayerCombatSystem _playerCombatSystem;
    private PlayerKnockout _playerKnockout;
    private PlayerDeath _playerDeath;
    private PlayerAcidation _playerAcidation;

    private Color _green = Color.green * Color.gray;

    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerComponents = playerComponents;

        
        _playerHealth = _playerComponents.GetPlayerHealth;
        _playerCombatSystem = _playerComponents.GetPlayerCombatSystem;
        _playerKnockout = _playerComponents.GetPlayerKnockout;
        _playerDeath = _playerComponents.GetPlayerDeath;
        _playerAcidation = _playerComponents.GetPlayerAcidation;

        _playerHealth.OnPlayerHealthChange += UpdateHealthUI;
        _playerCombatSystem.OnChargeChange += UpdateChargeUI;
        _playerKnockout.OnCanGetUp += PressSpaceToGetUp;
        _playerDeath.OnRespawnCountdown += UpdateRespawnTime;
        _playerAcidation.OnAcidationChange += UpdateAcidUI;
    }

    private void UpdateHealthUI(float amount)
    {
        _healthBar.fillAmount = amount;
        ColorBar(_healthBar, amount, _green * Color.gray, Color.yellow, Color.red);
    }

    private void UpdateChargeUI(float amount)
    {
        _chargeBar.fillAmount = amount;
        ColorBar(_chargeBar, amount, Color.red, Color.yellow, Color.white);
    }

    private void UpdateAcidUI(float amount)
    {
        _acidationBar.fillAmount = amount;
    }

    private void ColorBar(Image bar, float amount, Color full, Color mid, Color empty)
    {
        if (amount > 0.5f)
        {
            bar.color = Color.Lerp(mid, full, (amount - 0.5f) * 2);
        }
        else
        {
            bar.color = Color.Lerp(empty, mid, amount * 2);
        }
    }

    private void PressSpaceToGetUp(bool show)
    {
        _pressSpaceToGetUp.SetActive(show);
    }

    private void UpdateRespawnTime(int time)
    {
        switch (time)
        {
            case 0:
                _respawnCountDown.text = "Press [Jump] To Respawn";
                break;
            case -1:
                _respawnCountDown.text = string.Empty;
                break;
            default:
                _respawnCountDown.text = time.ToString();
                break;
        }

        PressSpaceToGetUp(false);
    }

    public void SetAcidCrosshair(bool b) => _acidCrosshair.SetActive(b);

}
