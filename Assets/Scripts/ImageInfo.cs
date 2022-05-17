using System;
using System.IO;
using UnityEngine;

namespace GamestureTask
{
    public class ImageInfo
    {
        public readonly int Id;
        public string ImageName => Path.GetFileNameWithoutExtension(_imageFile.Name);
        public bool IsImageLoaded { get; private set; }
        public Sprite Image { get; private set; }

        readonly FileInfo _imageFile;
        
        public ImageInfo(int id, FileInfo imageFile)
        {
            Id = id;
            _imageFile = imageFile;
        }
    
        public TimeSpan GetTimeSinceWasCreated() => DateTime.Now - _imageFile.CreationTime;

        public void SetSprite(Sprite sprite)
        {
            Image = sprite;
            IsImageLoaded = true;
        }
    }
}