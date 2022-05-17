using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamestureTask
{
    public class ListRow : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI imageNameTextMesh;
        [SerializeField] TextMeshProUGUI timeFromCreationTextMesh;

        public Vector2 AnchoredPosition => _rectTransform.anchoredPosition;
        public float Height => _height;

        RectTransform _rectTransform;
        AspectRatioFitter _imageAspectRatioFitter;
        ImageInfo _imageInfo;
        bool _hasImageInfo;
        bool _hasImage;
        float _topBorder;
        float _bottomBorder;
        float _height;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _imageAspectRatioFitter = image.GetComponent<AspectRatioFitter>();
            var verticalList = _rectTransform.GetComponentInParent<VerticalList>();
            _height = _rectTransform.rect.height;
            _topBorder = verticalList.TopBorder + _height;
            _bottomBorder = verticalList.BottomBorder - _height;
        }

        void Update()
        {
            if (!_hasImageInfo || !_imageInfo.IsImageLoaded || _hasImage) return;
            SetImage(_imageInfo.Image);
        }

        void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
            _imageAspectRatioFitter.aspectRatio = sprite.rect.width / sprite.rect.height;
            _hasImage = true;
            image.enabled = true;
        }

        public void Display(int i, int j, ImageInfo imageInfo)
        {
            _hasImageInfo = true;
            _hasImage = false;
            imageNameTextMesh.text = imageInfo.ImageName;
            timeFromCreationTextMesh.text = imageInfo.GetTimeSinceWasCreated().ToString("g");
            _imageInfo = imageInfo;
            image.enabled = _imageInfo.IsImageLoaded;
        }

        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            _rectTransform.anchoredPosition = anchoredPosition;
            if (IsInView())
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        bool IsInView()
        {
            var yPosition = AnchoredPosition.y;
            return yPosition < _topBorder && yPosition > _bottomBorder;
        }
        
        public void TurnOn() => gameObject.SetActive(true);

        public void TurnOff() => gameObject.SetActive(false);
    }
}