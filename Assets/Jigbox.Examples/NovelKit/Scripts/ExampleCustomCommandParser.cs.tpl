/*
using Jigbox.NovelKit;

namespace Jigbox.Examples
{
    using ObjectType = AdvObjectBase.ObjectType;

    public class ExampleCustomCommandParser : IAdvCustomCommandParser
    {
#region constansts

        protected static readonly System.Type TextCommandType = typeof(AdvCommandText);

#endregion

#region public methods

        public AdvCommandBase Parse(AdvCommandBase commandBase)
        {
            AdvCommandBase command;

            // 案件用オブジェクト系コマンドのパースを実行
            command = ParseObject(commandBase);
            if (command != null)
            {
                return command;
            }

            // オブジェクト系以外のコマンドのパースを実行
            command = ParseOther(commandBase);
            
            return command == null ? commandBase : command;
        }

#endregion

#region protected methods

        /// <summary>
        /// オブジェクト用コマンドのパースを行います。
        /// </summary>
        /// <param name="commandBase">パース元のコマンド</param>
        /// <returns>パースに成功した場合、正規のコマンドが返り、不正コマンドの場合はエラー、
        /// オブジェクト用コマンド以外の場合はnullが返ります。</returns>
        protected AdvCommandBase ParseObject(AdvCommandBase commandBase)
        {
            ObjectType objectType;
            if (!AdvObjectCommandParser.GetObjectType(commandBase, out objectType))
            {
                return null;
            }

            bool isAll;
            bool isSubObject;
            AdvCommandBase command = AdvObjectCommandParser.ValidateParameterOptions(commandBase, objectType, out isAll, out isSubObject);
            if (command != null)
            {
                return command;
            }

            switch (commandBase.Param[2])
            {
                // ここで案件用のオブジェクトコマンドをパースする
            }

            return command;
        }

        /// <summary>
        /// オブジェクト用以外のコマンドのパースを行います。
        /// </summary>
        /// <param name="commandBase">パース元のコマンド</param>
        /// <returns>パースに成功した場合、正規のコマンドが返り、不正コマンドの場合はエラーが返ります。</returns>
        protected AdvCommandBase ParseOther(AdvCommandBase commandBase)
        {
            AdvCommandBase command = null;
            switch (commandBase.Param[0])
            {
                // ここで案件用のオブジェクト以外のコマンドをパースする
            }

            if (command == null && commandBase.Type == AdvCommandBase.CommandType.ErrorCommand && commandBase.GetType() == TextCommandType)
            {
                // 案件用のテキストコマンドがあれば、ここでパースする
            }

            return command;
        }

#endregion
    }
}
*/
