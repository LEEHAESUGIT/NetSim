using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NETSIM_ConsoleView.LogMonitor
{
	public enum ELogType
	{
		DEBUG,
		INFO,
		WARN,
		ERROR,
		FATAL
	}

	public class LogView
	{
		private Channel<string> _logChannel = Channel.CreateUnbounded<string>();
		private readonly string _logFilePath = Path.GetFullPath("server_log.txt");
		private readonly string _logDirectory;

		#region Singletone
		/// <summary>
		/// 기존 알고있던 static형식의 싱글톤과 차이점
		/// 1. Lazy<>를 사용해 C#내부의 스레드 안전성 보장.
		/// 2. 프로그램 Runtime시 즉시 메모리에 객체가 올라가는게 아닌 Instance를 호출시 메모리에 올리는 
		///		방식.
		/// </summary>
		private static readonly Lazy<LogView> _instance = new Lazy<LogView>(() => new LogView());
		public static LogView Instance { get => _instance.Value; }
		private LogView()
		{

			this._logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
			Directory.CreateDirectory(_logDirectory);
			string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			_logFilePath = Path.Combine(_logDirectory, $"ServerLog_{timeStamp}.txt");


			string cmdCommand = $"powershell -Command \"Get-Content '{_logFilePath}' -Wait -Tail 20\"\n";
			cmdCommand += "----------------------------------------------------------------------\n\n";

			// 한글 경로 등에서 깨지지 않게 UTF8로 파일 생성
			File.AppendAllText(_logFilePath, cmdCommand, Encoding.UTF8);
			File.AppendAllText(_logFilePath, "-- Log Started --- \n");
			_ = Run();

		}
		#endregion

		private async Task Run()
		{
			await foreach (var logMessage in _logChannel.Reader.ReadAllAsync())
			{
				try
				{
					await File.AppendAllTextAsync(_logFilePath, logMessage);
				}
				catch (DirectoryNotFoundException ex)
				{
					// 1. "파일 경로가 잘못되었거나 폴더가 없을 때"
					// (AppendAllText는 파일은 만들어주지만, 폴더까지 자동으로 만들어주진 않습니다)
					Console.WriteLine($"[로깅 에러: 경로 없음] 지정된 폴더 경로를 찾을 수 없습니다. {ex.Message}");
				}
				catch (UnauthorizedAccessException ex)
				{
					// 2. "권한 문제로 텍스트 삽입 불가"
					// (파일이 '읽기 전용' 상태이거나, 관리자 권한이 필요한 폴더에 쓰려고 할 때)
					Console.WriteLine($"[로깅 에러: 쓰기 권한 없음] 파일에 접근할 권한이 없습니다. {ex.Message}");
				}
				catch (IOException ex)
				{
					// 3. "파일 잠김(Lock) 문제로 텍스트 삽입 불가" ⭐️ 가장 많이 발생!
					// (다른 프로그램이나 백신, 혹은 엑셀 같은 프로그램이 로그 파일을 꽉 물고 놔주지 않을 때)
					Console.WriteLine($"[로깅 에러: 파일 사용 중] 파일이 다른 프로세스에 의해 사용 중입니다. {ex.Message}");
				}
				catch (Exception ex)
				{
					// 4. 위의 3가지에 해당하지 않는 기타 모든 쌩뚱맞은 에러 (최후의 보루)
					Console.WriteLine($"[로깅 에러: 알 수 없는 치명적 오류] {ex.Message}");
				}

			}

		}

		public void OnLogMonitor()
		{
			var psi = new ProcessStartInfo
			{
				FileName = "powershell.exe",
				Arguments = $"-NoExit -Command \"Get-Content '{_logFilePath}' -Wait -Tail 20\"",

				UseShellExecute = true,
				CreateNoWindow = false
			};
			Process.Start(psi);

			
		}

		public void WriteLogFile(ELogType type, string body)
		{
			string resetCode = "\u001b[0m";
			string logMessage = "";
			switch (type)
			{
				case ELogType.DEBUG:
					logMessage = $"[\u001b[90m{type}{resetCode}][{DateTime.Now:HH:mm:ss.fff}] : {body}{resetCode} \n";
					break;
				case ELogType.INFO:
					logMessage = $"[\u001b[32m{type}{resetCode}][{DateTime.Now:HH:mm:ss.fff}] : {body}{resetCode} \n";
					break;
				case ELogType.WARN:
					logMessage = $"[\u001b[33m{type} {resetCode}][{DateTime.Now:HH:mm:ss.fff}] : {body}{resetCode} \n";
					break;
				case ELogType.ERROR:
					logMessage = $"[\u001b[31m{type} {resetCode}][{DateTime.Now:HH:mm:ss.fff}] : {body}{resetCode} \n";
					break;
				case ELogType.FATAL:
					logMessage = $"[\u001b[41m\u001b[37m{type}{resetCode}][{DateTime.Now:HH:mm:ss.fff}] : \u001b[41m\u001b[37m{body}{resetCode} \n";
					break;
				default:
					break;
			}

			_logChannel.Writer.TryWrite(logMessage);
		}

	}
}
