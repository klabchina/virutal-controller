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

namespace Jigbox.NovelKit
{
    using ObjectType = AdvObjectBase.ObjectType;

    public static class AdvObjectCommandParser
    {
#region constants

#region object type

        /// <summary>キャラクター関連コマンド先頭語</summary>
        public const string HeaderWordObjectCharacter = "ch";

        /// <summary>キャラクター以外の画像関連コマンド先頭語</summary>
        public const string HeaderWordObjectSprite = "sp";

        /// <summary>背景関連コマンド先頭語</summary>
        public const string HeaderWordObjectBg = "bg";

        /// <summary>CG(一枚絵)関連コマンド先頭語</summary>
        public const string HeaderWordObjectCg = "cg";

        /// <summary>感情表現系エモーション関連コマンド先頭語</summary>
        public const string HeaderWordObjectEmotional = "em";

        /// <summary>演出関連コマンド先頭語</summary>
        public const string HeaderWordObjectEffect = "ef";

        /// <summary>その他のオブジェクト関連コマンド先頭語</summary>
        public const string HeaderWordObjectOther = "ot";
        
        /// <summary>画面効果関連コマンド先頭語</summary>
        public const string HeaderWordObjectScreenEffect = "se";

#endregion

#region options

        /// <summary>その種類に含まれる全てのオブジェクトに対してコマンドを発行する場合</summary>
        public const string IndexAll = "all";

        /// <summary>差分オブジェクトとして設定する場合のオプション文字列</summary>
        public const string OptionSubObject = "sub-";

#endregion

#region commands

        /// <summary>リソースの読み込み</summary>
        public const string CommandLoad = "load";

        /// <summary>位置</summary>
        public const string CommandPosition = "pos";

        /// <summary>回転</summary>
        public const string CommandRotation = "rot";

        /// <summary>拡大縮小</summary>
        public const string CommandScale = "scale";

        /// <summary>画像サイズ</summary>
        public const string CommandSize = "size";

        /// <summary>画像の色</summary>
        public const string CommandColor = "color";

        /// <summary>レイヤー</summary>
        public const string CommandLayer = "layer";

        /// <summary>プレーンの設定</summary>
        public const string CommandPlane = "plane";

        /// <summary>画像の前後</summary>
        public const string CommandDepth = "depth";

        /// <summary>オブジェクトを表示</summary>
        public const string CommandShow = "show";

        /// <summary>オブジェクトを非表示</summary>
        public const string CommandHide = "hide";

        /// <summary>オブジェクトを破棄</summary>
        public const string CommandDelete = "delete";

        /// <summary>オブジェクトを移動</summary>
        public const string CommandTweenMove = "move";

        /// <summary>オブジェクトを回転</summary>
        public const string CommandTweenRotation = "rotation";

        /// <summary>オブジェクトを拡縮</summary>
        public const string CommandTweenScaling = "scaling";

        /// <summary>オブジェクトの色を変更</summary>
        public const string CommandTweenColor = "coloring";

        /// <summary>オブジェクトを振動</summary>
        public const string CommandShake = "shake";

        /// <summary>オブジェクトの振動を停止</summary>
        public const string CommandStopShake = "stopshake";

        /// <summary>オブジェクトのマテリアルを変更</summary>
        public const string CommandMaterial = "material";

        /// <summary>オブジェクトにメッセージを送信</summary>
        public const string CommandMessage = "message";

        /// <summary>オブジェクトの合成</summary>
        public const string CommandMix = "mix";

        /// <summary>合成状態のオブジェクトの分離</summary>
        public const string CommandDivide = "divide";

#endregion

#endregion

#region public methods

