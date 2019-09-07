using FullSerializer;

namespace DumbML {
    public static class JsonSerializer {
        private static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize<T>(T value) {
            // serialize the data
            fsData data;

            _serializer.TrySerialize(typeof(T), value, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }

        public static T Deserialize<T>(string Json) {
            // step 1: parse the JSON data
            fsData data = fsJsonParser.Parse(Json);
            // step 2: deserialize the data
            object deserialized = null;
            _serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();

            return (T)deserialized;
        }
    }



}
