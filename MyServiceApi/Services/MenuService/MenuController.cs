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
        DataContext dataContext = new DataContext();

        public async Task<ServerResponse<MealResponse>> GenerateMealRecommendation(MealRequest mealRequest)
        {
            ServerResponse<MealResponse> serverResponse = new();

            try
            {
                switch (mealRequest.SourceType)
                {
                    case SourceType.Local: // Parte de PERSONA A
                        ServerResponse<MealResponse> mealResponseLocal = GenerateMealRecommendationLocal(mealRequest);
                        if (!mealResponseLocal.Success)
                        {
                            throw new Exception(mealResponseLocal.Message);
                        }

                        serverResponse.Data = mealResponseLocal.Data;
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
                        ServerResponse<MealResponse> mealResponseDynamic = await GenerateMealRecommendationDynamic(mealRequest);
                        
                        if (!mealResponseDynamic.Success)
                        {
                            throw new Exception(mealResponseDynamic.Message);
                        }

                        serverResponse.Data = mealResponseDynamic.Data;
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

        // Call OpenAI for AI method
        private async Task<string> GetOpenAiResponse(MealRequest mealRequest)
        {
            // Construction of the OpenAI call
            string apiKeyFilePath = "Keys/openai.key";
            string apiKey = await File.ReadAllTextAsync(apiKeyFilePath);

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            string openaiSystemPrompt = @"You are a food recommendation system, there are 3 types 
                        of food (MainCourse, Drink, Dessert), you will always have one or two type of food as input: 
                        
                        One food and type input:
                        [ {""Name"": name, ""CourseType"": courseType} ]

                        Two food and type input:
                        [ {""Name"": name, ""CourseType"": courseType}, ""Name"": name, ""CourseType"": courseType} ]
                        
                        And you must generate recommendations for the other type or types as output: 
                        
                        For one food type input:
                        { ""RecommendedMeals"": 
                            [ 
                                {""Name"" : ""name"", ""CourseType"": ""courseType""}, 
                                {""Name"" : ""name"", ""CourseType"": ""courseType""} 
                            ] 
                        } 

                        For two food type input:
                        { ""RecommendedMeals"": 
                            [ 
                                {""Name"" : ""name"", ""CourseType"": ""courseType""}
                            ] 
                        } 
                        
                        For example, if the input have one CourseType like MainCourse, you must generate Drink and 
                        Dessert recommendations using the desired output format, dont use natural languaje 
                        only use the format:
                        
                        { ""RecommendedMeals"": 
                            [ 
                                {""Name"" : ""name"" , ""CourseType"": ""courseType""}, 
                                {""Name"" : ""name"", ""CourseType"": ""courseType""} 
                            ] 
                        }";

            string openaiUserPrompt;

            if (mealRequest.Meals.Count == 1)
                openaiUserPrompt = $"[{{\"Name\": {mealRequest.Meals[0].Name}, \"CourseType\": {mealRequest.Meals[0].Course}}}]";
            else
                openaiUserPrompt = $"[{{\"Name\": {mealRequest.Meals[0].Name}, \"CourseType\": {mealRequest.Meals[0].Course}}}, {{\"Name\": {mealRequest.Meals[1].Name}, \"CourseType\": {mealRequest.Meals[1].Course}}}]";

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
            string openaiResponse = await response.Content.ReadAsStringAsync();

            return openaiResponse;
        }

        public async Task<ServerResponse<MealResponse?>> GenerateMealRecommendationAI(MealRequest mealRequest)
        {
            ServerResponse<MealResponse?> serverResponse = new();

            try
            {
                string openaiResponse = await GetOpenAiResponse(mealRequest);

                // Deserialize the message from OpenAI to extract only the response message
                ChatCompletion? chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(openaiResponse);
                if (chatCompletion == null
                        || chatCompletion.Choices == null
                        || chatCompletion.Choices[0] == null
                        || chatCompletion.Choices[0].Message == null
                        || chatCompletion.Choices[0].Message!.Content == null)
                {
                    throw new Exception("An error occurred while getting the response from OpenAI");
                }
                string? deserializedResponse = chatCompletion.Choices[0].Message!.Content ?? throw new Exception("An error occurred while getting the response from OpenAI");

                // Parse the response message content from OpenAI to MealResponse object
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
                    throw new Exception("An error occurred while deserializing the OpenAI response");


                serverResponse.Success = true;
                serverResponse.Data = mealResponse;
            }
            catch (Exception e)
            {
                serverResponse.Success = false;
                serverResponse.Message = e.Message;
            }
            return serverResponse;
        }

        // Call external API for dynamic method.
        private async Task<string> CallExternalApi(MealRequest mealRequest)
        {
            try
            {
                using HttpClient client = new HttpClient();
                string url = "http://soa41d-project1.eastus.azurecontainer.io/recommendation/custom/?meal=tacos%20al%20pastor";
                HttpResponseMessage response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ServerResponse<MealResponse>> GenerateMealRecommendationDynamic(MealRequest mealRequest)
        {
            ServerResponse<MealResponse> serverResponse = new();
            MealResponse mealResponse = new();

            try
            {
                // Call external API
                string externalResponse = await CallExternalApi(mealRequest);
            
                // Parse response message 
                JsonDocument document = JsonDocument.Parse(externalResponse);
                JsonElement root = document.RootElement;
                string meal = root.GetProperty("meal").GetString();
                string drink = root.GetProperty("drink").GetString();
                string dessert = root.GetProperty("dessert").GetString();

                // Generate MealResponse object and add data from external API
                mealResponse.RecommendedMeals = new List<Meal>();
                mealResponse.RecommendedMeals.Add(new Meal { Name = meal, Course = CourseType.MainCourse });
                mealResponse.RecommendedMeals.Add(new Meal { Name = drink, Course = CourseType.Drink });
                mealResponse.RecommendedMeals.Add(new Meal { Name = dessert, Course = CourseType.Dessert });
            
            }catch(Exception e){

                serverResponse.Success = false;
                serverResponse.Message = e.Message;
                throw new Exception(e.Message);
            }
            
            serverResponse.Data = mealResponse;
            serverResponse.Success = true;
            return serverResponse;
        }

        // Get the local data for response
        private string GetLocalResponse(MealRequest mealRequest)
        {
            string localAnswer = dataContext.GetMissingRecommendations(mealRequest);
            return localAnswer;
        }

        public ServerResponse<MealResponse> GenerateMealRecommendationLocal(MealRequest mealRequest)
        {
            ServerResponse<MealResponse> serverResponse = new();
            try
            {
                string localResponse = GetLocalResponse(mealRequest);
                // Parse the response message content from LocalResponse to MealResponse object
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                };

                MealResponse? mealResponse = System.Text.Json.JsonSerializer.Deserialize<MealResponse>(localResponse, options);

                if (mealResponse == null || mealResponse.RecommendedMeals == null)
                    throw new Exception("An error occurred while deserializing the OpenAI response");


                serverResponse.Success = true;
                serverResponse.Data = mealResponse;
            }
            catch (Exception e)
            {
                serverResponse.Success = false;
                serverResponse.Message = e.Message;
            }
            return serverResponse;
        }
    }
}