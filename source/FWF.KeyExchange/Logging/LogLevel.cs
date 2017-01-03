using System;
using System.Collections.Generic;
using System.Linq;

namespace FWF.KeyExchange.Logging
{
    [Serializable]
    public class LogLevel
    {
        public static readonly LogLevel Off = new LogLevel(int.MaxValue, "OFF");
        public static readonly LogLevel Emergency = new LogLevel(120000, "EMERGENCY");
        public static readonly LogLevel Fatal = new LogLevel(110000, "FATAL");
        public static readonly LogLevel Alert = new LogLevel(100000, "ALERT");
        public static readonly LogLevel Critical = new LogLevel(90000, "CRITICAL");
        public static readonly LogLevel Severe = new LogLevel(80000, "SEVERE");
        public static readonly LogLevel Error = new LogLevel(70000, "ERROR");
        public static readonly LogLevel Warn = new LogLevel(60000, "WARN");
        public static readonly LogLevel Notice = new LogLevel(50000, "NOTICE");
        public static readonly LogLevel Info = new LogLevel(40000, "INFO");
        public static readonly LogLevel Debug = new LogLevel(30000, "DEBUG");
        public static readonly LogLevel Trace = new LogLevel(20000, "TRACE");
        public static readonly LogLevel Traffic = new LogLevel(15000, "TRAFFIC");
        public static readonly LogLevel Verbose = new LogLevel(10000, "VERBOSE");
        public static readonly LogLevel All = new LogLevel(int.MinValue, "ALL");

        public static readonly LogLevel Stat = new LogLevel(1000, "STAT");

        private static readonly List<LogLevel> _allLevels;

        static LogLevel()
        {
            _allLevels = new List<LogLevel>
            {
                Off,
                Emergency,
                Fatal,
                Alert,
                Critical,
                Severe,
                Error,
                Warn,
                Notice,
                Info,
                Debug,
                Trace,
                Traffic,
                Verbose,
                All,
                Stat
            };
        }

        private readonly int _levelValue;
        private readonly string _levelName;

        public LogLevel(int levelValue, string levelName)
        {
            _levelValue = levelValue;
            _levelName = levelName;
        }

        public string Name
        {
            get { return _levelName; }
        }

        public int Value
        {
            get { return _levelValue; }
        }

        public static List<LogLevel> AllLogLevels
        {
            get { return _allLevels;  }
        } 

        public override int GetHashCode()
        {
            return _levelValue;
        }

        public override bool Equals(object obj)
        {
            var input = obj as LogLevel;

            if (input != null)
            {
                return input._levelValue == _levelValue;
            }

            return false;
        }

        public static bool operator ==(LogLevel x, LogLevel y)
        {
            if (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
            {
                return true;
            }
            if (object.ReferenceEquals(x, null))
            {
                return false;
            }
            if (object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x._levelValue == y._levelValue;
        }

        public static bool operator !=(LogLevel x, LogLevel y)
        {
            return !(x == y);
        }

        public static bool operator >=(LogLevel x, LogLevel y)
        {
            if (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
            {
                return true;
            }
            if (object.ReferenceEquals(x, null))
            {
                return false;
            }
            if (object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x._levelValue >= y._levelValue;
        }

        public static bool operator <=(LogLevel x, LogLevel y)
        {
            if (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
            {
                return true;
            }
            if (object.ReferenceEquals(x, null))
            {
                return false;
            }
            if (object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x._levelValue <= y._levelValue;
        }

        public static implicit operator int (LogLevel logLevel)
        {
            if (ReferenceEquals(logLevel, null))
            {
                return LogLevel.Off;
            }

            return logLevel.Value;
        }

        public static implicit operator LogLevel(int levelValue)
        {
            return _allLevels.FirstOrDefault(x => x.Value == levelValue);
        }

        public static implicit operator LogLevel(string levelName)
        {
            return _allLevels.FirstOrDefault(x => x._levelName.Equals(levelName, StringComparison.OrdinalIgnoreCase));
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", _levelName, _levelValue);
        }
    }
}
