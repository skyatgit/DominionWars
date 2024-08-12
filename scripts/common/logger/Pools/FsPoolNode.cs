using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominionWars.Common.Logger;

public struct PoolFileConf
{
    // he log file dir.
    public string Dir;

    /// <summary>
    ///     The log file name. need to include suffix.
    ///     e.g. `main.log`
    /// </summary>
    public string Filename;

    /// <summary>
    /// </summary>
    public bool PathIsFormatString;

    /// <summary>
    ///     Log file maximum size. Use kilobytes as the unit. Default file size is 2mb.
    ///     If the value is 0 then the file has no size limit.
    /// </summary>
    public int? MaxSize;

    /// <summary>
    ///     Log file maximum cache file num(0~255). Five log files are cached by default.
    /// </summary>
    public int? RotateCount;

    /// <summary>
    ///     The encoding when writing to the log file.
    /// </summary>
    public Encoding Encoding;
}

public class FsPoolNode : PoolNode
{
    private readonly PoolFileConf _fileConf;
    private readonly string _filename;
    private StreamWriter _streamWriter;
    private string _streamWriterPath;

    public FsPoolNode(PoolFileConf poolFileConf)
    {
        _fileConf = poolFileConf;
        _fileConf.MaxSize ??= 1024 * 1024 * 2;
        _fileConf.Encoding ??= Encoding.UTF8;

        if (_fileConf.MaxSize < 0)
        {
            throw new Exception(
                $"FsPoolNode parameter range error! MaxSize must be 0~Infinity(${_fileConf.MaxSize}).");
        }

        if (_fileConf.RotateCount < 0)
        {
            throw new Exception(
                $"FsPoolNode parameter range error! RotateCount must be 0~Infinity(${_fileConf.RotateCount}).");
        }

        DateTime logDateTime = DateTime.Now;
        _filename = _fileConf.PathIsFormatString
            ? string.Format(_fileConf.Filename, logDateTime.ToString("yyyy_MM_dd_HH_mm_ss"))
            : _fileConf.Filename;
    }

    private async Task LogFileRename(string oldPath, string newPath, int cacheIndex = -1)
    {
        const string cachePath = "";
        if (_fileConf.RotateCount == null || cacheIndex < _fileConf.RotateCount)
        {
            if (!File.Exists(newPath))
            {
                await Task.Run(() => { File.Move(oldPath, newPath); });
            }

            if (cacheIndex == -1)
            {
                File.Move(oldPath, cachePath);
            }

            await LogFileRename(newPath, GetNextLogPath(newPath), cacheIndex + 1);
            if (cacheIndex == -1)
            {
                File.Move(oldPath, cachePath);
            }
        }
    }

    private int GetFileNum(string filename)
    {
        char[] numChars = { };
        filename.Split("").Reverse().Any(str =>
        {
            bool results = int.TryParse(str, out _);
            if (results)
            {
                ((IList)numChars).Add(str);
            }

            return results;
        });
        string numStr = string.Join("", numChars.Reverse());
        return int.Parse(numStr != ""
            ? numStr
            : filename.Substring(filename.Length - 3) == "old"
                ? "0"
                : "-1");
    }

    private string GetNextLogPath(string path, DateTime logDateTime = default)
    {
        string ext = Path.GetExtension(path);
        string filename = Path.GetFileName(path);
        string pathname = Path.GetDirectoryName(path);
        string newFilename = "";
        if (logDateTime == default)
        {
            int cacheIndex = GetFileNum(filename);
            string last = cacheIndex == 0 ? "_old" : $"_old_{cacheIndex}";
            if (filename.Contains(last))
            {
                newFilename = cacheIndex == 0
                    ? filename
                    : filename.Replace($"_old_{cacheIndex}", $"_old_{cacheIndex + 1}");
            }
            else
            {
                newFilename = $"{filename}_old";
            }
        }
        else
        {
            if (_fileConf.PathIsFormatString)
            {
                newFilename = string.Format(_fileConf.Filename, logDateTime.ToString("yyyy-MM-dd-HH:mm:ss"));
            }
        }

        return Path.Join(pathname, $"{newFilename}${ext}");
    }

    public override void On(LogData logData)
    {
        base.On(logData);
        string str = logData.Msg + Environment.NewLine;
        if (!Directory.Exists(_fileConf.Dir))
        {
            Directory.CreateDirectory(_fileConf.Dir);
        }

        string logPath = Path.Join(_fileConf.Dir, _filename);
        FileInfo fs = new FileInfo(logPath);
        if (File.Exists(logPath))
        {
            long size = fs.Length;
            if (_fileConf.MaxSize != null && size + str.Length >= _fileConf.MaxSize)
            {
                LogFileRename(logPath, GetNextLogPath(logPath)).RunSynchronously();
            }
        }
        else
        {
            File.Create(logPath).Close();
        }

        try
        {
            if (_streamWriter == null)
            {
                _streamWriterPath = logPath;
                _streamWriter = new StreamWriter(logPath, true, _fileConf.Encoding!);
            }
            else
            {
                if (_streamWriterPath != logPath)
                {
                    _streamWriter.Close();
                    _streamWriter = new StreamWriter(logPath, true, _fileConf.Encoding!);
                }
            }

            _streamWriter.Write(str);
        }
        catch (Exception e)
        {
            Logger.Error($"FsPoolNode StreamWriter error(${e.Message}).", "Logger");
        }
    }
}
