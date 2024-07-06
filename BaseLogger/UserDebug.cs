using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace LoggerLibrary
{
	public static class UserDebug
    {
		internal enum LogTypes
		{
			None = 0,
			Start,
			End,
			Error
		}
		private static int s_methodMaxLength=0;
		private static int s_fileMaxLength=0;
		public static string DateTimeFormat = "yyyy/MM/dd HH:mm:ss.fff";
		

		/// <summary>
		/// デバッグ用メソッド開始ログ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="memberName">メソッド名(入力不要)</param>
		/// <param name="sourceFilePath">ファイルパス（入力不要）</param>
		/// <param name="sourceLineNumber">ソースデータ行（入力不要）</param>
		/// <remarks>DEBUG時のみ実行</remarks>
		[Conditional("DEBUG")]
		public static void MethodStartLog(string message="",
                                    [CallerMemberName]string memberName = "",
		                            [CallerFilePath] string sourceFilePath = "",
		                            [CallerLineNumber]int sourceLineNumber = 0)
		{
			_DebugLog(DateTime.Now, LogTypes.Start,
						(string.IsNullOrWhiteSpace(message) ? "" : $"[Message：{message}]"),
						memberName, Path.GetFileName(sourceFilePath), sourceLineNumber);
			//DebugLog(DateTime.Now,$"[{memberName} : Start],{(string.IsNullOrWhiteSpace(message) ? "" : $"[Message：{message}]")},[File：{Path.GetFileName(sourceFilePath)}],[Line:{sourceLineNumber}]");
		}

		 /// <summary>
		/// デバッグ用メソッド開始ログ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="memberName">メソッド名(入力不要)</param>
		/// <param name="sourceFilePath">ファイルパス（入力不要）</param>
		/// <param name="sourceLineNumber">ソースデータ行（入力不要）</param>
		/// <remarks>DEBUG時のみ実行</remarks>
		[Conditional("DEBUG")]
		public static void MethodStartLog(string message,
									object[] args ,
                                    [CallerMemberName]string memberName = "",
		                            [CallerFilePath] string sourceFilePath = "",
		                            [CallerLineNumber]int sourceLineNumber = 0)
		{
			var hashs = new List<int>();
			string argText = ToStringModule.ListToString(args,ref hashs);
			_DebugLog(DateTime.Now, LogTypes.Start,
			$"{(string.IsNullOrWhiteSpace(message) ? "" : $"[Message：{message}]")}," +
			$"{(string.IsNullOrWhiteSpace(argText) ? "" : $"[Args：{argText}]")}",
			memberName,
			Path.GetFileName(sourceFilePath), sourceLineNumber);
			//DebugLog(DateTime.Now,$"[{memberName} : Start],{(string.IsNullOrWhiteSpace(message) ? "" : $"[Message：{message}]")}," +
			//					  $"{(string.IsNullOrWhiteSpace(argText) ? "" : $"[Args：{argText}]")}," +
			//					  $"[File：{Path.GetFileName(sourceFilePath)}],[Line:{sourceLineNumber}]");
		}
		
		/// <summary>
		/// デバッグ用メソッド終了ログ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="memberName">メソッド名(入力不要)</param>
		/// <param name="sourceFilePath">ファイルパス（入力不要）</param>
		/// <param name="sourceLineNumber">ソースデータ行（入力不要）</param>
		/// <remarks>DEBUG時のみ実行</remarks>
		[Conditional("DEBUG")]
		public static void MethodEndLog(string message = "",
                                    [CallerMemberName]string memberName = "",
		                            [CallerFilePath] string sourceFilePath = "",
		                            [CallerLineNumber]int sourceLineNumber = 0)
		{
			_DebugLog(DateTime.Now, LogTypes.End,
					(string.IsNullOrWhiteSpace(message) ? "" : $"[Message：{message}]"),
					memberName, Path.GetFileName(sourceFilePath), sourceLineNumber);
			//DebugLog(DateTime.Now,$"[{memberName} : End ],{(string.IsNullOrWhiteSpace(message) ? "" : $"[Message：{message}]")},[File：{Path.GetFileName(sourceFilePath)}],[Line:{sourceLineNumber}]");
		}


		/// <summary>
		/// デバッグ用エラーログ
		/// </summary>
		/// <param name="ex">エラー情報</param>
		/// <param name="memberName">メソッド名(入力不要)</param>
		/// <param name="sourceFilePath">ファイルパス（入力不要）</param>
		/// <param name="sourceLineNumber">ソースデータ行（入力不要）</param>
		/// <remarks>DEBUG時のみ実行</remarks>
		[Conditional("DEBUG")]
		public static void ErrorLog(Exception ex,
                                    [CallerMemberName]string memberName = "",
		                            [CallerFilePath] string sourceFilePath = "",
		                            [CallerLineNumber]int sourceLineNumber = 0)
		{
			_DebugLog(DateTime.Now, LogTypes.None,
					($"Type[{ex.GetType().Name}][Message：{ex.Message}]"),
					memberName, Path.GetFileName(sourceFilePath), sourceLineNumber);
			//DebugLog(DateTime.Now,$"[{memberName} : Error[Type:" +
			//	$"{ex.GetType().Name}]],[Message：{ex.Message}],[File：{Path.GetFileName(sourceFilePath)}],[Line{sourceLineNumber}]");
		}

		/// <summary>
		/// デバッグ用ログ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="memberName">メソッド名(入力不要)</param>
		/// <param name="sourceFilePath">ファイルパス（入力不要）</param>
		/// <param name="sourceLineNumber">ソースデータ行（入力不要）</param>
		/// <remarks>DEBUG時のみ実行</remarks>
		[Conditional("DEBUG")]
		public static void DebugLog(string message,
		                            [CallerMemberName]string memberName = "",
		                            [CallerFilePath] string sourceFilePath = "",
		                            [CallerLineNumber]int sourceLineNumber = 0)
		{
			_DebugLog(DateTime.Now, LogTypes.None,
					(string.IsNullOrWhiteSpace(message) ? "" : $"[Message：{message}]"),
					memberName, Path.GetFileName(sourceFilePath), sourceLineNumber);

			
		}
		/// <summary>
		/// デバッグ用ログ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <remarks>DEBUG時のみ実行</remarks>
		[Conditional("DEBUG")]
		private static void _DebugLog(DateTime dateTime,LogTypes type,string message, string memberName,
									 string sourceFileName ,
									int sourceLineNumber)
		{
			if(memberName.Length>s_methodMaxLength)
				s_methodMaxLength=memberName.Length;
			var member=memberName.PadLeft(s_methodMaxLength, ' ');
			if (sourceFileName.Length>s_fileMaxLength)
				s_fileMaxLength=sourceFileName.Length;
			var file=sourceFileName.PadLeft(s_fileMaxLength, ' ');

			var text="";
			switch (type)
			{
				case LogTypes.None:
				text=$"[{dateTime.ToString(DateTimeFormat)}]," +
						 $"[{member}],"+
						 $"[File：{file}]," +
						 $"[Line:{sourceLineNumber.ToString().PadLeft(5,' ')}]," +
						 $"[DEBUG]," +
						 $"{message}";
					break;
				case LogTypes.Start:
					text=$"[{dateTime.ToString(DateTimeFormat)}]," +
						 $"[{member}],"+
						 $"[File：{(file)}]," +
						 $"[Line:{sourceLineNumber.ToString().PadLeft(5, ' ')}]," +
						 $"[START]," +
						 $"{message}";
					break;
				case LogTypes.End:
					text=$"[{dateTime.ToString(DateTimeFormat)}]," +
						 $"[{member}],"+
						 $"[File：{(file)}]," +
						 $"[Line:{sourceLineNumber.ToString().PadLeft(5, ' ')}]," +
						 $"[END  ]," +
						 $"{message}";
					break;

				case LogTypes.Error:
					text=$"[{dateTime.ToString(DateTimeFormat)}]," +
						 $"[{member}],"+
						 $"[File：{(file)}]," +
						 $"[Line:{sourceLineNumber.ToString().PadLeft(5, ' ')}]," +
						 $"[ERROR]," +
						 $"{message}";
					break;
			}
			Console.WriteLine(text);
		}
    }
}
