using UnityEngine;
using UnityEngine.UI;


public class TargetActionUI : MonoBehaviour
{
    [SerializeField] private Image enabledImage;
    [SerializeField] private Image disableImage;

    public void Enable()
    {
        enabledImage.gameObject.SetActive(true);
        disableImage.gameObject.SetActive(false);
    }

    public void Disable()
    {
        enabledImage.gameObject.SetActive(false);
        disableImage.gameObject.SetActive(true);
    }
}
