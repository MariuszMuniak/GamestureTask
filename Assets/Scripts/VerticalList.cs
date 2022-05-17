using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace GamestureTask
{
    public class VerticalList : MonoBehaviour
    {
        [SerializeField] int totalNumberOfRecords = 10;
        [SerializeField] ListRow listRecordPrefab;
        [SerializeField] float space = 10f;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] int preLoadedRecords = 6;
        [SerializeField] ImageLoader imageLoader;

        public float TopBorder => _topBorder;
        public float BottomBorder => _bottomBorder;

        RectTransform _scrollRectRectTransform;
        List<ListRow> _records = new();
        float _prevVerticalScrollValue;
        float _topBorder;
        float _bottomBorder;
        float _recordHeight;
        int _firstVisibleRecordIndex;
        int _numberOfVisibleRecords;

        ObjectPool<GameObject> _recordsPool;

        void Awake()
        {
            _recordHeight = listRecordPrefab.GetComponent<RectTransform>().rect.height;
            _scrollRectRectTransform = scrollRect.GetComponent<RectTransform>();
        }

        void Start()
        {
            SetUpList();
            InitializeList();
        }

        void SetUpList()
        {
            totalNumberOfRecords = imageLoader.NumberOfImages;
            var contentSize = scrollRect.content.sizeDelta;
            contentSize.y = totalNumberOfRecords * (_recordHeight + space);
            scrollRect.content.sizeDelta = contentSize;
            scrollRect.verticalNormalizedPosition = 1f;
            scrollRect.onValueChanged.AddListener(ScrollRectOnValueChanged);
            _prevVerticalScrollValue = scrollRect.verticalNormalizedPosition;
            _topBorder = _scrollRectRectTransform.rect.height / 2f;
            _bottomBorder = _topBorder * -1;
        }

        void InitializeList()
        {
            var numberOfRecordsToCrete = NumberOfRecordsToCrete(imageLoader.NumberOfImages);
            var createdRecords = CreateRecords(numberOfRecordsToCrete);
            _records.AddRange(createdRecords);
            ResetRecordsPosition();
            _firstVisibleRecordIndex = 0;
            _numberOfVisibleRecords = NumberOfVisibleRecords();
            LoadImages();
            Display(_firstVisibleRecordIndex);
        }

        int NumberOfRecordsToCrete(int dataRecordCount)
        {
            if (dataRecordCount < 0) return 0;
            var numberOfRecords = NumberOfVisibleRecords();
            numberOfRecords += preLoadedRecords;
            numberOfRecords = Mathf.Clamp(numberOfRecords, 0, dataRecordCount);
            return numberOfRecords;
        }

        int NumberOfVisibleRecords() => Mathf.CeilToInt(_scrollRectRectTransform.rect.height / (_recordHeight + space));

        IEnumerable<ListRow> CreateRecords(int amount)
        {
            var records = new List<ListRow>();

            for (var i = 0; i < amount; i++)
            {
                records.Add(Instantiate(listRecordPrefab, scrollRect.viewport));
            }

            return records;
        }

        void ResetRecordsPosition()
        {
            var rootPosition = _scrollRectRectTransform.rect.height / 2f;
            rootPosition -= _recordHeight / 2f;
            var deltaPosition = _recordHeight + space;
            for (var i = 0; i < _records.Count; i++)
            {
                var position = new Vector2(0f, rootPosition - deltaPosition * i);
                _records[i].SetAnchoredPosition(position);
            }
        }
        
        void ScrollRectOnValueChanged(Vector2 scrollPosition)
        {
            var deltaPosition = Mathf.Abs(_prevVerticalScrollValue - scrollPosition.y) * scrollRect.content.sizeDelta.y;
            if (_prevVerticalScrollValue > scrollPosition.y)
            {
                MoveUp(deltaPosition);
            }
            else
            {
                MoveDown(deltaPosition);
            }

            _prevVerticalScrollValue = scrollPosition.y;
            LoadImages();
            Display(_firstVisibleRecordIndex);
        }
        
        void MoveDown(float deltaPosition)
        {
            for (var i = 0; i < _records.Count; i++)
            {
                var record = _records[i];
                var anchoredPosition = record.AnchoredPosition;
                if (scrollRect.verticalNormalizedPosition < 1 &&
                    anchoredPosition.y - deltaPosition <= _bottomBorder - record.Height / 2f)
                {
                    MoveRecordToTop(i);
                    _firstVisibleRecordIndex--;
                }
                else
                {
                    anchoredPosition.y -= deltaPosition;
                    record.SetAnchoredPosition(anchoredPosition);
                }
            }
        }

        void MoveRecordToTop(int recordIndex)
        {
            var anchoredPosition = _records[0].AnchoredPosition;
            anchoredPosition.y += _recordHeight + space;
            _records[recordIndex].SetAnchoredPosition(anchoredPosition);
            var temp = _records[recordIndex];
            for (var i = recordIndex; i > 0; i--)
            {
                _records[i] = _records[i - 1];
            }

            _records[0] = temp;
        }
        
        void MoveUp(float deltaPosition)
        {
            for (var i = 0; i < _records.Count; i++)
            {
                var record = _records[i];
                var anchoredPosition = record.AnchoredPosition;
                if (scrollRect.verticalNormalizedPosition > 0 &&
                    anchoredPosition.y + deltaPosition > _topBorder + record.Height / 2f)
                {
                    DropRecordDown(i);
                    i -= 1;
                    _firstVisibleRecordIndex++;
                }
                else
                {
                    anchoredPosition.y += deltaPosition;
                    record.SetAnchoredPosition(anchoredPosition);
                }
            }
        }

        void DropRecordDown(int recordIndex)
        {
            var anchoredPosition = _records[^1].AnchoredPosition;
            anchoredPosition.y -= _recordHeight + space;
            _records[recordIndex].SetAnchoredPosition(anchoredPosition);
            var temp = _records[recordIndex];
            for (var i = recordIndex; i < _records.Count - 1; i++)
            {
                _records[i] = _records[i + 1];
            }

            _records[^1] = temp;
        }
        
        void LoadImages()
        {
            var firstImageIndexToLoad = Mathf.Clamp(_firstVisibleRecordIndex, 0,
                totalNumberOfRecords - _records.Count - 1);
            imageLoader.LoadImages(firstImageIndexToLoad, _records.Count);
        }
        
        void Display(int firstImageIndex)
        {
            var maxImageIndex = totalNumberOfRecords - _numberOfVisibleRecords - 1;
            var firstImageIndexToDisplay = Mathf.Clamp(firstImageIndex, 0, maxImageIndex);
            var imageInfos = imageLoader.GetImageInfos(firstImageIndexToDisplay, _numberOfVisibleRecords);
            for (var i = 0; i < _numberOfVisibleRecords; i++)
            {
                if (i >= imageInfos.Count) break;
                var imageInfo = imageInfos.First(x => x.Id == firstImageIndexToDisplay + i);
                _records[i].Display(firstImageIndexToDisplay + i, imageInfo.Id, imageInfo);
            }
        }
        
        public void ResetList()
        {
            imageLoader.FindImageFiles();
            DestroyRecords();
            SetUpList();
            InitializeList();
        }

        void DestroyRecords()
        {
            foreach (var listRow in _records)
            {
                Destroy(listRow.gameObject);
            }

            _records = new List<ListRow>();
        }
    }
}