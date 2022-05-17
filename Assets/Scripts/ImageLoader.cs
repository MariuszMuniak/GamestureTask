using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace GamestureTask
{
    public class ImageLoader : MonoBehaviour
    {
        public const string IMAGES_DIRECTORY_NAME = "Images";

        public int NumberOfImages => _imageFiles.Count;

        List<FileInfo> _imageFiles = new();
        List<ImageInfo> _loadedImages = new();
        Dictionary<int, Coroutine> _imagesCoroutines = new();

        void Awake() => FindImageFiles();

        public void FindImageFiles()
        {
            foreach (var keyValuePair in _imagesCoroutines)
            {
                StopCoroutine(keyValuePair.Value);
            }

            _imageFiles = FileManager.GetImageFiles(IMAGES_DIRECTORY_NAME, SearchOption.AllDirectories);
            _loadedImages = new List<ImageInfo>();
            _imagesCoroutines = new Dictionary<int, Coroutine>();
        }

        public List<ImageInfo> GetImageInfos(int startIndex, int count)
        {
            if (count <= 0) return new List<ImageInfo>();
            var images = new List<ImageInfo>();
            if (startIndex < 0) startIndex = 0;
            var endIndex = startIndex + count;
            if (endIndex >= _imageFiles.Count) endIndex = _imageFiles.Count;
            for (var i = startIndex; i < endIndex; i++)
            {
                ImageInfo image;
                if (IsImageLoaded(i) || IsImageLoading(i))
                {
                    image = GetImage(i);
                }
                else
                {
                    image = LoadImage(i);
                    _loadedImages.Add(image);
                }

                images.Add(image);
            }

            return images;
        }

        public void LoadImages(int startIndex, int count)
        {
            if (startIndex < 0) startIndex = 0;
            var endIndex = startIndex + count;
            if (endIndex >= _imageFiles.Count) endIndex = _imageFiles.Count;
            for (var i = startIndex; i < endIndex; i++)
            {
                if (IsImageLoaded(i) || IsImageLoading(i)) continue;
                var image = LoadImage(i);
                _loadedImages.Add(image);
            }

            for (var i = _loadedImages.Count - 1; i >= 0; i--)
            {
                var image = _loadedImages[i];
                if (image.Id >= startIndex && image.Id <= endIndex) continue;
                _loadedImages.Remove(image);
                if (!IsImageLoading(image.Id)) continue;
                StopCoroutine(_imagesCoroutines[image.Id]);
                _imagesCoroutines.Remove(image.Id);
            }
        }

        ImageInfo GetImage(int id) => _loadedImages.First(image => image.Id == id);

        bool IsImageLoaded(int imageIndex) => _loadedImages.Any(x => x.Id == imageIndex);

        bool IsImageLoading(int imageIndex) => _imagesCoroutines.ContainsKey(imageIndex);

        ImageInfo LoadImage(int i)
        {
            var imageFile = _imageFiles[i];
            var imageInfo = new ImageInfo(i, imageFile);
            var createImageCoroutine = StartCoroutine(CreateImageCoroutine(imageInfo, imageFile.FullName));
            _imagesCoroutines.Add(imageInfo.Id, createImageCoroutine);
            return imageInfo;
        }

        IEnumerator CreateImageCoroutine(ImageInfo imageInfo, string imageSourcePath)
        {
            if (!File.Exists(imageSourcePath))
            {
                _imagesCoroutines.Remove(imageInfo.Id);
                yield break;
            }

            using var webRequest = UnityWebRequestTexture.GetTexture($"file://{imageSourcePath}");
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                var texture2D = DownloadHandlerTexture.GetContent(webRequest);
                var spriteRect = new Rect(0f, 0f, texture2D.width, texture2D.height);
                var sprite = Sprite.Create(texture2D, spriteRect, Vector2.zero, 100f);
                imageInfo.SetSprite(sprite);
            }

            _imagesCoroutines.Remove(imageInfo.Id);
        }
    }
}