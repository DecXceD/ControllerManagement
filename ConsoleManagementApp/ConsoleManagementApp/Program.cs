using System.Net.Http;

HttpClient client = new HttpClient();

var response = await client.GetAsync("https://localhost:44340/api/User/Login?username=Admin&password=terces");
var content = await response.Content.ReadAsStringAsync();
Console.WriteLine(content);