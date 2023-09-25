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

using System.Collections.Generic;

namespace Jigbox.NovelKit
{
    public sealed class AdvScriptResourceManager
    {
#region properties

        /// <summary>シーン</summary>
        Dictionary<string, List<string>> scenes = new Dictionary<string, List<string>>();


        /// <summary>定数</summary>
        Dictionary<string, List<string>> constantValues = new Dictionary<string, List<string>>();


        /// <summary>マクロ</summary>
        Dictionary<string, List<string>> macro = new Dictionary<string, List<string>>();

#endregion

#region public methods

        /// <summary>
        /// 全てクリアします。
        /// </summary>
        public void ClearAll()
        {
            scenes.Clear();
            constantValues.Clear();
            macro.Clear();
        }

        /// <summary>
        /// スクリプトのシーン名を追加します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        /// <param name="sceneNames">読み込んだシーン名</param>
        public void AddScene(string resource, List<string> sceneNames)
        {
            if (scenes.ContainsKey(resource))
            {
                scenes[resource] = sceneNames;
            }
            else
            {
                scenes.Add(resource, sceneNames);
            }
        }

        /// <summary>
        /// スクリプトのシーン名を破棄します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        public void RemoveScenes(string resource)
        {
            if (scenes.ContainsKey(resource))
            {
                scenes.Remove(resource);
            }
        }

        /// <summary>
        /// 対象スクリプトのシーン名が追加されているかどうかを取得します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        public bool ContainsScenes(string resource)
        {
            return scenes.ContainsKey(resource);
        }

        /// <summary>
        /// スクリプトのシーン名をクリアします。
        /// </summary>
        public void ClearScenes()
        {
            scenes.Clear();
        }

        /// <summary>
        /// 対象スクリプトに含まれるシーン名を取得します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        /// <returns></returns>
        public List<string> GetScenes(string resource)
        {
            if (scenes.ContainsKey(resource))
            {
                return scenes[resource];
            }
            return null;
        }

        /// <summary>
        /// 定数名を追加します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        /// <param name="constantValueNames">定数名</param>
        public void AddConstantValues(string resource, List<string> constantValueNames)
        {
            if (constantValues.ContainsKey(resource))
            {
                constantValues[resource] = constantValueNames;
            }
            else
            {
                constantValues.Add(resource, constantValueNames);
            }
        }

        /// <summary>
        /// 定数名を破棄します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        public void RemoveConstantValues(string resource)
        {
            if (constantValues.ContainsKey(resource))
            {
                constantValues.Remove(resource);
            }
        }

        /// <summary>
        /// 対象ファイルの定数名が追加されているかどうかを取得します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        public bool ContainsConstantValues(string resource)
        {
            return constantValues.ContainsKey(resource);
        }

        /// <summary>
        /// 定数名をクリアします。
        /// </summary>
        public void ClearConstantValues()
        {
            constantValues.Clear();
        }

        /// <summary>
        /// 対象ファイルに定義された定数名を取得します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        /// <returns></returns>
        public List<string> GetConstantValues(string resource)
        {
            if (constantValues.ContainsKey(resource))
            {
                return constantValues[resource];
            }
            return null;
        }

        /// <summary>
        /// マクロ名を追加します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        /// <param name="macroNames">マクロ名</param>
        public void AddMacro(string resource, List<string> macroNames)
        {
            if (macro.ContainsKey(resource))
            {
                macro[resource] = macroNames;
            }
            else
            {
                macro.Add(resource, macroNames);
            }
        }

        /// <summary>
        /// マクロ名を破棄します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        public void RemoveMacro(string resource)
        {
            if (macro.ContainsKey(resource))
            {
                macro.Remove(resource);
            }
        }

        /// <summary>
        /// 対象ファイルのマクロ名が追加されているかどうかを取得します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        public bool ContainsMacro(string resource)
        {
            return macro.ContainsKey(resource);
        }

        /// <summary>
        /// マクロ名をクリアします。
        /// </summary>
        public void ClearMacro()
        {
            macro.Clear();
        }

        /// <summary>
        /// 対象ファイルに定義されたマクロ名を取得します。
        /// </summary>
        /// <param name="resource">リソースのパス</param>
        /// <returns></returns>
        public List<string> GetMacros(string resource)
        {
            if (macro.ContainsKey(resource))
            {
                return macro[resource];
            }
            return null;
        }

#endregion
    }
}
