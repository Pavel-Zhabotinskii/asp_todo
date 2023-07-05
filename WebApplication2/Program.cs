using System.Text.RegularExpressions;
using WebApplication2.models;
using WebApplication2.services;


var builder = WebApplication.CreateBuilder();
var app = builder.Build();

string PATH = $"{Environment.CurrentDirectory}\\todoDataList.json";

FileService fileService = new FileService(PATH);
List<TodoList> todoList;

// начальные данные
todoList = fileService.LoadData();

app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;

    // 2e752824-1657-4c7f-844b-6ec2e168e99c
    string expressionForGuid = @"^/api/tasks/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
    if (path == "/api/tasks" && request.Method == "GET")
    {
        await GetAllTasks(response);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
    {
        // получаем id из адреса url
        string? id = path.Value?.Split("/")[3];
        await GetTask(id, response);
    }
    else if (path == "/api/tasks" && request.Method == "POST")
    {
        await CreateTask(response, request);
    }
    else if (path == "/api/tasks" && request.Method == "PUT")
    {
        await UpdateTask(response, request);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
    {
        string? id = path.Value?.Split("/")[3];
        await DeleteTask(id, response);
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();

// получение всех задач
async Task GetAllTasks(HttpResponse response)
{
    await response.WriteAsJsonAsync(todoList);
}
// получение одной задачи по id
async Task GetTask(string? id, HttpResponse response)
{
    // получаем задачу по id
    TodoList? task = todoList.FirstOrDefault((u) => u.Id == id);
    // если задача найдена, отправляем ее
    if (task != null)
        await response.WriteAsJsonAsync(task);
    // если не найдена, отправляем статусный код и сообщение об ошибке
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Задача не найдена" });
    }
}
// Удаление задачи
async Task DeleteTask(string? id, HttpResponse response)
{
    // получаем задачу по id
    TodoList? task = todoList.FirstOrDefault((u) => u.Id == id);
    // если задача найдена, удаляем ее
    if (task != null)
    {
        todoList.Remove(task);
        fileService.SaveData(todoList);
        await response.WriteAsJsonAsync(task);
    }
    // если не найдена, отправляем статусный код и сообщение об ошибке
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Задача не найдена" });
    }
}

//Создаем новую задачу
async Task CreateTask(HttpResponse response, HttpRequest request)
{
    try
    {
        // получаем данные задачи
        var task = await request.ReadFromJsonAsync<TodoList>();
        if (task != null)
        {
            // устанавливаем id для новой задачи
            task.Id = Guid.NewGuid().ToString();
            // добавляем задачу в список
            todoList.Add(task);
            fileService.SaveData(todoList);
            await response.WriteAsJsonAsync(task);
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
}

//Изменение задачи
async Task UpdateTask(HttpResponse response, HttpRequest request)
{
    try
    {
        // получаем данные задачи
        TodoList? taskData = await request.ReadFromJsonAsync<TodoList>();
        if (taskData != null)
        {
            // получаем задачу по id
            var task = todoList.FirstOrDefault(u => u.Id == taskData.Id);
            // если задача найдена, изменяем ее данные и отправляем обратно клиенту
            if (task != null)
            {
                task.isDone = taskData.isDone;
                task.Text = taskData.Text;
                fileService.SaveData(todoList);
                await response.WriteAsJsonAsync(task);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "Задача не найдена" });
            }
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
}