using UnityEngine;
using UnityEngine.UI;

namespace Script.Controller
{
    [RequireComponent(typeof(Button))]
    public class MoveButton : MonoBehaviour
    {
        [SerializeField] private move _direction;
        
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener( ()=> RealizationBox.Instance.gameController.Move(_direction));
        }
    }
}