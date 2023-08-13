using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace LeagueStatusBot.API.Services
{
    public class ChatGPTService
    {
        private readonly string ApiKey = "API KEY";
        public List<object> messages = new List<object>();

        public ChatGPTService()
        {
            messages.Add( 
            
                new { role = "system", content = """
                    You are a Dungeon Master for an RPG Game. With the following data, you will generate a short narrative to start off the game.
                    You will be given all player stats. Combine those with the player actions. Roll and determine all actions and outcomes.
                    You will roll/determine based off of D&D 5th Edition.
                    
                    RULES TO ALWAYS FOLLOW - NEVER BREAK THESE:
                    - Determine when the game has ended. If all members of a party are dead/If a party escapes.
                    - IF YOU DETERMINE A GAME HAS ENDED and cannot continue after your reply, the next reply you will ONLY REPLY BACK ^GAME^. This will end the game and reset the lobby.
                    - Players can NEVER use something they don't have in their inventory or equipment.
                    - Player can ALWAYS use the world around them and any items/objects
                    - Players that respond out of character and 'troll' or 'flaming', ignore their response and come up with an action for them.
                    - A Response for the end of a ROUND MUST have a JSON string for the update of each character's HP Pool, and if they are a NPC or Player - that can be read back. This will be parsed from the end of the response, parsing after a # symbol. 
                    Example: #[ { "Name": "darktideplayer192", "HitPoints": 10, "Player": true }, {"Name:" "Trogdor the Great", "HitPoints": 8, "Player": false } { "Name": "darktideplayer193", "HitPoints": 4, "Player": true } ]

                    Here is the game Data - Please be the GM and start off a short narrative with the following Data within 100 tokens. You do NOT need a JSON string at the end of this:
                    """ }   
            );
        }

        public async Task<string> GenerateCompletionAsync(string prompt)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

                messages.Add(new {role = "user", content = prompt});

                var payload = new
                {
                    model = "gpt-4",
                    messages = messages.ToArray(),
                    temperature =  1,
                    max_tokens = 100,
                    top_p = 1,
                    frequency_penalty = 0,
                    presence_penalty = 0
                };

                var serializedPayload = JsonConvert.SerializeObject(payload);
                Console.WriteLine(serializedPayload);

                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API Call Failed: {response.StatusCode}, {response.ReasonPhrase}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(jsonResponse);
                var assistantMessage = jsonObject["choices"]?[0]?["message"]?["content"]?.ToString();

                messages.Add(new { role = "assistant", content = assistantMessage });

                return assistantMessage;
            }
        }

        private void ResetMessages()
        {
            messages = new List<object>
            {
                new { role = "system", content = $"""
                    You are a Game Master for an RPG roleplaying game. You receive game data, and interpret to produce 'action shot' kind of storytelling.
                    Parties of players are dropped through portals. You are there to generate a random world for them to drop in.
                    You are the judge - you decide how much damage someone should take, how much damage someone should do, if someone trying to do something fails or succeeds. You judge this off the wits and smart thinking of the player from their responses.
                    
                    RULES TO ALWAYS FOLLOW - NEVER BREAK THESE:
                    - Players actions are ALWAYS rolled for. You make a random roll of the die to determine their success.
                    - Players can NEVER use something they don't have in their inventory or equipment. You state that they try to do that action, but fail.
                    - Player can ALWAYS use the world around them and any items/objects
                    - Players that respond out of character and 'troll' or 'flaming', ignore their response and come up with an action for them.
                    - Format your responses. You can wrap words in ** for bold, * for italic, __ for underlined.


                    Here is the game Data - Please be the GM and start off a short narrative with the following Data - Short and sweet if possible:
                    """ }
            };
        }

        public async Task<string> GenerateARound(string prompt)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

                messages.Add(new { role = "user", content = "Summarize and Report these ROUND Details in short story. Each only a sentence or two. Provide a JSON string summary at the end and after a #, unless you determine Game has Ended:\n" + prompt });

                var payload = new
                {
                    model = "gpt-4",
                    messages = messages.ToArray(),
                    temperature = 1,
                    max_tokens = 150,
                    top_p = 1,
                    frequency_penalty = 0,
                    presence_penalty = 0
                };

                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(jsonResponse);
                var assistantMessage = jsonObject["choices"]?[0]?["message"]?["content"]?.ToString();

                messages.Add(new { role = "assistant", content = assistantMessage });

                return assistantMessage;
            }
        }
    }
}
