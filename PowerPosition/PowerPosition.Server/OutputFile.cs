namespace PowerPosition.Server
{
    public class OutputFile
    {
        private string _fileName;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">Name of file including the path</param>
        public OutputFile(string fileName)
        {
            _fileName = fileName;
        }
        /// <summary>
        /// Writes totals to the file.
        /// </summary>
        /// <param name="totals"></param>
        public void Write(IEnumerable<TotalPerTimePeriod> totals)
        {
            try
            {
                // Ensure the directory exists before writing
                string directory = Path.GetDirectoryName(_fileName);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var file = new StreamWriter(_fileName, append: false))
                {
                    file.WriteLine("Local Time,Volume");

                    foreach (var item in totals)
                    {
                        file.WriteLine($"{item.TimePeriod},{item.Total}");
                    }

                    file.Flush();
                }
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException($"Unable to write to file '{_fileName}'. The file may be locked by another process.", ex);
            }
        }
    }
}
