using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestRadComboBox
{
public class Logger
{
public static void LogWriteError(string s)
{
using (FileStream stream = new FileStream("log.txt", 
    FileMode.OpenOrCreate, FileAccess.ReadWrite))
{
    StreamWriter sw = new StreamWriter(stream);
    sw.BaseStream.Seek(0, SeekOrigin.End);
    sw.Write(s);
    sw.Flush();
    sw.Close();
}

}
}
}
