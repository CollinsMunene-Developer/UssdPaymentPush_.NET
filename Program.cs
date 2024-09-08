using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class UssdPaymentPush
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.africastalking.com/ussd"; // Replace with the correct API endpoint
    private const string ApiKey = "atsk_1cc4dcae69af4987d350ba6fcd2d86a74431b0aeb468da19802adf62ac9f9f261d111e23";

    public UssdPaymentPush()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("apiKey", ApiKey);
    }

    public async Task<bool> InitiatePayment(string phoneNumber, decimal amount, string currency)
    {
        var paymentRequest = new
        {
            phoneNumber = phoneNumber,
            amount = amount,
            currency = currency,
            transactionId = Guid.NewGuid().ToString()
        };

        var json = JsonConvert.SerializeObject(paymentRequest);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Payment Initiated Successfully: {responseContent}");
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Payment Initiation Failed. Status: {response.StatusCode}. Message: {errorContent}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Payment Initiation Failed: {ex.Message}");
            return false;
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var ussdPush = new UssdPaymentPush();
        bool result = await ussdPush.InitiatePayment("+1234567890", 100.00m, "USD");
        Console.WriteLine($"Payment Push Result: {result}");
    }
}