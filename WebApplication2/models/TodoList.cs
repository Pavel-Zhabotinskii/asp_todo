namespace WebApplication2.models
{
    public class TodoList
    {
        public string Id { get; set; } = "";
        public string Text { get; set; } = "";
        public string Date { get; set; } = DateTime.Now.ToShortTimeString();
        public bool isDone { get; set; } = false;
    }
}
