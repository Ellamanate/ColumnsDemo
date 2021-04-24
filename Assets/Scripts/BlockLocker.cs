using UnityEditor;
using UnityEngine;


public class BlockLocker : MonoBehaviour
{
    public bool IsLock => isLock;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite[] locked;
    [SerializeField] private bool isLock;

    public void UnlockAnimation(string key) => animator.SetBool(key, false); 

    public void Unlock() 
    {
        spriteRenderer.sprite = null;
        isLock = false; 
    }

    public void LockAnimation(string key) => animator.SetBool(key, true);

    public void Lock() => isLock = true;

    public void OnLockAnimationEnd()
    {
        if (locked.Length > 0)
            spriteRenderer.sprite = locked[Random.Range(0, locked.Length)];
    }

    public void OnUnlockAnimationEnd() => spriteRenderer.sprite = null;
}