        /// <summary>
        /// オブジェクト用コマンドとしてパースを行います。
        /// </summary>
        /// <param name="commandBase">パース元のコマンド</param>
        /// <returns>パースに成功した場合、正規のコマンドが返り、不正コマンドの場合はエラー、
        /// オブジェクト用コマンド以外の場合はnullが返ります。</returns>
        public static AdvCommandBase Parse(AdvCommandBase commandBase)
        {
            ObjectType objectType;
            if (!GetObjectType(commandBase, out objectType))
            {
                return null;
            }

            bool isAll;
            bool isSubObject;
            AdvCommandBase command = ValidateParameterOptions(commandBase, objectType, out isAll, out isSubObject);
            if (command != null)
            {
                return command;
            }

            switch (commandBase.Param[2])
            {
                case CommandLoad:
                    command = new AdvCommandObjectLoad(commandBase, objectType, isSubObject);
                    break;
                case CommandPosition:
                    command = new AdvCommandObjectPosition(commandBase, objectType, isSubObject);
                    break;
                case CommandRotation:
                    command = new AdvCommandObjectRotation(commandBase, objectType, isSubObject);
                    break;
                case CommandScale:
                    command = new AdvCommandObjectScale(commandBase, objectType, isSubObject);
                    break;
                case CommandSize:
                    command = new AdvCommandObjectSize(commandBase, objectType, isSubObject);
                    break;
                case CommandColor:
                    command = new AdvCommandObjectColor(commandBase, objectType, isSubObject);
                    break;
                case CommandLayer:
                    command = new AdvCommandObjectLayer(commandBase, objectType, isSubObject);
                    break;
                case CommandPlane:
                    if (!isSubObject)
                    {
                        command = new AdvCommandObjectPlane(commandBase, objectType, isSubObject);
                    }
                    else
                    {
                        // 差分用オブジェクトは親がPlaneではないので、そもそも意味が無い
                        command = AdvCommandParser.CreateErrorCommand(commandBase, "オプションにサブオブジェクトを指定した場合、planeコマンドは使用できません。");
                    }
                    break;
                case CommandDepth:
                    if (!isAll)
                    {
                        command = new AdvCommandObjectDepth(commandBase, objectType, isSubObject);
                    }
                    else
                    {
                        // 特定種類のオブジェクト全体にDepthを設定しても、
                        // Depthはそもそもオブジェクト毎に異なるものなので意味が無い
                        command = AdvCommandParser.CreateErrorCommand(commandBase, "全てのオブジェクトを指定した場合、depthコマンドは使用できません。");
                    }
                    break;
                case CommandShow:
                    command = new AdvCommandObjectShow(commandBase, objectType, isSubObject);
                    break;
                case CommandHide:
                    command = new AdvCommandObjectHide(commandBase, objectType, isSubObject);
                    break;
                case CommandDelete:
                    command = new AdvCommandObjectDelete(commandBase, objectType, isSubObject);
                    break;
                case CommandTweenMove:
                    command = new AdvCommandObjectTweenMove(commandBase, objectType, isSubObject);
                    break;
                case CommandTweenRotation:
                    command = new AdvCommandObjectTweenRotation(commandBase, objectType, isSubObject);
                    break;
                case CommandTweenScaling:
                    command = new AdvCommandObjectTweenScaling(commandBase, objectType, isSubObject);
                    break;
                case CommandTweenColor:
                    command = new AdvCommandObjectTweenColor(commandBase, objectType, isSubObject);
                    break;
                case CommandShake:
                    command = new AdvCommandObjectShake(commandBase, objectType, isSubObject);
                    break;
                case CommandStopShake:
                    command = new AdvCommandObjectStopShake(commandBase, objectType, isSubObject);
                    break;
                case CommandMaterial:
                    command = new AdvCommandObjectMaterial(commandBase, objectType, isSubObject);
                    break;
                case CommandMessage:
                    command = new AdvCommandObjectMessage(commandBase, objectType, isSubObject);
                    break;
                case CommandMix:
                    if (!isAll && !isSubObject)
                    {
                        command = new AdvCommandObjectMix(commandBase, objectType, isSubObject);
                    }
                    else
                    {
                        // 合成は個々のオブジェクトに対してのみ実行可能
                        if (isAll)
                        {
                            command = AdvCommandParser.CreateErrorCommand(commandBase, "オプションにサブオブジェクトを指定した場合、mixコマンドは使用できません。");
                        }
                        else
                        {
                            command = AdvCommandParser.CreateErrorCommand(commandBase, "全てのオブジェクトを指定した場合、mixコマンドは使用できません。");
                        }
                    }
                    break;
                case CommandDivide:
                    if (!isAll && !isSubObject)
                    {
                        command = new AdvCommandObjectDivide(commandBase, objectType, isSubObject);
                    }
                    else
                    {
                        // 合成は個々のオブジェクトに対してのみ実行可能
                        if (isAll)
                        {
                            command = AdvCommandParser.CreateErrorCommand(commandBase, "オプションにサブオブジェクトを指定した場合、divideコマンドは使用できません。");
                        }
                        else
                        {
                            command = AdvCommandParser.CreateErrorCommand(commandBase, "全てのオブジェクトを指定した場合、divideコマンドは使用できません。");
                        }
                    }
                    break;
            }

            return command;
        }

