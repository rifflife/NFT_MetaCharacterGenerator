using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MCGCore
{
	public static class HashManager
	{
		private static SHA256 mSHA256 = SHA256.Create();

        /// <summary>
        /// 각각의 요소를 해싱하고, 정렬한 뒤 재해싱 한 해쉬 코드를 반환합니다. SHA256을 사용합니다.
        /// 유효하지 않은 값인 경우 null을 반환합니다.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
		public static string GetHeshStringOrNull(List<IMetadataAttribute> attributes)
		{
            if (attributes == null)
			{
                return null;
			}

            List<string> hashList = new List<string>();

            foreach (var attribute in attributes)
			{
                hashList.Add(attribute.GetHeshString());
			}

            hashList.Sort();

            List<byte> hashBuffer = new List<byte>();

            foreach (var hexString in hashList)
			{
                hashBuffer.AddRange(StreamManager.HexToBytes(hexString));
			}

			var hashBytes = mSHA256.ComputeHash(hashBuffer.ToArray());

            return StreamManager.ToHex(hashBytes);
		}

        public static string GetHeshString(byte[] bytes)
		{
            var buffer = mSHA256.ComputeHash(bytes);
            return StreamManager.ToHex(buffer);
		}
    }
}
