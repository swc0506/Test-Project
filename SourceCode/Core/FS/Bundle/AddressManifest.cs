using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.FS
{
    public class AddressManifest
    {
        public const string NAME = "AddressManifest";
        
        private const char SEPARATOR = '|';

        private readonly Dictionary<string, string> addressMap = new Dictionary<string, string>();

        public AddressManifest(Dictionary<string, string>.Enumerator iter)
        {
            while (iter.MoveNext())
            {
                addressMap.Add(iter.Current.Key, iter.Current.Value);
            }
        }

        public AddressManifest(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            using (StringReader strReader = new StringReader(content))
            {
                string line;
                while (!string.IsNullOrEmpty((line = strReader.ReadLine())))
                {
                    string[] data = line.Split(SEPARATOR);
                    if (null != data && data.Length == 2)
                    {
                        addressMap.Add(data[0], data[1]);
                    }
                }
            }
        }

        public string GetAddress(string path)
        {
            if (addressMap.TryGetValue(path, out string abPath))
            {
                return abPath;
            }

            return null;
        }

        public int Count
        {
            get { return addressMap.Count; }
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            SortedDictionary<string, string> map = new SortedDictionary<string, string>(addressMap, new SortValue());
            foreach (var item in map)
            {
                strBuilder.AppendLine(string.Format("{0}{1}{2}", item.Key, SEPARATOR, item.Value));
            }

            return strBuilder.ToString();
        }
    }
}