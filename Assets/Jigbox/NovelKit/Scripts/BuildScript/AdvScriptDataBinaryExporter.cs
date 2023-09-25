/**
 * Jigbox
 * Copyright(c) 2016 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 *
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications 
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */

#if UNITY_EDITOR || NOVELKIT_EDITOR
using System.IO;
using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public class AdvScriptDataBinaryExporter : AdvScriptDataExporter
    {
#region properties

        /// <summary>Encoder</summary>
        public AdvScriptDataEncoder Encoder { get; set; }

#endregion

#region public methods

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdvScriptDataBinaryExporter()
        {
            Encoder = new AdvScriptDataEncoder();
        }

#endregion

#region protected methods

        /// <summary>
        /// 出力するファイル、ディレクトリの存在確認、生成を行います。
        /// </summary>
        /// <param name="dstFilePath">実行用ファイルの出力先ディレクトリ</param>
        /// <param name="stream">ファイルに書き込むデータ</param>
        protected override void WriteStream(string dstFilePath, List<string> stream)
        {
            Encoder.Encode(stream);
            
            using (BinaryWriter writer = new BinaryWriter(File.Open(dstFilePath, FileMode.Create)))
            {
                writer.Write(Encoder.GetHeader());
                writer.Write(Encoder.GetData());
            }
        }

#endregion
    }
}
#endif
