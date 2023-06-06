using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minimalist.Bar.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageBhv : EmptyBhv
    {
        // Public properties
        public Color Color
        {
            get
            {
                return Image.color;
            }

            set
            {
                Image.color = value;
            }
        }

        // Protected properties
        protected Image Image => _image == null ? GetComponent<Image>() : _image;

        // Private fields
        private Image _image;

        private void Awake()
        {
            _image = Image;
        }
    }
}