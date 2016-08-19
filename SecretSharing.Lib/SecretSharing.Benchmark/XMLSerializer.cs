using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Benchmark
{
    public class XMLSerializer<T>
    {
        public  void Serialize(T holder, string pathToWriteSerialization)
        {
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(T));

            System.IO.StreamWriter file = new System.IO.StreamWriter(
                pathToWriteSerialization);
            writer.Serialize(file, holder);
            file.Close();
        }

        public T Deserialize( string pathToReadSerialization)
        {
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StreamReader file = new System.IO.StreamReader(
                pathToReadSerialization);
            T result = (T)reader.Deserialize(file);
            return result;
        }
    }
}
