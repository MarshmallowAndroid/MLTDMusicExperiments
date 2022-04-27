﻿using MessagePack;
using System.Collections.Generic;
using System.IO;

namespace MirishitaMusicPlayer
{
    public class AssetList
    {
        public AssetList(Stream assetListStream)
        {
            var fileDictionary = (MessagePackSerializer.Deserialize<object>(assetListStream) as object[])[0] as Dictionary<object, object>;

            Assets = new Asset[fileDictionary.Count];

            int index = 0;
            foreach (var file in fileDictionary)
            {
                string fileName = file.Key.ToString();
                var assetInfo = file.Value as object[];

                uint fileSize = 0;
                if (assetInfo[2].GetType().Name == "UInt16")
                    fileSize = (ushort)assetInfo[2];
                else if (assetInfo[2].GetType().Name == "UInt32")
                    fileSize = (uint)assetInfo[2];

                Assets[index++] = new(fileName, assetInfo[1].ToString(), fileSize);
            }

            assetListStream.Dispose();
        }

        public Asset[] Assets { get; }
    }
}
