using UnityEngine;
using UnityEngine.UI;
namespace CircleButtonInventory
{
    public class CustomShapeButton : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}