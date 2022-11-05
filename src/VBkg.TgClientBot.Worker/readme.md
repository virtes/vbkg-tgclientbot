# Configuration

```shell
cd src/VBkg.TgClientBot.Worker

dotnet user-secrets set "TelegramBotOptions:Token" "<BOT_TOKEN>"
dotnet user-secrets set "TelegramBotOptions:Username" "<BOT_USERNAME>"
dotnet user-secrets set "BackgroundRemoverClientOptions:Host" "<BACKGROUND_REMOVER_HOST>"
dotnet user-secrets set "ServerGrpcClientOptions:Host" "<SERVER_GRPC_HOST>"
dotnet user-secrets set "ServerGrpcClientOptions:ApiKey" "<SERVER_GRPC_API_KEY>"
```