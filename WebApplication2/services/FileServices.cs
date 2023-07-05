using Newtonsoft.Json;
using WebApplication2.models;

namespace WebApplication2.services
{
    public class FileService
    {
        private readonly string PATH;

        public FileService(string path) => PATH = path;

        public List<TodoList> LoadData()
        {
            var fileExists = File.Exists(PATH);
            if (!fileExists)
            {
                File.CreateText(PATH).Dispose();
                return new List<TodoList> ();
            }
            using (var reader = File.OpenText(PATH))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<TodoList>>(fileText);
            }
        }

        public void SaveData(object todoModelList)
        {
            using (StreamWriter writer = File.CreateText(PATH))
            {
                string output = JsonConvert.SerializeObject(todoModelList);
                writer.Write(output);
            }
        }
    }
}
