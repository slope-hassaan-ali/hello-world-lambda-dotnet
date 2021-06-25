using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorld
{
    public class Parameters
    {
        public string Input { get; set; }
    }
    
    public class Function
    {
        public string Handler(Parameters parameters, ILambdaContext context)
        {
            return parameters.Input?.ToUpper();
        }
    }
}