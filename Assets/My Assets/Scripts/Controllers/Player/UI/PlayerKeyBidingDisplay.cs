using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyBidingDisplay : MonoBehaviour, IPlayerComponent
{
    [SerializeField] private List<KeybindUI> _keybinds = new List<KeybindUI>();
    [SerializeField] private List<KeyBindSprite> _keyBindSprites = new List<KeyBindSprite>();
    private PlayerInputsHandler _playerInputsHandler;
    public void InitializePlayerComponent(PlayerComponentsRefrences playerComponents)
    {
        _playerInputsHandler = playerComponents.GetPlayerInputsHandler;
        SetupKeyBinds();
    }

    private void SetupKeyBinds()
    {
        foreach (KeybindUI keybind in _keybinds)
        {
            string key = _playerInputsHandler.GetBinding(keybind.GetInputActionName);
            Sprite s = TryGetKeyBindSprite(key);
            if (s == null)
            {
                keybind.SetText(key);
            }
            else
            {
                keybind.SetImage(s);
            }
        }
    }

    public Sprite TryGetKeyBindSprite(string key)
    {
        foreach (KeyBindSprite item in _keyBindSprites)
        {
            if (item.Key == key) return item.Sprite;
        }
        return null;
    }

    [System.Serializable]
    public class KeyBindSprite
    {
        public string Key;
        public Sprite Sprite;
    }
}
