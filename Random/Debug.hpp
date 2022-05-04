#ifndef DEBUG_HPP_
#define DEBUG_HPP_

#include <fstream>
#include <string>
#include <sstream>
#include <ctime>

#define ENABLE_LOGGER

#define LOG(...) nfynt::DLog("Log", __VA_ARGS__)
#define WARN(...) nfynt::DLog("Warn", __VA_ARGS__)
#define ERROR(...) nfynt::DLog("Error", __VA_ARGS__)

namespace nfynt {
	enum class DebugType
	{
		Console,
		File
	};
	static DebugType debugType = DebugType::Console;
	const std::string debugFilename = "my_log.txt";
	static std::ofstream logFile;

	static void InitLogger(DebugType type)
	{
		debugType = type;
#ifndef ENABLE_LOGGER
		return;
#endif
		if (debugType == DebugType::File)
			logFile.open(debugFilename, std::ios::app);
	}

	static void FinalizeLogger()
	{
#ifndef ENABLE_LOGGER
		return;
#endif
		if (debugType == DebugType::File)
			logFile.close();
	}

	static std::string GetLogStamp(const char* logType)
	{
		std::time_t res = std::time(nullptr);
		tm ltm;
		localtime_s(&ltm, &res);
		std::stringstream ss;
		ss << "[" << logType << " - " << ltm.tm_hour << ":" << ltm.tm_min << " : " << ltm.tm_sec << "] ";
		return ss.str();
	}

	template <typename Arg, typename... Args>
	static void DLog(const char* logType, Arg&& arg, Args&&... args)
	{
#ifndef ENABLE_LOGGER
		return;
#endif
		if (debugType == DebugType::Console) {
			std::cout << GetLogStamp(logType)<< std::forward<Arg>(arg);
			using expander = int[];
			(void)expander {
				0, (void(std::cout << ',' << std::forward<Args>(args)), 0)...
			};
			std::cout << std::endl;
		}
		else
		{
			logFile << GetLogStamp(logType)<< std::forward<Arg>(arg);
			using expander = int[];
			(void)expander {
				0, (void(logFile << ',' << std::forward<Args>(args)), 0)...
			};
			logFile << std::endl;
			logFile.flush();
		}
	}
}
#endif
