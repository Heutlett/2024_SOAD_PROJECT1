using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyServiceApi.Data;
using MyServiceApi.Models;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyServiceApi.Services.MenuService
{
    public class MenuController : IMenuController
    {
        private readonly DataContext _context;
        static readonly HttpClient client = new HttpClient();
        public MenuController(DataContext context)
        {
            _context = context;
        }

        public async Task<ServerResponse<MealResponse>> GenerateMealRecommendation(MealRequest mealRequest)
        {
            ServerResponse<MealResponse> serverResponse = new();

            try
            {
                switch (mealRequest.SourceType)
                {
                    case SourceType.Local: // Parte de PERSONA A
                        serverResponse.Success = true;
                        serverResponse.Message = "This is a response from local";
                        break;

                    case SourceType.AI: // Parte de PERSONA B
                        ServerResponse<MealResponse?> mealResponse = await GenerateMealRecommendationAI(mealRequest);

                        if (!mealResponse.Success)
                        {
                            throw new Exception(mealResponse.Message);
                        }

                        serverResponse.Data = mealResponse.Data;
                        serverResponse.Message = "This is a response from OpenAI";
                        break;

                    case SourceType.Dynamic: // Parte de PERSONA C
                        serverResponse.Message = "This is a response from dynamic";
                        break;
                }
                serverResponse.Success = true;
            }
            catch (Exception e)
            {
                serverResponse.Message = e.Message;
                serverResponse.Success = false;
            }
            return serverResponse;
        }

        public async Task<ServerResponse<MealResponse?>> GenerateMealRecommendationAI(MealRequest mealRequest)
        {
            ServerResponse<MealResponse?> serverResponse = new();

            try
            {
                // Verify if the meal in the mealRequest object is not null
                if (mealRequest.Meal == null
                        || mealRequest.Meal.Name == null
                        || mealRequest.Meal.Course == null)
                {
                    throw new Exception("The Meal property of the mealRequest is invalid, please fill out all fields with correct data");
                }

                // Construction of the OpenAI call
                string apiKeyFilePath = "Keys/openai.key";
                string apiKey = await File.ReadAllTextAsync(apiKeyFilePath);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                    string openaiSystemPrompt = @"You are a food recommendation system, there are 3 types 
                        of food (MainCourse, Drink, Dessert), you will always have one type of food as input 
                        {""Name"": name, ""CourseType"": courseType} and you must generate recommendations 
                        for the OTHER TWO types as output: 
                        { ""RecommendedMeals"": 
                            [ 
                                {""Name"" : ""name"", ""CourseType"": ""courseType""}, 
                                {""Name"" : ""name"", ""CourseType"": ""courseType""} 
                            ] 
                        }, for example, if the CourseType input is MainCourse, you must generate Drink and 
                        Dessert recommendations using the desired output format, dont use natural languaje 
                        only use the format:
                        { ""RecommendedMeals"": 
                            [ 
                                {""Name"" : ""name"" , ""CourseType"": ""courseType""}, 
                                {""Name"" : ""name"", ""CourseType"": ""courseType""} 
                            ] 
                        }";

                    string openaiUserPrompt = $"{{\"Name\": {mealRequest.Meal.Name}, \"CourseType\": {mealRequest.Meal.Course}}}";

                    var requestBody = new
                    {
                        model = "gpt-3.5-turbo",
                        messages = new[]
                        {
                        new { role = "system", content =  openaiSystemPrompt},
                        new { role = "user", content =  openaiUserPrompt}
                    }
                    };

                    // Call OpenAI
                    string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                    string callResponse = await response.Content.ReadAsStringAsync();

                    // Deserialize the message from OpenAI to extract only the response message
                    ChatCompletion? chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(callResponse);
                    if (chatCompletion == null
                            || chatCompletion.Choices == null
                            || chatCompletion.Choices[0] == null
                            || chatCompletion.Choices[0].Message == null
                            || chatCompletion.Choices[0].Message!.Content == null)
                    {
                        throw new Exception("An error occurred while getting the response from OpenAI");
                    }
                    string? deserializedResponse = chatCompletion.Choices[0].Message!.Content;
                    if (deserializedResponse == null)
                    {
                        throw new Exception("An error occurred while getting the response from OpenAI");
                    }

                    // Deserialize and convert the response message content from OpenAI to MealResponse object
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                    };
                    MealResponse? mealResponse = System.Text.Json.JsonSerializer.Deserialize<MealResponse>(deserializedResponse, options);
                    if (mealResponse == null || mealResponse.RecommendedMeals == null)
                    {
                        throw new Exception("An error occurred while deserializing the OpenAI response");
                    }

                    serverResponse.Success = true;
                    serverResponse.Data = mealResponse;
                }
            }
            catch (Exception e)
            {
                serverResponse.Success = false;
                serverResponse.Message = e.Message;
            }
            return serverResponse;
        }

        public Task<ServerResponse<MealResponse>> GenerateMealRecommendationDynamic(MealRequest mealRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ServerResponse<MealResponse>> GenerateMealRecommendationLocal(MealRequest mealRequest)
        {
            throw new NotImplementedException();
        }
    }
}