        /// <summary>
        /// <para>オブジェクトの種類を取得します。</para>
        /// <para>コマンドがオブジェクトでない場合は、falseを返します。</para>
        /// </summary>
        /// <param name="commandBase">コマンド</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <returns></returns>
        public static bool GetObjectType(AdvCommandBase commandBase, out ObjectType objectType)
        {
            switch (commandBase.Param[0])
            {
                case HeaderWordObjectCharacter:
                    objectType = ObjectType.Character;
                    break;
                case HeaderWordObjectSprite:
                    objectType = ObjectType.Sprite;
                    break;
                case HeaderWordObjectBg:
                    objectType = ObjectType.Bg;
                    break;
                case HeaderWordObjectCg:
                    objectType = ObjectType.Cg;
                    break;
                case HeaderWordObjectEmotional:
                    objectType = ObjectType.Emotional;
                    break;
                case HeaderWordObjectEffect:
                    objectType = ObjectType.Effect;
                    break;
                case HeaderWordObjectOther:
                    objectType = ObjectType.Other;
                    break;
                case HeaderWordObjectScreenEffect:
                    objectType = ObjectType.ScreenEffection;
                    break;
                // オブジェクト系以外ではない場合
                default:
                    objectType = ObjectType.Other;
                    return false;
            }
            return true;
        }

        /// <summary>
        /// <para>オブジェクトコマンドの引数のオプション指定状態を確認します。</para>
        /// <para>不正状態だった場合、エラーコマンドを返します。</para>
        /// </summary>
        /// <param name="commandBase">コマンド</param>
        /// <param name="objectType">オブジェクトの種類</param>
        /// <param name="isAll">全てのオブジェクトを選択するかどうか</param>
        /// <param name="isSubObject">サブオブジェクトかどうか</param>
        /// <returns></returns>
        public static AdvCommandBase ValidateParameterOptions(AdvCommandBase commandBase, ObjectType objectType, out bool isAll, out bool isSubObject)
        {
            isAll = false;
            isSubObject = false;
            // 1つ目のパラメータがオブジェクトの種類、2つ目が管理ID、3つ目がコマンド
            if (commandBase.Param.Length < 2)
            {
                return AdvCommandParser.CreateErrorCommand(commandBase, "管理IDが設定されていません。");
            }
            else
            {
                isAll = commandBase.Param[1] == IndexAll;
            }

            if (commandBase.Param.Length < 3)
            {
                return AdvCommandParser.CreateErrorCommand(commandBase, "コマンドが設定されていません。");
            }

            // 命令の直後のパラメータに"sub-"という文字列が含まれる場合、差分用のコマンドとして扱う
            if (commandBase.Param.Length >= 4)
            {
                isSubObject = commandBase.Param[3].IndexOf(OptionSubObject) >= 0;
            }
            // 差分用のコマンドでは管理IDにallは指定できない
            if (isSubObject && isAll)
            {
                return AdvCommandParser.CreateErrorCommand(commandBase, "オプションに差分を指定した場合、管理IDをallにすることはできません。");
            }

            // 画面効果のみ、オブジェクトとして存在はしているが扱いが特殊なので例外処理
            if (objectType == ObjectType.ScreenEffection)
            {
                if (isAll)
                {
                    return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、allは使用できません。");
                }
                if (isSubObject)
                {
                    return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、サブオブジェクトは扱えません。");
                }
                // 画面効果の場合、IDは必ず1
                int id;
                if (int.TryParse(commandBase.Param[1], out id))
                {
                    if (id != 1)
                    {
                        return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、IDは1以外指定できません。");
                    }
                }

                switch (commandBase.Param[2])
                {
                    case CommandLoad:
                        return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、loadコマンドは使用できません。");
                    case CommandLayer:
                        return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、layerコマンドは使用できません。");
                    case CommandPlane:
                        return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、planeコマンドは使用できません。");
                    case CommandDepth:
                        return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、depthコマンドは使用できません。");
                    case CommandDelete:
                        return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、deleteコマンドは使用できません。");
                    case CommandMessage:
                        return AdvCommandParser.CreateErrorCommand(commandBase, "オブジェクトの種類に画面効果を指定した場合、messageコマンドは使用できません。");
                }
            }

            return null;
        }

#endregion
    }
}
