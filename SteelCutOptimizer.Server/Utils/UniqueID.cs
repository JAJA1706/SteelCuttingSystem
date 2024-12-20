using ampl;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace SteelCutOptimizer.Server.Utils
{
    public class UniqueID
    {
        private static HashSet<string> UsedIDs = new HashSet<string>();
        private static readonly object _lock = new object();

        private string _id;
        public string Get() { 
            return _id; 
        }

        public UniqueID()
        {
            string newId;
            lock (_lock)
            {
                do
                {
                    newId = GenerateRandomString();
                } while (UsedIDs.Contains(newId));

                UsedIDs.Add(newId);
            }
            _id = newId;
        }

        ~UniqueID()
        {
            lock (_lock)
            {
                UsedIDs.Remove(_id);
            }
        }

        private static string GenerateRandomString()
        {
            const int ID_LENGTH = 6; 
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, ID_LENGTH)
                                        .Select(_ => chars[random.Next(chars.Length)])
                                        .ToArray());
        }
    }
}
