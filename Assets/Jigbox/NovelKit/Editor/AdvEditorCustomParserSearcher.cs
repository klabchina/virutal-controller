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

using System;

namespace Jigbox.NovelKit
{
    public static class AdvEditorCustomParserSearcher
    {
#region public methods

        /// <summary>
        /// Custom Parserを検索して、ヒットした場合、そのインスタンスを返します。
        /// </summary>
        /// <returns></returns>
        public static IAdvCustomCommandParser GetCustomParser()
        {
            Type target = null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.GetInterface(typeof(IAdvCustomCommandParser).ToString()) != null)
                    {
                        target = type;
                        break;
                    }
                }
            }
            
            if (target != null)
            {
                IAdvCustomCommandParser instance = Activator.CreateInstance(target) as IAdvCustomCommandParser;
                return instance;
            }

            return null;
        }

#endregion
    }
}
