using System;
using UnityEngine;

public abstract class ComponentsRefrences : MonoBehaviour
{
    public Action OnUpdate;
    [SerializeField] protected CharacterController _characterController;

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }

    protected abstract void InitializeComponents();

    public CharacterController GetCharacterController => _characterController;
}
