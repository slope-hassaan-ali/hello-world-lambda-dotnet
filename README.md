# Hello World AWS Lambda .NET

## Pre-requisites
Docker and Docker Compose and the CLI tool "awslocal"

Additionally the .NET Core SDK needs to be installed: https://dotnet.microsoft.com/download/dotnet/3.1

Once the .NET Core SDK is installed, install .NET AWS Lambda Tools: ``dotnet tool install --global Amazon.Lambda.Tools ``

## Step 1 - Run LocalStack

Use the following Docker Compose config to start up LocalStack:

```dockerfile
version: '3'

services:
  localstack:
    image: localstack/localstack:latest
    restart: unless-stopped
    container_name: localstack
    network_mode: bridge
    environment:
      - AWS_DEFAULT_REGION=us-east-1
      - DEFAULT_REGION=us-east-1
      - HOSTNAME_FROM_LAMBDA=localhost
      - LAMBDA_DOCKER_NETWORK=host
      - LAMBDA_EXECUTOR=docker
      - LOCALSTACK_API_KEY=${LOCALSTACK_API_KEY}
      - MAIN_CONTAINER_NAME=localstack
      - SERVICES=batch,ec2,ecr,ecs,events,lambda,s3,serverless,ssm,stepfunctions
      - DEBUG=1
    ports:
      - "4566:4566"
      - "4510:4510"
    volumes:
      - "${TMPDIR:-/tmp/localstack}:/tmp/localstack"
      - "/var/run/docker.sock:/var/run/docker.sock"
```

## Step 2 - Deploy to LocalStack
First package the code by running this from the root directory of this project: ``dotnet lambda package -pl src/HelloWorld/ -pt zip -o ./function.zip``

Now run this to deploy the function to LocalStack (make sure to replace "CHANGE THIS TO THE ABSOLUTE PATH TO function.zip" with the correct path):
```bash
awslocal lambda create-function \                                                                                         
  --function-name test-function \
  --environment "Variables={EnvironmentName=dev}" \
  --timeout 120 \
  --role "slope-dev-projection-role" \
  --runtime "dotnetcore3.1" \
  --zip-file fileb:///CHANGE THIS TO THE ABSOLUTE PATH TO function.zip \
  --handler "HelloWorld::HelloWorld.Function::Handler"
```

## Step 3 - Test
Test the function by running this: ``awslocal lambda invoke --function-name "test-function" --cli-binary-format raw-in-base64-out --payload '{ "input": 0 }' test.json``

**Issue/Bug:** The command to invoke the function times out instead of quickly finishing.

**Lambda Debug Output:**
```
INIT: Starting daemons...

ls-daemon: Starting XRay server loop on UDP port 2000

ls-daemon: Starting DNS server loop on UDP port 53

-----

[ERROR] [1624626751042] LAMBDA_RUNTIME Failed to get next invocation. No Response from endpoint

An error occurred while attempting to execute your code.: LambdaException


terminate called after throwing an instance of 'std::logic_error'

what(): basic_string::_M_construct null not valid

25 Jun 2021 13:12:31,045 [WARN] (invoke@invoke.c:297 errno: None) run_dotnet(dotnet_path, &args) failed
```