using System;
using System.Collections.Generic;
using System.Text;

namespace PowerPosition.WindowsService
{
    public class ApplicationSettings
    {
        public string CsvFilePath { get; set; } = string.Empty;
        public int TimerInterval { get; set; }
    }
}
