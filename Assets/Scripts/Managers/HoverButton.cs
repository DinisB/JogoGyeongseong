using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace.Managers
{
    public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField] private float increasedSize;
        [SerializeField] private Image image;
        [SerializeField] private Color baseColor;
        [SerializeField] private Color hoverColor;

        private void Start()
        {
            image.color = baseColor;
        }

        public bool Enabled { get; set; } = true;

        private Vector3 _startSize;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Enabled) return;
            _startSize = transform.localScale;
            image.color = hoverColor;
            transform.localScale = _startSize + (_startSize * increasedSize);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Enabled) return;
            transform.localScale = _startSize;
            image.color = baseColor;
        }
        
    }
}