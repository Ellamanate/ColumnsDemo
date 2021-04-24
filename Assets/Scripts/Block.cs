using UnityEngine;
using System.Linq;

public class Block : MonoBehaviour
{
    public int X { get => x; }
    public int Y { get => y; }
    public bool IsTarget { get => isTarget; }
    public bool IsLock { get => locker.IsLock; }
    public BlockColor BlockColor { get => blockColor; }
    public float ClipDuration { get => animator.runtimeAnimatorController.animationClips[0].averageDuration; }

    [SerializeField] private int x;
    [SerializeField] private int y;
    [SerializeField] private bool isTarget;
    [SerializeField] private BlockColor blockColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private BlockLocker locker;
    [SerializeField] private GameObject target;

    public static Block CreateBlock(Block block, int x, int y, bool isTarget, bool isLock, BlockColor blockColor, Transform parent)
    {
        Block createdBlock = Instantiate(block, parent);

        if (ToolBox.GetData(out SceneData sceneData))
        {
            createdBlock.blockColor = sceneData.Colors.ToPossibleColor(blockColor);
            createdBlock.spriteRenderer.color = createdBlock.blockColor.Color;
            createdBlock.isTarget = isTarget;

            if (isLock)
                createdBlock.Lock();

            if (isTarget)
                GameObject.Instantiate(createdBlock.target, createdBlock.transform);
        }

        createdBlock.Move(x, y);

        return createdBlock;
    }

    public static void RemoveBlock(Block block)
    {
        block.animator.SetBool("Destroy", true);
        Destroy(block.gameObject, block.ClipDuration);
    }

    public static void RemoveBlock(Block block, float speed)
    {
        block.animator.speed = speed;
        block.animator.SetBool("Destroy", true);
        Destroy(block.gameObject, block.ClipDuration);
    }

    public void Move(int x, int y)
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector3(x, y, 0);
    }

    public void UnlockAnimation(string key) => locker.UnlockAnimation(key);
    public void Unlock() => locker.Unlock();
    public void LockAnimation(string key) => locker.LockAnimation(key);
    public void Lock() => locker.Lock();
}