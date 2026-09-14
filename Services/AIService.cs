using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Course_Management.Services
{
	public class AIService
	{
		private readonly IConfiguration _configuration;
		private readonly HttpClient _httpClient;

		public AIService(
			IConfiguration configuration,
			HttpClient httpClient)
		{
			_configuration = configuration;
			_httpClient = httpClient;
		}

		public async Task<string> AskAI(string question)
		{
			var apiKey = _configuration["Gemini:ApiKey"];
			var model = _configuration["Gemini:Model"];

			if (string.IsNullOrWhiteSpace(apiKey))
			{
				throw new Exception("Gemini API key is not configured.");
			}

			if (string.IsNullOrWhiteSpace(model))
			{
				model = "gemini-2.5-flash";
			}

			var url =
				$"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

			var requestBody = new
			{
				contents = new[]
				{
					new
					{
						parts = new[]
						{
							new
							{
								text = question
							}
						}
					}
				}
			};

			var json = JsonSerializer.Serialize(requestBody);

			using var content = new StringContent(
				json,
				Encoding.UTF8,
				"application/json"
			);

			var response = await _httpClient.PostAsync(url, content);

			var responseContent = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception(
					$"Gemini API error: {responseContent}"
				);
			}

			using var document =
				JsonDocument.Parse(responseContent);

			var answer =
				document.RootElement
					.GetProperty("candidates")[0]
					.GetProperty("content")
					.GetProperty("parts")[0]
					.GetProperty("text")
					.GetString();

			return answer ?? "Sorry, I couldn't generate a response.";
		}
	}
}