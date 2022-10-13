# Configuration

```shell
cd src/VBkg.TgClientBot.Worker

dotnet user-secrets set "TelegramBotOptions:Token" "<BOT_TOKEN>"
dotnet user-secrets set "ServerGrpcApi:Host" "<SERVER_GRPC_HOST>"
dotnet user-secrets set "ServerGrpcApi:ApiKey" "<SERVER_GRPC_API_KEY>"
```