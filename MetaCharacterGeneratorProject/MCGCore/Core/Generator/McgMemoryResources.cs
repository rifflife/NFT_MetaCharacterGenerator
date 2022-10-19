using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MCGCore
{
	/// <summary> MCG 자원 관리용 클래스입니다. </summary>
	public class McgMemoryResources
	{
		public bool ShouldCacheOnMemoryWhenGenerate { get; private set; }
		public string CachePath { get; private set; }

		public bool IsInitialized => mIsInitialized;

		private Dictionary<string, (BitmapSource Bitmap, string LayerName)> mImageCacheSet;
		private Dictionary<string, (string BitmapPath, string LayerName)> mCachedImagePathSet;
		private bool mIsInitialized = false;

		private McgProjectConfiguration mConfig;

		/// <summary>
		/// 디스크에 저장된 캐싱된 이미지를 모두 삭제하고 초기화합니다.
		/// </summary>
		public void Initialize(McgProjectConfiguration config)
		{
			mConfig = config;

			mImageCacheSet = new Dictionary<string, (BitmapSource Bitmap, string LayerName)>();
			mCachedImagePathSet = new Dictionary<string, (string BitmapPath, string LayerName)>();

			ShouldCacheOnMemoryWhenGenerate = mConfig.ShouldCacheOnMemoryWhenGenerate;
			CachePath = mConfig.CachePath;

			// 새로운 이미지를 캐싱하기 위해 이미 캐싱된 이미지를 삭제합니다.
			var files = Directory.GetFiles(CachePath);

			foreach (string f in files)
			{
				var fileExtension = Path.GetExtension(f).ToLower();
				if (fileExtension == mConfig.ImageFileExtension)
				{
					try
					{
						File.Delete(f);
					}
					catch
					{
						throw new CacheDeleteFailedException("Failed to delete cached images!");
					}
				}
			}

			mIsInitialized = true;
			StaticConsole.WriteLine("Cached image deleted.");
			StaticConsole.WriteLine("Memory resources initialize success.");
		}

		/// <summary>
		/// 이미지를 디스크에 저장 후 캐싱합니다.
		/// </summary>
		/// <exception cref="NotSupportedException"/>
		/// <exception cref="NotInitializedException"/>
		public void CacheIamge(string imageName, BitmapSource bitmap, string layerName)
		{
			checkInitialize();

			StaticConsole.WriteLine($"Cache image : {imageName}");

			string imagePath = Path.Combine(CachePath, $"{imageName}{mConfig.ImageFileExtension}");

			ImageProcessor.SaveImage(bitmap, imagePath);

			if (ShouldCacheOnMemoryWhenGenerate)
			{
				mImageCacheSet[imageName] = (bitmap, layerName);
			}

			mCachedImagePathSet[imageName] = (imagePath, layerName);
		}

		/// <summary> 캐싱한 이미지를 반환합니다. </summary>
		/// <exception cref="CacheFailedException"/>
		/// <exception cref="NotInitializedException"/>
		public (BitmapSource Bitmap, string LayerName) LoadCacheImage(string imageName)
		{
			checkInitialize();

			StaticConsole.WriteLine($"Cached image load : {imageName}");

			if (ShouldCacheOnMemoryWhenGenerate)
			{
				if (mImageCacheSet.TryGetValue(imageName, out var bitmap))
				{
					return bitmap;
				}
				else
				{
					throw new CacheFailedException("There is no cached image !");
				}
			}
			else
			{
				bool isFinded = mCachedImagePathSet.TryGetValue(imageName, out var imageData);

				if (isFinded)
				{
					try
					{
						return (ImageProcessor.LoadBitmap(imageData.BitmapPath), imageData.LayerName);
					}
					catch
					{
						throw new CacheFailedException("Failed to load cached image !");
					}
				}
				else
				{
					throw new CacheFailedException("There is no cached image !");
				}
			}
		}

		public bool HasImage(string imageName)
		{
			return mImageCacheSet.ContainsKey(imageName) || mCachedImagePathSet.ContainsKey(imageName);
		}

		private void checkInitialize()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException("MCG Memory Resources not initialized");
			}
		}
	}
}
