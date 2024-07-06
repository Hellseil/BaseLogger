/******************************************************************************
 * ファイル	：LogData.cs
 * 目的		：
 * 名前空間	:LoggerLibrary.Datas
 * 依存関係	：
 * 注意点	：
 * 備考		：
 * Netver	：4.8
 * 変更履歴
 *	2024/##/##	ysugi		新規作成
*******************************************************************************/


//-----------------------------------------------
#region 使用する名前空間
using EnumExtendLibrary;
using LoggerLibrary.Enumeration;
using System;
using System.IO;
using System.Runtime.CompilerServices;


#endregion

namespace LoggerLibrary.Datas
{
	public class LogData
	{
		//---------------------------------------------------------------------
		#region 定数

		#endregion

		//---------------------------------------------------------------------
		#region メンバー

		#endregion

		//---------------------------------------------------------------------
		#region コンストラクタ＆デストラクタ

		/// <summary>
		/// LogDataを生成します
		/// </summary>
		public LogData(LoggerBase logger, DateTime dateTime, LogLevel logLevel, string message, string methodName, string tagName)
		{
			this.DateTimeString = dateTime.ToString(logger.DateTimeFormat);
			this.Level = logLevel;
			this.Message = message;
			this.MethodName = methodName;
			this.TagName = tagName;
		}

		/// <summary>
		/// LogDataを破棄します
		/// </summary>
		~LogData()
		{

		}
		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		public virtual string DateTimeString { get; protected set; }
		public virtual LogLevel Level { get; protected set; }
		public virtual string Message { get; protected set; }
		public virtual string MethodName { get; protected set; }
		public virtual string TagName { get; protected set; }
		#endregion

		//---------------------------------------------------------------------
		#region 公開メソッド
		public static LogData Create(LoggerBase logger, DateTime datetime, LogLevel logLevel, string message = "", [CallerFilePath] string filepath = "", [CallerMemberName] string memberName = "")
		{
			return new LogData(logger, datetime, logLevel, message, memberName, Path.GetFileName(filepath));
		}
		public static LogData Create(LoggerBase logger, DateTime datetime, LogLevel logLevel, string message, ILogTag logTag, [CallerMemberName] string memberName = "")
		{
			return new LogData(logger, datetime, logLevel, message, memberName, logTag.Tag);
		}
		public virtual string ToLogString()
		{
			return $"[{this.DateTimeString}],[{this.Level.DisplayName()}],[{this.TagName}],[{this.MethodName}]{(string.IsNullOrWhiteSpace(this.Message) ? "" : $"[{this.Message}]")}";
		}
		#endregion

		//---------------------------------------------------------------------
		#region 非公開メソッド

		#endregion

		//---------------------------------------------------------------------
		#region インターフェース実装

		#endregion

	}
}
