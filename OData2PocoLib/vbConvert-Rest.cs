
/**********************************************************************************************************************
 * A service provided by Refactoring Essentials for Visual Studio. 
 * It is included in the Visual Studio extension (v4.0+), where it runs locally without calling out to this REST API. 
 * Part of the SharpDevelop OSS project. MIT-licensed.
 ***********************************************************************************************************************/
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// Convert To/From CSharp and vb.net
/// </summary>
public class CodeConvertorRestService
{
    public static async Task<string> CodeConvert(string source, string lang = "cs2vb.net")
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            ConvertRequest p = new ConvertRequest
            {
                Code = source,
                RequestedConversion = lang
            };
           
            var url = "https://codeconverter.icsharpcode.net";

            client.BaseAddress = new Uri(url);
            //var response = await client.PostAsJsonAsync("/api/converter", p).ConfigureAwait(false);
            var json = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/converter", json).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var deserializedCode = JsonConvert.DeserializeObject<ConvertResponse>(data);
                if (deserializedCode.ConversionOk != null && (bool)deserializedCode.ConversionOk)
                {
                   
                    return deserializedCode.ConvertedCode;
                }
                //Console.WriteLine("Error in Code Conversion");
                return "Error in Code Conversion";
            }
            //Console.Write("Error: Code Converter Service isn't available");
            return "Error: Code Converter Service isn't available";
        }
    }

    public class ConvertRequest
    {

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }


        [JsonProperty(PropertyName = "requestedConversion")]
        public string RequestedConversion { get; set; }

    }

    public class ConvertResponse
    {

        [JsonProperty(PropertyName = "conversionOk")]
        public bool? ConversionOk { get; set; }


        [JsonProperty(PropertyName = "convertedCode")]
        public string ConvertedCode { get; set; }


        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; set; }

    }
}


