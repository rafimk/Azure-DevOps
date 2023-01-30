namespace Azure_DevOps.Api.Services;

public class WorkItemService
{
    public readonly string _token = "";
    public readonly string _type = "Product Backlog Item";
    public readonly string _apiVersion = "api-version=6.0";
    public readonly string _uri = "http://xxxxx.xx/collection/projectname/_apis/wit/workitems/";
    public readonly string _epic = "Epic";
    public readonly string _feature = "Feature";
    public readonly string _back_log = "Product Backlog Item";
    public readonly string _ac = "Acceptance Criteria";
    public readonly string _ua = "UI Acceptance Criteria";


    public async Task CreateWorkItem(string title, string description)
    {
        var base_uri = "http://xxxxx.xx/collection/";
        var project_name = "xxxproject"

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(ASCIIEncoding.ASCII.GetByte(string.Format("{0}:{1}", "", _token))));
        
        List<Object> data = new List<Onject>()
        {
            new { op = "add", path = "/fields/System.Title", from = string.Empty, value = title },
            new { op = "add", path = "/fields/System.Description", from = string.Empty, value = description }
        };

        var json = JsonConvert.SerialieObject(data);

        string uri = String.Join("?", String.Join("/", base_uri, project_name, "_apis/wit/workitems", $"${_type}"), _apiVersion);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

        string result = await CreateWit(client, uri, content);

        if (!String.IsNullOrEmpty(result))
        {
            dynamic wit = JsonConvert.DeserializeObject<Object>(result);
            Console.WriteLine(JsonConvert.SerialieObject(wit, Formating.Indented));
        }

        Console.ReadLine();
        client.Dispose();
    }

    public async Task CreateChildWorkItem(string title, string description, string parentId)
    {
        var base_uri = "http://xxxxx.xx/collection/";
        var project_name = "xxxproject"

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(ASCIIEncoding.ASCII.GetByte(string.Format("{0}:{1}", "", _token))));
        
        List<Object> data = new List<Onject>()
        {
            new { op = "add", path = "/fields/System.Title", from = string.Empty, value = title },
            new { op = "add", path = "/fields/System.Description", from = string.Empty, value = description }
            new { op = "add", path = "/relations/-", from string.Empty, value = new { rel = "System.LinkTypes.Hierarchy-Reverse",
                url = $"http://xxxx.xx/collection/site/_apis/wit/workItems/{parentId}",
                attributes = new { comment = "Making a new link for dependency"}}}
        };

        var json = JsonConvert.SerialieObject(data);

        string uri = String.Join("?", String.Join("/", base_uri, project_name, "_apis/wit/workitems", $"${_type}"), _apiVersion);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

        string result = await CreateWit(client, uri, content);

        if (!String.IsNullOrEmpty(result))
        {
            dynamic wit = JsonConvert.DeserializeObject<Object>(result);
            Console.WriteLine(JsonConvert.SerialieObject(wit, Formating.Indented));
        }

        Console.ReadLine();
        client.Dispose();
    }


    public async Task<List<Object>> GetWorkItem(string workItemNo)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(ASCIIEncoding.ASCII.GetByte(string.Format("{0}:{1}", "", _token))));

                using (HttpResponseMessage response = client.GetAsync($"{_url}{workItemNo}?{_apiVersion}").Result)
                {
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync();
                    var jsonObject = Object.Parse(body);

                    Console.WriteLine(body);

                    return new List<Object>();
                }
            }
        }   
        catch (Exception ex)
        {
            return new List<Object>();
        }
    }


    private async Task<string> CreateWit(HttpClient client, string uri, HttpContent conten)
    {
        try
        {
            using (HttpResponseMessage response = await client.PostAsync(uri, content))
            {
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.ToString());
            return string.Empty;
        }
    }
}