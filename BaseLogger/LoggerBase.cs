using LoggerLibrary.Datas;
using LoggerLibrary.Enumeration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LoggerLibrary
{
	public abstract class LoggerBase:IDisposable
	{

		#region Member
		/// <summary>
		/// ログ書き込みタスク
		/// </summary>
		private Task	_logTask ;
		/// <summary>
		/// 出力待機ログリスト
		/// </summary>
		protected Queue<LogData>	_logQueue ;
		/// <summary>
		/// 終了判定
		/// </summary>
		private bool	_closeFlag ;
		protected LogLevel _logLevel = LogLevel.Debug ;

		private bool _isFilePathChenged = true ;
		private string _filePath=System.IO.Directory.GetCurrentDirectory()+"\\log_"+DateTime.Now.ToString("yyyyMMdd")+".log";
		protected string _initialText="";
		protected string _dateFormat = "yyyy/MM/dd HH:mm:ss.fff";
		#endregion

		#region Property
		protected string _FilePath
		{ 
			get
			{
				return this._filePath ;
			}
			set
			{
				if(this._filePath!=value)
				{
					this._filePath = value ;
					this._isFilePathChenged=true ;
				}
			}
		}
		public string DateTimeFormat
		{
			get => this._dateFormat;
		}
		#endregion
        
		#region Constructor
		/// <summary>
		/// ログクラスを生成します
		/// </summary>
		public LoggerBase(LogLevel logLevel)
		{
			this._logLevel = logLevel ;
			this._logQueue = new Queue<LogData>() ;
			this._closeFlag = false ;
		}
		#endregion

		#region Method
		const string SEPARATION_TEXT = "-------------------------------------------------------------------------------";

		/// <summary>
		/// ロギング開始
		/// </summary>
		/// <param name="path">ログフォルダパス</param>
		public virtual void StartLogging( string initial_text )
		{
			this._initialText = initial_text ;


			this._logTask = Task.Run( async () =>
			{
				while( ! this._closeFlag ){
					this._WriteLogFile() ;
					await Task.Delay( 10 ) ;
				}
			} ) ;
		}
		#region エラーログ
		/// <summary>
		/// エラーログを出力します。
		/// </summary>
		/// <param name="senderName">エラー発生元</param>
		/// <param name="message">エラー</param>
		public virtual void AddErrorLog( string message,ILogTag logTag, [CallerMemberName]string memberName = "")
		{
			if( LogLevel.Error  <= this._logLevel ){
				_AddLog(LogLevel.Error, logTag.Tag, memberName, message);
			}
		}
		/// <summary>
		/// エラーログを出力します。
		/// </summary>
		/// <param name="senderName">エラー発生元</param>
		/// <param name="message">エラー</param>
		public virtual void AddErrorLog( string message,[CallerFilePath]string filepath="", [CallerMemberName]string memberName = "")
		{
			_AddLog(LogLevel.Error, Path.GetFileName(filepath), memberName, message);
		}
		/// <summary>
		/// エラーログを出力します。
		/// </summary>
		/// <param name="senderName">エラー発生元</param>
		/// <param name="message">エラー</param>
		public virtual void AddErrorLog( Exception ex,ILogTag logTag, [CallerMemberName]string memberName = "")
		{
				_AddLog(LogLevel.Error, logTag.Tag, memberName, ex.Message);
		}
		/// <summary>
		/// エラーログを出力します。
		/// </summary>
		/// <param name="senderName">エラー発生元</param>
		/// <param name="message">エラー</param>
		public virtual void AddErrorLog( Exception ex,[CallerFilePath]string filepath="", [CallerMemberName]string memberName = "")
		{
			_AddLog(LogLevel.Error, Path.GetFileName(filepath), memberName, ex.Message);
		}
		#endregion
		/// <summary>
		/// 警告ログを出力します。
		/// </summary>
		/// <param name="senderName">警告発生元</param>
		/// <param name="message">警告メッセージ</param>
		public void AddWarningLog( string message,ILogTag logTag, [CallerMemberName]string memberName = "" )
		{
			_AddLog(LogLevel.Warning, logTag.Tag, memberName, message);
		}
		
		/// <summary>
		/// 警告ログを出力します。
		/// </summary>
		/// <param name="senderName">警告発生元</param>
		/// <param name="message">警告メッセージ</param>
		public void AddWarningLog(string senderName,[CallerFilePath]string filepath="", [CallerMemberName]string memberName = "" )
		{
			_AddLog(LogLevel.Warning, Path.GetFileName(filepath), memberName, senderName);
		}
		/// <summary>
		/// 操作ログを出力します
		/// </summary>
		/// <param name="sourceName">発生元</param>
		/// <param name="actionName">動作名</param>
		public void AddOperateLog(string senderName,[CallerFilePath]string filepath="", [CallerMemberName]string memberName = ""  )
		{
			_AddLog(LogLevel.Operate, Path.GetFileName(filepath), memberName, senderName);
		}
		/// <summary>
		/// 操作ログを出力します
		/// </summary>
		/// <param name="sourceName">発生元</param>
		/// <param name="actionName">動作名</param>
		public void AddOperateLog( string senderName,ILogTag logTag, [CallerMemberName]string memberName = ""  )
		{
			_AddLog(LogLevel.Operate, logTag.Tag, memberName, senderName);
		}

		/// <summary>
		/// 動作ログを出力します
		/// </summary>
		/// <param name="sourceName">発生元</param>
		/// <param name="actionName">動作名</param>
		public void AddActiongLog(string senderName,[CallerFilePath]string filepath="", [CallerMemberName]string memberName = ""  )
		{
			_AddLog(LogLevel.Action, Path.GetFileName(filepath), memberName, senderName);
		}
		/// <summary>
		/// 動作ログを出力します
		/// </summary>
		/// <param name="sourceName">発生元</param>
		/// <param name="actionName">動作名</param>
		public void AddActiongLog( string senderName,ILogTag logTag, [CallerMemberName]string memberName = ""  )
		{
			_AddLog(LogLevel.Action, logTag.Tag, memberName, senderName);
		}

		/// <summary>
		/// トレースログを出力します
		/// </summary>
		/// <param name="sourceName">発生元</param>
		/// <param name="actionName">動作名</param>
		public void AddTracegLog(string senderName,[CallerFilePath]string filepath="", [CallerMemberName]string memberName = ""  )
		{
			_AddLog(LogLevel.Trace, Path.GetFileName(filepath), memberName, senderName);
		}
		/// <summary>
		/// トレースログを出力します
		/// </summary>
		/// <param name="sourceName">発生元</param>
		/// <param name="actionName">動作名</param>
		public void AddTraceLog( string senderName,ILogTag logTag, [CallerMemberName]string memberName = ""  )
		{
			_AddLog(LogLevel.Trace, logTag.Tag, memberName, senderName);
		}
		/// <summary>
		/// デバッグログを出力します
		/// </summary>
		/// <param name="sourceName">発生元</param>
		/// <param name="actionName">動作名</param>
		public void AddDebugLog(string senderName,[CallerFilePath]string filepath="", [CallerMemberName]string memberName = ""  )
		{
			_AddLog(LogLevel.Debug, Path.GetFileName(filepath), memberName, senderName);
		}
		/// <summary>
		/// デバッグログを出力します
		/// </summary>
		/// <param name="sourceName">発生元</param>
		/// <param name="actionName">動作名</param>
		public void AddDebugLog( string senderName,ILogTag logTag, [CallerMemberName]string memberName = ""  )
		{
			_AddLog(LogLevel.Debug, logTag.Tag,memberName,senderName);
		}
		protected virtual void _AddLog(LogLevel logLevel,string tag,string methodName,string message="")
		{
			_AddLog(DateTime.Now, logLevel,tag,methodName,message);
		}
		protected virtual void _AddLog(DateTime date, LogLevel logLevel, string tag, string methodName, string message="")
		{
			_AddLog(new LogData(this, date, logLevel, message, methodName, tag));
		}
		protected virtual void _AddLog(LogData logData)
		{
			if (logData.Level <= this._logLevel)
			{
				lock (this._logQueue)
				{
					this._logQueue.Enqueue(logData);
				}
			}
		}
		/// <summary>
		/// ログを出力します。
		/// </summary>
		/// <returns>出力できたかどうか</returns>
		protected virtual bool _WriteLogFile()
		{
			bool result = false;
			if( ( this._logQueue != null ) && ( this._logQueue.Count > 0 ) ){
				var logList = new Queue<LogData>() ;
				lock( this._logQueue ){
					while( this._logQueue.Count != 0 ){
						logList.Enqueue( this._logQueue.Dequeue() ) ;
					}
				}
				FileStream fs = null;
				StreamWriter sw = null;
				bool initializetextwrite=false;
				while(logList.Count>0)
				{
					this._FileOperation(logList.Peek().ToLogString());
					try
					{
						if (this._isFilePathChenged)
						{
							continue;
						}
						if(fs==null)
						{
							if (!Directory.Exists(Path.GetDirectoryName(this._FilePath)))
							{
								Directory.CreateDirectory(Path.GetDirectoryName(this._FilePath));
							}
							fs = new FileStream(this._FilePath, FileMode.Append);
							sw = new StreamWriter(fs) ;
						}
						if(initializetextwrite)
						{
							string line=$"Start,{SEPARATION_TEXT}\r\n"+
										$"Start,{this._initialText}\r\n" +
										$"Start,{SEPARATION_TEXT}\r\n";
							this._FileOperation(line);
							if (this._isFilePathChenged)
							{
								continue;
							}
							sw.WriteLine(line);
							initializetextwrite=false;
						}
						sw.WriteLine(logList.Dequeue().ToLogString()) ;
						
						result = true ;

					}
					catch ( Exception ex )
					{
						result = false ;
						UserDebug.ErrorLog( ex ) ;
					}
					finally
					{
						if(logList.Count == 0||this._isFilePathChenged)
						{
							if(sw!=null)
							{
								sw.Close() ;
								sw.Dispose();
								sw= null ;
							}
							if(fs!=null)
							{
								fs.Close();
								fs.Dispose();
								fs = null ;
							}
							this._isFilePathChenged = false;
							initializetextwrite = true ;
						}
					}
				}
			}
			else
				result = true ;

			return( result ) ;
		}
		protected virtual void _FileOperation(string nextLine)
		{
		}
		public virtual void Dispose()
		{
			this._closeFlag = true;
			if( this._logTask != null ){
				this._logTask.Wait() ;
				this._logTask.Dispose() ;
			}

			// 最後に残留しているログを出力
			this._WriteLogFile() ;

		}

		#endregion
	}
}
