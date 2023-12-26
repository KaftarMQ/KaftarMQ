using Newtonsoft.Json;

namespace Consumer;

class Consumer
{
    private readonly string _queueName;
    private readonly string _pullEndpoint;
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public Consumer(string queueName, string pullEndpoint, string apiKey)
    {
        _queueName = queueName;
        _pullEndpoint = pullEndpoint;
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }
    }

    public async Task PullMessagesAsync()
    {
        try
        {
            // Construct the URL with the queue name
            string url = $"{_pullEndpoint}?queue={_queueName}";

            // Send an HTTP GET request to the pull endpoint
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            // Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content
                string content = await response.Content.ReadAsStringAsync();

                // Deserialize the response into a message object
                var message = JsonConvert.DeserializeObject<Message>(content);

                // Process the received message
                ProcessMessage(message);
            }
            else
            {
                // Handle non-success status code
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Exception occurred: {ex.Message}");
        }
    }

    private void ProcessMessage(Message message)
    {
        // Process the message here
        Console.WriteLine($"Received message: {message.Content}");
    }

    private async Task AcknowledgeMessageAsync(string messageId)
    {
        try
        {
            // Construct the URL for acknowledgment
            string url = $"{_ackEndpoint}?messageId={messageId}";

            // Send an HTTP request to acknowledge the message
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Handle non-success status code
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
        }
    }

    // Rest of your Consumer class...
}