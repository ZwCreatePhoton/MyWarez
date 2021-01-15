using System;
using System.IO;


namespace MyWarez.Core
{
    public class TemporaryContext : IDisposable
    {
        private string tempDir { get; set; }
        private string previousCWD { get; set; }

        public TemporaryContext()
        {
            previousCWD = Directory.GetCurrentDirectory();
            var myTempDir = Constants.TempDirectory + Utils.RandomString(10);
            Directory.CreateDirectory(myTempDir);
            Directory.SetCurrentDirectory(myTempDir);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
            Directory.SetCurrentDirectory(previousCWD);
            // TODO: Delete the temp directory if in Release mode
        }
    }
}
