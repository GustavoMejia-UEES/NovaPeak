using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Propiedades públicas para que otros scripts accedan a los inputs
    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool ShootTriggered { get; private set; }
    public bool MeleeTriggered { get; private set; }
    public bool ParryTriggered { get; private set; }

    // Callbacks del Input System (estos nombres deben coincidir con tus Acciones)
    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        // Solo registra si el botón fue presionado en este frame
        if (value.isPressed)
            JumpTriggered = true;
    }

    public void OnShoot(InputValue value)
    {
        if (value.isPressed)
            ShootTriggered = true;
    }

    public void OnMelee(InputValue value)
    {
        if (value.isPressed)
            MeleeTriggered = true;
    }

    public void OnParry(InputValue value)
    {
        if (value.isPressed)
            ParryTriggered = true;
    }

    // Métodos para resetear los triggers después de que han sido consumidos por otros scripts
    public void ResetJumpTrigger() => JumpTriggered = false;
    public void ResetShootTrigger() => ShootTriggered = false;
    public void ResetMeleeTrigger() => MeleeTriggered = false;
    public void ResetParryTrigger() => ParryTriggered = false;

    // Considera resetear los triggers al final de cada frame si prefieres
    // que se limpien automáticamente, aunque da menos control.
    // void LateUpdate()
    // {
    //     ResetJumpTrigger();
    //     ResetShootTrigger();
    //     ResetMeleeTrigger();
    //     ResetParryTrigger();
    // }
}